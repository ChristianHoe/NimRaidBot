using EventBot.Business.Interfaces;
using EventBot.DataAccess.Commands.Raid;
using EventBot.DataAccess.Queries.Raid;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.Minun
{
    public interface INutzerCommand : ICommand
    { }

    public class NutzerCommand : StatefulCommand, INutzerCommand
    {
        private readonly IGetCurrentUserSettingsQuery getCurrentUserSettingsQuery;
        private readonly IAddUserCommand addUserCommand;
        private readonly ISetUserLevelCommand setUserLevelCommand;
        private readonly ISetUserNameCommand setUserNameCommand;
        private readonly ISetUserTeamCommand setUserTeamCommand;

        public NutzerCommand(
            DataAccess.Commands.IStateUpdateCommand stateUpdateCommand,
            DataAccess.Commands.IStatePopCommand statePopCommand,
            DataAccess.Commands.IStatePushCommand statePushCommand,
            Queries.StatePeakQuery statePeakQuery,

            IAddUserCommand addUserCommand,
            IGetCurrentUserSettingsQuery getCurrentUserSettingsQuery,
            ISetUserLevelCommand setUserLevelCommand,
            ISetUserNameCommand setUserNameCommand,
            ISetUserTeamCommand setUserTeamCommand

            )
            : base(stateUpdateCommand, statePushCommand, statePopCommand, statePeakQuery)
        {
            this.addUserCommand = addUserCommand;
            this.getCurrentUserSettingsQuery = getCurrentUserSettingsQuery;
            this.setUserLevelCommand = setUserLevelCommand;
            this.setUserNameCommand = setUserNameCommand;
            this.setUserTeamCommand = setUserTeamCommand;


            base.Steps.Add(0, this.Step0);

            base.Steps.Add(1, this.Step1);
            base.Steps.Add(2, this.Step2);


            base.Steps.Add(3, this.Step3);
            base.Steps.Add(4, this.Step4);


            base.Steps.Add(5, this.Step5);
            base.Steps.Add(6, this.Step6);

        }

        public override string Key => "/nutzer";
        public override string HelpText => "Startet Nutzereinrichtung";

        protected async Task<StateResult> Step0(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var chatId = base.GetChatId(message);

            await bot.SendTextMessageAsync(chatId, "Willkommen bei der Einrichtung des Bots").ConfigureAwait(false);
            await bot.SendTextMessageAsync(chatId, "Du kannst 'x' antworten, dann wird der ursprüngliche Wert beibehalten.").ConfigureAwait(false);

            return await this.Step1(message, text, bot, batchMode);
        }

        protected async Task<StateResult> Step1(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            var currentSettings = this.getCurrentUserSettingsQuery.Execute(new DataAccess.Queries.Raid.GetCurrentUserSettingsRequest(UserId: userId));

            var name = currentSettings?.IngameName ?? "";

            await bot.SendTextMessageAsync(chatId, $"Trainername: {name}");
            return StateResult.AwaitUserAt(2);
        }

        protected async Task<StateResult> Step2(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            if (!SkipCurrentStep(text))
            {
                var userId = base.GetUserId(message);

                this.setUserNameCommand.Execute(new DataAccess.Commands.Raid.SetUserNameRequest(UserId: userId, Name: text));
            }

            return await this.Step3(message, text, bot, batchMode);
        }

        protected async Task<StateResult> Step3(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            var currentSettings = this.getCurrentUserSettingsQuery.Execute(new DataAccess.Queries.Raid.GetCurrentUserSettingsRequest(UserId: userId));
            var team = currentSettings.Team?.ToString() ?? "";

            await bot.SendTextMessageAsync(chatId, $"Team: {team} \n\r(1-Blau, 2-Rot, 3-Gelb)");

            return StateResult.AwaitUserAt(4);
        }

        protected async Task<StateResult> Step4(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            if (!SkipCurrentStep(text))
            {
                if (!int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int team))
                {
                    await bot.SendTextMessageAsync(chatId, "Das Team konnte nicht erkannt werden, bitte probiere es noch einmal.");
                    return StateResult.TryAgain;
                }

                if (team < 1 || 3 < team)
                {
                    await bot.SendTextMessageAsync(chatId, "Das Team konnte nicht erkannt werden, bitte probiere es noch einmal.");
                    return StateResult.TryAgain;
                }

                this.setUserTeamCommand.Execute(new DataAccess.Commands.Raid.SetUserTeamRequest(UserId :userId, Team: (TeamType)team));
            }

            return await this.Step5(message, text, bot, batchMode);
        }

        protected async Task<StateResult> Step5(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            var currentSettings = this.getCurrentUserSettingsQuery.Execute(new DataAccess.Queries.Raid.GetCurrentUserSettingsRequest(UserId: userId));

            var level = currentSettings.Level?.ToString() ?? "";

            await bot.SendTextMessageAsync(chatId, $"Level: {level}");
            return StateResult.AwaitUserAt(6);
        }

        protected async Task<StateResult> Step6(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            if (!SkipCurrentStep(text))
            {
                if (!int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int level))
                {
                    await bot.SendTextMessageAsync(chatId, "Das Level konnte nicht erkannt werden, bitte probiere es noch einmal.");
                    return StateResult.TryAgain;
                }

                if (level < 1 || 50 < level)
                {
                    await bot.SendTextMessageAsync(chatId, "Das Level konnte nicht erkannt werden, bitte probiere es noch einmal.");
                    return StateResult.TryAgain;
                }

                this.setUserLevelCommand.Execute(new DataAccess.Commands.Raid.SetUserLevelRequest(UserId: userId, Level: level));
            }

            await bot.SendTextMessageAsync(chatId, "Konfiguration abgeschlossen");

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
