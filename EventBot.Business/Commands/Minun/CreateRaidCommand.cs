using EventBot.Business.Interfaces;
using EventBot.Business.Queries;
using EventBot.DataAccess.Commands.Raid;
using EventBot.DataAccess.Queries.Raid;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.Minun
{
    public interface ICreateRaidCommand : ICommand
    { }

    public class CreateRaidCommand : CreateAbstractEventCommand, ICreateRaidCommand
    {
        private readonly ISetTimeModeForManualRaidCommand setTimeModeForManualRaidCommand;
        private readonly ISetNowForManualRaidCommand setNowForManualRaidCommand;
        private readonly ISetRaidLevelForManualRaidCommand setRaidLevelForManualRaidCommand;
        private readonly ISetPokeIdForManualRaidCommand setPokeIdForManualRaidCommand;

        public CreateRaidCommand
        (
            DataAccess.Commands.IStateUpdateCommand stateUpdateCommand,
            DataAccess.Commands.IStatePopCommand statePopCommand,
            DataAccess.Commands.IStatePushCommand statePushCommand,
            Queries.StatePeakQuery statePeakQuery,

            ISetChatForManualRaidAndInitializeCommand setChatForManualRaidCommand,
            IGetActiveChatsForUser getActiveChatsForUser,
            IGetCurrentManualRaidQuery getCurrentManualRaidQuery,
            ISetGymForManualRaidCommand setGymForManualRaidCommand,
            ISetTimeModeForManualRaidCommand setTimeModeForManualRaidCommand,
            ISetNowForManualRaidCommand setNowForManualRaidhCommand,
            ISetRaidLevelForManualRaidCommand setRaidLevelForManualRaidCommand,
            ISetPokeIdForManualRaidCommand setPokeIdForManualRaidCommand,
            ICreateManuelRaidCommand createManuelRaidCommand,
            IGetSpecialGymsForChatsQuery getSpecialGymsQuery,
            IGetPogoConfigurationQuery getPogoConfigurationQuery,
            IGetActiveGymsByChatQuery getActiveGymsByChatQuery,

            TelegramProxies.NimRaidBot nimRaidBot

            )
            : base(
                stateUpdateCommand, statePushCommand, statePopCommand, statePeakQuery, 
            
                getActiveChatsForUser,
                setChatForManualRaidCommand,
                getCurrentManualRaidQuery,
                getActiveGymsByChatQuery,
                getSpecialGymsQuery,
                setGymForManualRaidCommand,
                createManuelRaidCommand,
                getPogoConfigurationQuery,

                nimRaidBot
            )
        {
            this.setTimeModeForManualRaidCommand = setTimeModeForManualRaidCommand;
            this.setNowForManualRaidCommand = setNowForManualRaidhCommand;
            this.setRaidLevelForManualRaidCommand = setRaidLevelForManualRaidCommand;
            this.setPokeIdForManualRaidCommand = setPokeIdForManualRaidCommand;

            base.Steps.Add(0, this.Step0);

            base.Steps.Add(5, this.Step5);
            base.Steps.Add(6, this.Step6);

            base.Steps.Add(7, this.Step7);
            base.Steps.Add(8, this.Step8);

            base.Steps.Add(9, this.Step9);
            base.Steps.Add(10, this.Step10);

            base.Steps.Add(11, this.Step11);
            base.Steps.Add(12, this.Step12);
        }

        public override string HelpText => "Erzeugt einen Raid-Poll";
        public override string Key => "/raid";
        public override ChatRestrictionType ChatRestriction => ChatRestrictionType.Private;

        protected async Task<StateResult> Step0(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var chatId = base.GetChatId(message);

            await bot.SendTextMessageAsync(chatId, "Anlegen eines Raids").ConfigureAwait(false);

            return await this.Step1(message, text, bot, batchMode);
        }

        protected override async Task<StateResult> Step4(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            if (!(await base.Step4(message, text, bot, batchMode)).IsFinished)
                return StateResult.TryAgain;

            return await this.Step9(message, text, bot, batchMode);
        }

        protected async Task<StateResult> Step9(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            if (!batchMode)
            {
                var chatId = base.GetChatId(message);
                await bot.SendTextMessageAsync(chatId, "Raidlevel");
            }

            return StateResult.AwaitUserAt(10);
        }

        protected async Task<StateResult> Step10(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            if (!SkipCurrentStep(text))
            {
                if (!int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int raidLevel))
                {
                    await bot.SendTextMessageAsync(chatId, "Das Raidlevel konnte nicht erkannt werden, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return StateResult.TryAgain;
                }

                if (raidLevel < 0 || 6 < raidLevel)
                {
                    await bot.SendTextMessageAsync(chatId, "Raidlevel liegen außerhalb des gültigen Bereichs (0 - 5 + 6 für Mega), bitte probiere es noch einmal.").ConfigureAwait(false);
                    return StateResult.TryAgain;
                }

                this.setRaidLevelForManualRaidCommand.Execute(new SetRaidLevelForManualRaidRequest(UserId: userId, Level: raidLevel));
            }

            return await this.Step5(message, text, bot, batchMode);
        }

        protected async Task<StateResult> Step5(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            if (!batchMode)
            {
            var chatId = base.GetChatId(message);
            await bot.SendTextMessageAsync(chatId, "Wähle: 1 - Zeit bis zum Start, 2 - Zeit bis zum Ende").ConfigureAwait(false);
            }

            return StateResult.AwaitUserAt(6);
        }

        protected async Task<StateResult> Step6(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            if (!SkipCurrentStep(text))
            {
                if (!int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int timeMode))
                {
                    await bot.SendTextMessageAsync(chatId, "Die Zeitwahl konnte nicht erkannt werden, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return StateResult.TryAgain;
                }

                if (timeMode < 1 || 2 < timeMode)
                {
                    await bot.SendTextMessageAsync(chatId, "Die Zeitwahl konnte nicht erkannt werden, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return StateResult.TryAgain;
                }


                this.setTimeModeForManualRaidCommand.Execute(new SetTimeModeForManualRaidRequest(UserId: userId, TimeMode: timeMode));
            }

            return await this.Step7(message, text, bot, batchMode);
        }

        protected async Task<StateResult> Step7(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            if (!batchMode)
            {
                var chatId = base.GetChatId(message);
                await bot.SendTextMessageAsync(chatId, "Minuten bis zum Start/Ende").ConfigureAwait(false);
            }

            return StateResult.AwaitUserAt(8);
        }

        protected async Task<StateResult> Step8(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            if (!SkipCurrentStep(text))
            {
                if (!int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int minuten))
                {
                    await bot.SendTextMessageAsync(chatId, "Die Minuten konnte nicht erkannt werden, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return StateResult.TryAgain;
                }

                if (minuten < 0 || 60 < minuten)
                {
                    await bot.SendTextMessageAsync(chatId, "Die Minuten liegen außerhalb des gültigen Bereichs (0 - 60), bitte probiere es noch einmal.").ConfigureAwait(false);
                    return StateResult.TryAgain;
                }

                var current = this.getCurrentManualRaidQuery.Execute(new GetCurrentManualRaidRequest(UserId: userId));
                if (current == null)
                {
                    await bot.SendTextMessageAsync(chatId, "Raid nicht in der Datenbank gefunden. Bitte informiere den Administrator.").ConfigureAwait(false);
                    return StateResult.TryAgain;
                }


                var start = DateTime.UtcNow;
                if (current.TimeMode == 1)
                {
                    start = start.AddMinutes(minuten);
                }
                else
                {
                    var pogoConfigurations = this.getPogoConfigurationQuery.Execute(new GetPogoConfigurationRequest());
                    start = start.AddMinutes(minuten - pogoConfigurations.RaidDurationInMin);
                }

                this.setNowForManualRaidCommand.Execute(new SetNowForManualRaidRequest(UserId: userId, Start: start));

                if (current.TimeMode == 2)
                    return await this.Step11(message, text, bot, batchMode);
            }   

            return await this.Step13(message, text, bot, batchMode);
        }

        protected async Task<StateResult> Step11(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            if (!batchMode)
            {
                var chatId = base.GetChatId(message);
                await bot.SendTextMessageAsync(chatId, "Poke-Id:");
            }

            return StateResult.AwaitUserAt(12);
        }

        protected async Task<StateResult> Step12(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            if (!SkipCurrentStep(text))
            {
                var input = text.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var poke = input.Length >= 1 ? input[0] : string.Empty;
                var form = input.Length >= 2 ? input[1][0] : (char?)null;

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

                this.setPokeIdForManualRaidCommand.Execute(new SetPokeIdForManualRaidRequest(UserId: userId, PokeId: pokeId, PokeForm: form));
            }

            return await this.Step13(message, text, bot, batchMode);
        }
    }
}
