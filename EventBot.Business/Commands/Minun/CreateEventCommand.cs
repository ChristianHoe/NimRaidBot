using EventBot.Business.Helper;
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
    public interface ICreateEventCommand : ICommand
    { }

    public class CreateEventCommand : CreateAbstractEventCommand, ICreateEventCommand
    {
        private readonly ISetTimeModeForManualRaidCommand setTimeModeForManualRaidCommand;
        private readonly ISetNowForManualRaidCommand setNowForManualRaidCommand;
        private readonly ISetRaidLevelForManualRaidCommand setRaidLevelForManualRaidCommand;
        private readonly ISetTitleForManualRaidCommand setTitleForManualRaidCommand;

        public CreateEventCommand
        (
            DataAccess.Commands.IStateUpdateCommand stateUpdateCommand,
            DataAccess.Commands.IStatePopCommand statePopCommand,
            DataAccess.Commands.IStatePushCommand statePushCommand,
            Queries.StatePeakQuery statePeakQuery,

            ISetChatForManualRaidAndInitializeCommand setChatForManualRaidCommand,
            IGetActiveChatsForUser getActiveChatsForUser,
            IGetCurrentManualRaidQuery getCurrentManualRaidQuery,
            IGetCurrentChatSettingsQuery getCurrentChatSettingsQuery,
            IGetGymsByChatQuery getGymsByChatQuery,
            ISetGymForManualRaidCommand setGymForManualRaidCommand,
            ISetTimeModeForManualRaidCommand setTimeModeForManualRaidCommand,
            ISetNowForManualRaidCommand setNowForManualRaidhCommand,
            ISetRaidLevelForManualRaidCommand setRaidLevelForManualRaidCommand,
            ISetTitleForManualRaidCommand setTitleForManualRaidCommand,
            ICreateManuelRaidCommand createManuelRaidCommand,
            IGetSpecialGymsForChatsQuery getSpecialGymsQuery,
            IGetPogoConfigurationQuery getPogoConfigurationQuery,
            IGetActiveGymsByChatQuery getActiveGymsByChatQuery,

            TelegramProxies.NimRaidBot nimRaidBot

            )
            : base(stateUpdateCommand, statePushCommand, statePopCommand, statePeakQuery,
            
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
            this.setTitleForManualRaidCommand = setTitleForManualRaidCommand;

            base.Steps.Add(0, this.Step0);

            base.Steps.Add(5, this.Step5);
            base.Steps.Add(6, this.Step6);

            base.Steps.Add(7, this.Step7);
            base.Steps.Add(8, this.Step8);

            base.Steps.Add(9, this.Step9);
            base.Steps.Add(10, this.Step10);
        }

        public override string HelpText => "Erzeugt einen Event-Poll";
        public override string Key => "/event";
        public override ChatRestrictionType ChatRestriction => ChatRestrictionType.Private;

        protected async Task<StateResult> Step0(Message message, string text, TelegramBotClient bot)
        {
            var chatId = base.GetChatId(message);

            await bot.SendTextMessageAsync(chatId, "Anlegen eines Events").ConfigureAwait(false);

            return await this.Step1(message, text, bot);
        }

        protected override async Task<StateResult> Step4(Message message, string text, TelegramBotClient bot)
        {
            if (!(await base.Step4(message, text, bot)).IsFinished)
                return StateResult.TryAgain;

            return await this.Step5(message, text, bot);
        }

        protected async Task<StateResult> Step5(Message message, string text, TelegramBotClient bot)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            // set raid level = 6 (can not be blocked at the moment)    
            this.setRaidLevelForManualRaidCommand.Execute(new SetRaidLevelForManualRaidRequest { UserId = userId, Level = 6 });

            return await this.Step6(message, text, bot);
        }

        protected async Task<StateResult> Step6(Message message, string text, TelegramBotClient bot)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            // configure begin date
            this.setTimeModeForManualRaidCommand.Execute(new SetTimeModeForManualRaidRequest { UserId = userId, TimeMode = 1 });

            return await this.Step7(message, text, bot);
        }


        protected async Task<StateResult> Step7(Message message, string text, TelegramBotClient bot)
        {
            var chatId = base.GetChatId(message);
            await bot.SendTextMessageAsync(chatId, "Titel (Max 40):").ConfigureAwait(false);

            return StateResult.AwaitUserAt(8);
        }

        protected async Task<StateResult> Step8(Message message, string text, TelegramBotClient bot)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            if (!SkipCurrentStep(text))
            {

                if (text.Length > 40)
                {
                    await bot.SendTextMessageAsync(chatId, "Titel ist zu lang!").ConfigureAwait(false);
                    return StateResult.TryAgain;
                }

                this.setTitleForManualRaidCommand.Execute(new SetTitleForManualRaidRequest { UserId = userId, Title = text });
            }   

            return await this.Step9(message, text, bot);
        }

        protected async Task<StateResult> Step9(Message message, string text, TelegramBotClient bot)
        {
            var chatId = base.GetChatId(message);
            await bot.SendTextMessageAsync(chatId, "Termin (yyyy.mm.dd hh:mm):").ConfigureAwait(false);


            return StateResult.AwaitUserAt(10);
        }

        TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneIds.Local);

        protected async Task<StateResult> Step10(Message message, string text, TelegramBotClient bot)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            if (!SkipCurrentStep(text))
            {
                if (!DateTime.TryParseExact(text, "yyyy/MM/dd HH:mm", new CultureInfo("de-de"), DateTimeStyles.None, out DateTime date))
                {
                    await bot.SendTextMessageAsync(chatId, "Datum konnte nicht erkannt werden, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return StateResult.TryAgain;
                }

                if (date < DateTime.Now)
                {
                    await bot.SendTextMessageAsync(chatId, "Das Startdatum liegt in der Vergangenheit, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return StateResult.TryAgain;
                }
                
                var x = TimeZoneInfo.ConvertTimeToUtc(date, timezone);
                this.setNowForManualRaidCommand.Execute(new SetNowForManualRaidRequest { UserId = userId, Start = x });
            }   

            return await this.Step13(message, text, bot);
        }
    }
}
