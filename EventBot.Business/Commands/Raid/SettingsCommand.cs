using EventBot.Business.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;
using System.Globalization;
using EventBot.DataAccess.Commands.Raid;

namespace EventBot.Business.Commands.Raid
{
    public interface ISettingsCommand : ICommand
    { }

    public class SettingsCommand : StatefulCommand, ISettingsCommand
    {
        private readonly DataAccess.Commands.Raid.ISetNordCommand setNordCommand;
        private readonly DataAccess.Commands.Raid.ISetWestCommand setWestCommand;
        private readonly DataAccess.Commands.Raid.ISetEastCommand setEastCommand;
        private readonly DataAccess.Commands.Raid.ISetSouthCommand setSouthCommand;
        private readonly DataAccess.Commands.Raid.IActivateChatCommand activateChatCommand;
        private readonly DataAccess.Commands.Raid.ISetMinRaidLevelCommand setRaidLevelCommand;
        private readonly DataAccess.Commands.Raid.IAddChatCommand addChatCommand;

        private readonly DataAccess.Queries.Raid.IGetCurrentChatSettingsQuery getCurrentChatSettingsQuery;


        public SettingsCommand(
            DataAccess.Commands.IStateUpdateCommand stateUpdateCommand,
            DataAccess.Commands.IStatePopCommand statePopCommand,
            DataAccess.Commands.IStatePushCommand statePushCommand,
            Queries.StatePeakQuery statePeakQuery,

            DataAccess.Commands.Raid.ISetNordCommand setNordCommand,
            DataAccess.Commands.Raid.ISetWestCommand setWestCommand,
            DataAccess.Commands.Raid.ISetEastCommand setEastCommand,
            DataAccess.Commands.Raid.ISetSouthCommand setSouthCommand,
            DataAccess.Commands.Raid.IActivateChatCommand activateChatCommand,
            DataAccess.Commands.Raid.ISetMinRaidLevelCommand setRaidLevelCommand,
            DataAccess.Commands.Raid.IAddChatCommand addChatCommand,

            DataAccess.Queries.Raid.IGetCurrentChatSettingsQuery getCurrentChatSettingsQuery
            )
            : base(stateUpdateCommand, statePushCommand, statePopCommand, statePeakQuery)
        {
            this.setNordCommand = setNordCommand;
            this.setWestCommand = setWestCommand;
            this.setEastCommand = setEastCommand;
            this.setSouthCommand = setSouthCommand;
            this.activateChatCommand = activateChatCommand;
            this.setRaidLevelCommand = setRaidLevelCommand;
            this.addChatCommand = addChatCommand;

            this.getCurrentChatSettingsQuery = getCurrentChatSettingsQuery;


            base.Steps.Add(0, this.Step0);

            base.Steps.Add(1, this.Step1);
            base.Steps.Add(2, this.Step2);

            base.Steps.Add(3, this.Step3);
            base.Steps.Add(4, this.Step4);

            base.Steps.Add(5, this.Step5);
            base.Steps.Add(6, this.Step6);

            base.Steps.Add(7, this.Step7);
            base.Steps.Add(8, this.Step8);

            base.Steps.Add(9, this.Step9);
            base.Steps.Add(10, this.Step10);

        }

        public override string HelpText
        {
            get { return "Konfiguriert den Bot. /settings"; }
        }

        public override string Key
        {
            get { return "/settings"; }
        }

        public override ChatRestrictionType ChatRestriction => ChatRestrictionType.Group;

        protected async Task<StateResult> Step0(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var userId = message.From.Id;
            if (message.Chat.Type != Telegram.Bot.Types.Enums.ChatType.Private)
            {
                var admins = await bot.GetChatAdministratorsAsync(message.Chat.Id).ConfigureAwait(false);

                if (!admins.Any(x => x.User.Id == userId))
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, "Keine Administratorrechte").ConfigureAwait(false);
                    return StateResult.Finished;
                }
            }

            await bot.SendTextMessageAsync(message.Chat.Id, "Willkommen bei der Einrichtung des Bots").ConfigureAwait(false);
            await bot.SendTextMessageAsync(message.Chat.Id, "Du kannst 'x' antworten, dann wird der ursprüngliche Wert beibehalten.").ConfigureAwait(false);
            await bot.SendTextMessageAsync(message.Chat.Id, "Begrenze bitte das Gebiet, in dem der Bot reagieren soll").ConfigureAwait(false);

            return await this.Step1(message, text, bot, batchMode);
        }

        protected async Task<StateResult> Step1(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var currentSettings = this.getCurrentChatSettingsQuery.Execute(new DataAccess.Queries.Raid.GetCurrentChatSettingsRequest(ChatId: message.Chat.Id));

            var north = _Helper.GetNorth(currentSettings)?.ToString(CultureInfo.InvariantCulture) ?? "-";
            // TODO: leer -> alte koordinate zählt + Größenbegrenzung

            await bot.SendTextMessageAsync(message.Chat.Id, $"Aktueller Wert {north} \n\rKoordinate, die nach Norden begrenzt", replyMarkup: new ForceReplyMarkup());
            return  StateResult.AwaitUserAt(2);
        }

        protected async Task<StateResult> Step2(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            if (!SkipCurrentStep(text))
            {
                if (!decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal north))
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, "Koordinate, die nach Norden begrenzt konnte nicht erkannt werden, bitte probiere es noch einmal.", replyMarkup: new ForceReplyMarkup());
                    return StateResult.TryAgain;
                }

                this.setNordCommand.Execute(new DataAccess.Commands.Raid.SetNordRequest(ChatId: message.Chat.Id, Nord: north));
            }
            return await this.Step3(message, text, bot, batchMode);
        }

        protected async Task<StateResult> Step3(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var currentSettings = this.getCurrentChatSettingsQuery.Execute(new DataAccess.Queries.Raid.GetCurrentChatSettingsRequest(ChatId: message.Chat.Id));
            var east = _Helper.GetEast(currentSettings)?.ToString(CultureInfo.InvariantCulture) ?? "-";

            await bot.SendTextMessageAsync(message.Chat.Id, $"Aktueller Wert {east} \n\rKoordinate, die nach Osten begrenzt", replyMarkup: new ForceReplyMarkup());

            return StateResult.AwaitUserAt(4);
        }

        protected async Task<StateResult> Step4(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            if (!SkipCurrentStep(text))
            {
                if (!decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal east))
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, "Koordinate, die nach Osten begrenzt konnte nicht erkannt werden, bitte probiere es noch einmal.", replyMarkup: new ForceReplyMarkup());
                    return StateResult.TryAgain;
                }

                this.setEastCommand.Execute(new DataAccess.Commands.Raid.SetEastRequest(ChatId: message.Chat.Id, East: east));
            }

            return await this.Step5(message, text, bot, batchMode);
        }

        protected async Task<StateResult> Step5(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var currentSettings = this.getCurrentChatSettingsQuery.Execute(new DataAccess.Queries.Raid.GetCurrentChatSettingsRequest(ChatId: message.Chat.Id));
            var south = _Helper.GetSouth(currentSettings)?.ToString(CultureInfo.InvariantCulture) ?? "-";

            await bot.SendTextMessageAsync(message.Chat.Id, $"Aktueller Wert {south} \n\rKoordinate, die nach Süden begrenzt", replyMarkup: new ForceReplyMarkup());
            return StateResult.AwaitUserAt(6);
        }

        protected async Task<StateResult> Step6(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            if (!SkipCurrentStep(text))
            {
                if (!decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal south))
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, "Koordinate, die nach Süden begrenzt konnte nicht erkannt werden, bitte probiere es noch einmal.", replyMarkup: new ForceReplyMarkup());
                    return StateResult.TryAgain;
                }

                this.setSouthCommand.Execute(new DataAccess.Commands.Raid.SetSouthRequest(ChatId: message.Chat.Id, South: south));
            }
            return await this.Step7(message, text, bot, batchMode);
        }

        protected async Task<StateResult> Step7(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var currentSettings = this.getCurrentChatSettingsQuery.Execute(new DataAccess.Queries.Raid.GetCurrentChatSettingsRequest(ChatId: message.Chat.Id));
            var west = _Helper.GetWest(currentSettings)?.ToString(CultureInfo.InvariantCulture) ?? "-";

            await bot.SendTextMessageAsync(message.Chat.Id, $"Aktueller Wert {west} \n\rKoordinate, die nach Westen begrenzt", replyMarkup: new ForceReplyMarkup());
            return StateResult.AwaitUserAt(8);
        }
 
        protected async Task<StateResult> Step8(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            if (!SkipCurrentStep(text))
            {
                if (!decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal west))
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, "Koordinate, die nach Westen begrenzt konnte nicht erkannt werden, bitte probiere es noch einmal.", replyMarkup: new ForceReplyMarkup());
                    return StateResult.TryAgain;
                }

                this.setWestCommand.Execute(new DataAccess.Commands.Raid.SetWestRequest(ChatId: message.Chat.Id, West: west));
            }

            return await this.Step9(message, text, bot, batchMode);
        }

        protected async Task<StateResult> Step9(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var currentSettings = this.getCurrentChatSettingsQuery.Execute(new DataAccess.Queries.Raid.GetCurrentChatSettingsRequest(ChatId: message.Chat.Id));

            await bot.SendTextMessageAsync(message.Chat.Id, $"Aktueller Wert {currentSettings?.RaidLevel ?? 1} \n\rMinimum Raid-Level", replyMarkup: new ForceReplyMarkup());
            return StateResult.AwaitUserAt(10);
        }

        protected async Task<StateResult> Step10(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            if (!SkipCurrentStep(text))
            {
                if (!int.TryParse(text, out int level))
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, "Raid Level konnte nicht erkannt werden. Probiere es bitte noch einmal", replyMarkup: new ForceReplyMarkup());
                    return StateResult.TryAgain;
                }

                if (level < 0 || 5 < level)
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, "Kein gültiges Raidlevel. Probiere es bitte noch einmal", replyMarkup: new ForceReplyMarkup());
                    return StateResult.TryAgain;
                }

                this.setRaidLevelCommand.Execute(new DataAccess.Commands.Raid.SetMinRaidLevelRequest(ChatId: message.Chat.Id, RaidLevel: level));
            }


            //this.activateChatCommand.Execute(new DataAccess.Commands.Raid.ActivateChatRequest { ChatId = message.Chat.Id });
            //await bot.SendTextMessageAsync(message.Chat.Id, "Konfiguration abgeschlossen");

            await bot.SendTextMessageAsync(message.Chat.Id, "Warten auf Freigabe");

            await Helper.Operator.SendMessage(bot, $"{message.Chat.Id} wartet auf Freischaltung.").ConfigureAwait(false);

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
