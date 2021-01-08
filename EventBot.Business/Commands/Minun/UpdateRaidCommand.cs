using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventBot.Business.Helper;
using EventBot.Business.Interfaces;
using EventBot.Business.Queries;
using EventBot.Business.TelegramProxies;
using EventBot.DataAccess.Commands;
using EventBot.DataAccess.Commands.Raid;
using EventBot.DataAccess.Queries.Raid;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.Minun
{
    public interface IModifyRaidCommand : ICommand
    {
    }


    public class ModifyRaidCommand : StatefulCommand, IModifyRaidCommand
    {
        private readonly IGetActiveUserRaids getActiveUserRaids;
        private readonly ISetRaidIdToUpdateCommand setRaidIdToUpdateCommand;

        private readonly IGetCurrentManualRaidQuery getCurrentManualRaidQuery;

        private readonly IDeletePollsByIdsCommand deletePollsByIdsCommand;

        private readonly IGetActivePollByRaidId getActivePollByRaidId;

        private readonly IGetActivePogoGroups getActivePogoGroups;

        private readonly ISetPokeIdForRaidCommand setPokeIdForRaidCommand;



        private readonly TelegramProxies.NimRaidBot nimRaidBot;

        public ModifyRaidCommand(
            IStateUpdateCommand stateUpdateCommand, 
            IStatePushCommand statePushCommand, 
            IStatePopCommand statePopCommand, 
            StatePeakQuery statePeakQuery,

            IGetActiveUserRaids getActiveUserRaids,
            ISetRaidIdToUpdateCommand setRaidIdToUpdateCommand,
            IDeletePollsByIdsCommand deletePollsByIdsCommand,
            IGetCurrentManualRaidQuery getCurrentManualRaidQuery,
            IGetActivePollByRaidId getActivePollByRaidId,
            IGetActivePogoGroups getActivePogoGroups,
            ISetPokeIdForRaidCommand setPokeIdForRaidCommand,


            TelegramProxies.NimRaidBot nimRaidBot
            
            ) : base(stateUpdateCommand, statePushCommand, statePopCommand, statePeakQuery)
        {
            this.getActiveUserRaids = getActiveUserRaids;
            this.setRaidIdToUpdateCommand = setRaidIdToUpdateCommand;
            this.deletePollsByIdsCommand = deletePollsByIdsCommand;
            this.getCurrentManualRaidQuery = getCurrentManualRaidQuery;
            this.getActivePollByRaidId = getActivePollByRaidId;
            this.getActivePogoGroups = getActivePogoGroups;
            this.setPokeIdForRaidCommand = setPokeIdForRaidCommand;

            this.nimRaidBot = nimRaidBot;

            base.Steps.Add(0, Step0);
            base.Steps.Add(1, Step1);
            base.Steps.Add(2, Step2);   
            base.Steps.Add(3, Step3);
            base.Steps.Add(4, Step4);

            base.Steps.Add(5, Step5);
            base.Steps.Add(6, Step6);
            base.Steps.Add(7, Step7);

        }

        public override string HelpText => "Erlaubt die Anpassung eines Raids";

        public override string Key => "/update";

        public override ChatRestrictionType ChatRestriction => ChatRestrictionType.Private;

        protected async Task<StateResult> Step0(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var chatId = base.GetChatId(message);

            //await bot.SendTextMessageAsync(chatId, "Aktualisieren eines Raids.").ConfigureAwait(false);

            return await this.Step1(message, text, bot, batchMode);
        }

        protected async Task<StateResult> Step1(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            if (!batchMode)
                await bot.SendTextMessageAsync(chatId, "Bitte gib die Raid-Id an:.").ConfigureAwait(false);

            return StateResult.AwaitUserAt(2);
        }

         
        protected async Task<StateResult> Step2(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            if (!int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int raidId))
            {
                await bot.SendTextMessageAsync(chatId, "Es wurde keine gültige Raid-Id ausgewählt, bitte probiere es noch einmal.").ConfigureAwait(false);
                return StateResult.TryAgain;
            }

            var raids = this.getActiveUserRaids.Execute(new GetActiveUserRaidsRequest(UserId: userId));
            if (raids?.Any(x => x == raidId) ?? false)
            {
                await bot.SendTextMessageAsync(chatId, "Raid wurde nicht vom Nutzer erstellt oder ist abgelaufen. Raid-Id:").ConfigureAwait(false);
                return StateResult.TryAgain;
            }

            this.setRaidIdToUpdateCommand.Execute(new SetRaidIdToUpdateRequest(UserId: userId, RaidId: raidId));

            return await this.Step3(message, text, bot, batchMode);
        }

        protected async Task<StateResult> Step3(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            var msg = new StringBuilder("Aktion ausführen:");
            msg.AppendLine();
            msg.AppendLine("1 - Löschen");
            msg.AppendLine("2 - Poke-Id anpassen");
            // msg.AppendLine("3 - Ort anpassen");
            // msg.AppendLine("4 - Startzeit verschieben");
            // msg.AppendLine("5 - Level anpassen");

            await bot.SendTextMessageAsync(chatId, msg.ToString()).ConfigureAwait(false);

            return StateResult.AwaitUserAt(4);
        }

        protected async Task<StateResult> Step4(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            if (!SkipCurrentStep(text))
            {
                var userId = base.GetUserId(message);
                var chatId = base.GetChatId(message);

                if (!int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int actionId))
                {
                    await bot.SendTextMessageAsync(chatId, "Es wurde keine Zahl ausgewählt, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return StateResult.TryAgain;
                }

                if (actionId < 1 || 5 < actionId)
                {
                    await bot.SendTextMessageAsync(chatId, "Es wurde keine gültige Zahl ausgewählt, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return StateResult.TryAgain;            
                }

                switch(actionId)
                {
                    case 1: 
                        return await this.Step5(message, text, bot, batchMode);
                    case 2:
                        return await this.Step6(message, text, bot, batchMode);

                }

                // TODO:
            }

            return StateResult.TryAgain;
        }


        protected async Task<StateResult> Step5(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var userId = base.GetUserId(message);

            var raidId = this.getCurrentManualRaidQuery.Execute(new GetCurrentManualRaidRequest(UserId: userId)).UpdRaidId;


            var chats = this.getActivePogoGroups.Execute(new GetActivePogoGroupsRequest(BotIds: new long[] { this.nimRaidBot.BotId }));
            int numberOfCurrentActiveUsers = chats.Count(x => x.RaidLevel.HasValue);
            if (numberOfCurrentActiveUsers <= 0)
                return StateResult.Finished;

            foreach(var chat in chats)
            {
                try
                {
                    var poll = this.getActivePollByRaidId.Execute(new GetActivePollByRaidRequest(RaidId: raidId, ChatId: chat.ChatId));

                    if (poll != null)
                    {
                        this.deletePollsByIdsCommand.Execute(new DeletePollsByIdsRequest(Ids: new[] { poll.Id }));
                        await this.nimRaidBot.DeleteMessageAsync(chat.ChatId, (int)poll.MessageId).ConfigureAwait(false);
                    }
                }
                catch
                {
                    await Operator.SendMessage(bot, $"Manuelles löschen von Raid {raidId} für Chat {chat.ChatId} fehlgeschlagen.");
                }
            }

            return await this.StepFinished(message, text, bot, batchMode);
        }

        protected async Task<StateResult> Step6(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var chatId = base.GetChatId(message);
            await bot.SendTextMessageAsync(chatId, "Poke-Id:");


            return StateResult.AwaitUserAt(7);
        }

        protected async Task<StateResult> Step7(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            if (!SkipCurrentStep(text))
            {
                string[] input = message.Text.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                string poke = input.Length >= 1 ? input[0].Trim() : string.Empty;
                char? form = input.Length >= 2 ? (char?)input[1][0] : null;

                if (!int.TryParse(poke, NumberStyles.Any, CultureInfo.InvariantCulture, out int pokeId))
                {
                    await bot.SendTextMessageAsync(chatId, "Die Poke-Id konnte nicht erkannt werden, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return StateResult.TryAgain;
                }

                if (pokeId < 0 || 2000 < pokeId)
                {
                    await bot.SendTextMessageAsync(chatId, "Keine gültige Poke-Id, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return StateResult.TryAgain;
                }

                var raidId = this.getCurrentManualRaidQuery.Execute(new GetCurrentManualRaidRequest(UserId: userId)).UpdRaidId;


                this.setPokeIdForRaidCommand.Execute(new SetPokeIdForRaidRequest(RaidId: raidId, PokeId: pokeId, PokeForm: form));
            }

            return await this.StepFinished(message, text, bot, batchMode);
        }



        protected async Task<StateResult> StepFinished(Message message, string text, TelegramBotClient bot, bool batchMode)        
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            await bot.SendTextMessageAsync(chatId, $"Fertig.").ConfigureAwait(false);

            return StateResult.Finished;
        }

        private bool SkipCurrentStep(string text)
        {
            // geht eigentlich nicht
            if (string.IsNullOrWhiteSpace(text))
                return true;

            return text.Trim().ToLowerInvariant().Equals("x");
        }
    }
}