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
        /// <summary>
        /// Signals, if a command is not an one-liner and needs states support
        /// </summary>
        bool UsesStates { get; }
        string HelpText { get; }
        string Key { get; }
        ChatRestrictionType ChatRestriction { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="text"></param>
        /// <param name="bot"></param>
        /// <param name="step"></param>
        /// <returns>If needs state support True := finally finished => state support can be removed</returns>
        Task<bool> Execute(Message message, string text, TelegramBotClient bot, int step);
    }

    public abstract class Command : ICommand
    {
        protected Regex SplitParameterRegEx {  get { return new Regex(@"""[^ ""]*"" | '[^'] * '|[^\s]+"); } }
        public virtual bool UsesStates { get { return false; } }
        public virtual ChatRestrictionType ChatRestriction {  get { return ChatRestrictionType.None; } }

        public abstract string HelpText { get; }
        public abstract string Key { get; }
        public abstract Task<bool> Execute(Message message, string text, TelegramBotClient bot, int step);

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

        protected int GetUserId(Message message)
        {
            return message.From.Id;
        }

        //protected int GetUserId(CallbackQuery message)
        //{
        //    return message.From.Id;
        //}
    }

    public abstract class StatefulCommand : Command
    {
        public override bool UsesStates {  get { return true; } }

        protected Dictionary<int, Func<Message, string, TelegramBotClient, int, Task<bool>>> Steps = new Dictionary<int, Func<Message, string, TelegramBotClient, int, Task<bool>>>();

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

        protected void NextState(Message message, int step)
        {
            this.stateUpdateCommand.Execute(new StateUpdateRequest(new State(message.Chat.Id, this.Key, step)));
        }

        public override async Task<bool> Execute(Message message, string text, TelegramBotClient bot, int step)
        {
            if (step == 0)
            {
                // push current state to stack if new
                var lastState = this.statePeakQuery.Execute(message);
                if ((lastState == null) || (lastState.Command != this.Key))
                    this.statePushCommand.Execute(new StatePushRequest(new State(message.Chat.Id, this.Key, 0)));
            }

            if (this.Steps.ContainsKey(step))
            {
                var result = await this.Steps[step].Invoke(message, text, bot, step);

                // pop current state from stack if finished
                if (result == true)
                    this.statePopCommand.Execute(new StatePopRequest(new State(message.Chat.Id, this.Key )));

                return result;
            }

            return false;
        }
    }
}
