using EventBot.Business.TelegramProxies;
using EventBot.DataAccess.Commands;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Interfaces
{
    [Flags]
    public enum ChatRestrictionType
    {
        //None = 0,
        Private = 1,
        Group = 2,
        Channel = 4,
        None = 15
    }


    public interface ICommand
    {
        string HelpText { get; }
        string Key { get; }
        ChatRestrictionType ChatRestriction { get; }

        Task ExecuteAsync(Message message, string text, TelegramBotClient bot);
    }

    public interface IStatefulCommand : ICommand
    {
        Task ExecuteStepAsync(Message message, string text, TelegramBotClient bot, int step);
    }

    public abstract class Command : ICommand
    {
        protected Regex SplitParameterRegEx {  get { return new Regex(@"""[^ ""]*"" | '[^'] * '|[^\s]+"); } }
        public virtual ChatRestrictionType ChatRestriction {  get { return ChatRestrictionType.None; } }

        public abstract string HelpText { get; }
        public abstract string Key { get; }
        public abstract Task ExecuteAsync(Message message, string text, TelegramBotClient bot);

        protected long GetChatId(Message message)
        {
            return message.Chat.Id;
        }

        //protected long GetChatId(CallbackQuery message)
        //{
        //    return message.Message.Chat.Id;
        //}

        protected int GetMessageId(Message message)
        {
            return message.MessageId;
        }

        //protected int GetMessageId(CallbackQuery message)
        //{
        //    return message.Message.MessageId;
        //}

        protected long GetUserId(Message message)
        {
            return message.From.Id;
        }

        //protected int GetUserId(CallbackQuery message)
        //{
        //    return message.From.Id;
        //}
    }

    public abstract class StatefulCommand : Command, IStatefulCommand
    {
        public struct StateResult
        {
            private enum States
            {
                Finished,
                Store,
                DoNothing,
                ContinueWith,
            }

            private States _state;

            private StateResult(int next, States state)
            {
                Next = next;
                _state = state;
            }


            public int Next { get; }
            public bool Store => _state == States.Store;
            public bool IsFinished => _state == States.Finished;
            public bool IsTryAgain => _state == States.DoNothing;


            public static StateResult ContinueWith(int next)
            {
                return new StateResult(next, States.ContinueWith);
            }

            public static StateResult AwaitUserAt(int next)
            {
                return new StateResult(next, States.Store);
            }

            public static StateResult TryAgain { get; } = new StateResult(-1, States.DoNothing);

            public static StateResult Finished { get; } = new StateResult(-1, States.Finished);
        }

        protected delegate Task<StateResult> ExecuteStep(Message message, string text, TelegramBotClient bot, bool batchMode = false);

        protected Dictionary<int, ExecuteStep> Steps = new Dictionary<int, ExecuteStep>();

        readonly protected IStateUpdateCommand stateUpdateCommand;
        readonly protected IStatePopCommand statePopCommand;
        readonly protected IStatePushCommand statePushCommand;
        readonly protected Business.Queries.StatePeakQuery statePeakQuery;

        public StatefulCommand(
            IStateUpdateCommand stateUpdateCommand,
            IStatePushCommand statePushCommand,
            IStatePopCommand statePopCommand,
            Queries.StatePeakQuery statePeakQuery
            )
        {
            this.stateUpdateCommand = stateUpdateCommand;
            this.statePopCommand = statePopCommand;
            this.statePushCommand = statePushCommand;
            this.statePeakQuery = statePeakQuery;
        }

        public override async Task ExecuteAsync(Message message, string text, TelegramBotClient bot)
        {
            // push current state to stack if new
            var lastState = this.statePeakQuery.Execute(message);
            if ((lastState == null) || (lastState.Command != this.Key))
                this.statePushCommand.Execute(new StatePushRequest(new State(message.Chat.Id, this.Key, 0)));

            if (!string.IsNullOrWhiteSpace(text))
            {
                var result = await this.StepBatch(message, text, bot);
                UpdateState(message, result);
                return;
            }

            await ExecuteStepAsync(message, text, bot, 0);
        } 

        protected async Task<StateResult> StepBatch(Message message, string text, TelegramBotClient bot)
        {
            int stepIdx = 0;
            int parameterIdx = 0;
            int lastStepIdx = 0;
            string lastParameter = text;
            string parameter = text;

            string[] parameters = text?.Split(',') ?? new string[0];
            int count = parameters.Length;
           
            while (true)
            {
                var batchMode = parameterIdx != count;

                var stepResult = await this.Steps[stepIdx].Invoke(message, parameter.Trim(), bot, batchMode);
                if (stepResult.IsFinished)
                    return stepResult;

                if (stepResult.Store)
                {
                    if (parameterIdx >= count)
                        return stepResult;

                    lastParameter = parameter;
                    parameter = parameters[parameterIdx];
                    parameterIdx++;
                    
                    lastStepIdx = stepIdx;
                    stepIdx = stepResult.Next;
                }

                if (stepResult.IsTryAgain)
                    return await this.Steps[lastStepIdx].Invoke(message, lastParameter.Trim(), bot, false);
            }

            return StateResult.TryAgain;
        }

        public async Task ExecuteStepAsync(Message message, string text, TelegramBotClient bot, int step)
        {
            if (this.Steps.ContainsKey(step))
            {
                var result = await this.Steps[step].Invoke(message, text, bot, false);
                UpdateState(message, result);
            }
        }

        private void UpdateState(Message message, StateResult result)
        {
            // pop current state from stack if finished
            if (result.IsFinished)
            {
                this.statePopCommand.Execute(new StatePopRequest(new State(message.Chat.Id, this.Key)));
                return;
            }

            if (result.Store)
            {
                this.stateUpdateCommand.Execute(new StateUpdateRequest(new State(message.Chat.Id, this.Key, result.Next)));
                return;
            }
        }

    }
}
