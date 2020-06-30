using EventBot.Business.Helper;
using EventBot.Business.Interfaces;
using EventBot.DataAccess.Commands.Raid;
using EventBot.DataAccess.ModelsEx;
using EventBot.DataAccess.Queries.Raid;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.Minun
{
    public interface ICreateEventCommand : ICommand
    { }

    public class CreateEventCommand : StatefulCommand, ICreateEventCommand
    {
        private readonly IGetActiveChatsForUser getActiveChatsForUser;
        private readonly ISetChatForManualRaidAndInitializeCommand setChatForManualRaidCommand;
        private readonly IGetCurrentManualRaidQuery getCurrentManualRaidQuery;
        private readonly IGetCurrentChatSettingsQuery getCurrentChatSettingsQuery;
        private readonly IGetGymsByChatQuery getGymsByChatQuery;
        private readonly ISetGymForManualRaidCommand setGymForManualRaidCommand;
        private readonly ISetTimeModeForManualRaidCommand setTimeModeForManualRaidCommand;
        private readonly ISetNowForManualRaidCommand setNowForManualRaidCommand;
        private readonly ISetRaidLevelForManualRaidCommand setRaidLevelForManualRaidCommand;
        private readonly ISetTitleForManualRaidCommand setTitleForManualRaidCommand;
        private readonly ICreateManuelRaidCommand createManuelRaidCommand;
        private readonly IGetSpecialGymsForChatsQuery getSpecialGymsQuery;
        private readonly TelegramProxies.NimRaidBot nimRaidBot;

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
            TelegramProxies.NimRaidBot nimRaidBot

            )
            : base(stateUpdateCommand, statePushCommand, statePopCommand, statePeakQuery)
        {
            this.setChatForManualRaidCommand = setChatForManualRaidCommand;
            this.getActiveChatsForUser = getActiveChatsForUser;
            this.getCurrentManualRaidQuery = getCurrentManualRaidQuery;
            this.getCurrentChatSettingsQuery = getCurrentChatSettingsQuery;
            this.getGymsByChatQuery = getGymsByChatQuery;
            this.setGymForManualRaidCommand = setGymForManualRaidCommand;
            this.setTimeModeForManualRaidCommand = setTimeModeForManualRaidCommand;
            this.setNowForManualRaidCommand = setNowForManualRaidhCommand;
            this.setRaidLevelForManualRaidCommand = setRaidLevelForManualRaidCommand;
            this.setTitleForManualRaidCommand = setTitleForManualRaidCommand;
            this.createManuelRaidCommand = createManuelRaidCommand;
            this.getSpecialGymsQuery = getSpecialGymsQuery;

            this.nimRaidBot = nimRaidBot;

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

            base.Steps.Add(13, this.Step13);
        }

        public override string HelpText => "Erzeugt einen Event-Poll";
        public override string Key => "/event";
        public override ChatRestrictionType ChatRestriction => ChatRestrictionType.Private;

        protected async Task<bool> Step0(Message message, string text, TelegramBotClient bot, int step)
        {
            var chatId = base.GetChatId(message);

            await bot.SendTextMessageAsync(chatId, "Anlegen eines Events").ConfigureAwait(false);

            return await this.Step1(message, text, bot, step);
        }

        protected async Task<bool> Step1(Message message, string text, TelegramBotClient bot, int step)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            var chats = this.getActiveChatsForUser.Execute(new GetActiveChatsForUserRequest { UserId = userId, BotId = nimRaidBot.BotId });

            if (chats.Count() == 0)
            {
                await bot.SendTextMessageAsync(chatId, $"Für den Befehl musst du mindestens in einer Gruppe Mitglied sein.").ConfigureAwait(false);
                return true;
            }

            StringBuilder msg = new StringBuilder();
            for (int i = 0; i < chats.Count(); i++)
            {
                msg.AppendLine($"{i} - {chats[i].Name}");
            }

            if (chats.Count() > 1)
            {
                await bot.SendTextMessageAsync(chatId, $"Bitte wähle eine Gruppe\n\r{msg.ToString()}").ConfigureAwait(false);
                base.NextState(message, 2);
            }
            else
            {
                await bot.SendTextMessageAsync(chatId, $"Folgende Gruppe wird verwendet\n\r{msg.ToString()}").ConfigureAwait(false);
                setChatForManualRaidCommand.Execute(new SetChatForManualRaidAndInitializeRequest { UserId = userId, ChatId = chats.First().ChatId });
                base.NextState(message, 3);
            }

            return false;
        }

        protected async Task<bool> Step2(Message message, string text, TelegramBotClient bot, int step)
        {
            if (!SkipCurrentStep(text))
            {
                var userId = base.GetUserId(message);
                var chatId = base.GetChatId(message);

                if (!int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int chat))
                {
                    await bot.SendTextMessageAsync(chatId, "Es wurde keine Zahl ausgewählt, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return false;
                }

                var activeChatIds = this.getActiveChatsForUser.Execute(new GetActiveChatsForUserRequest { UserId = userId, BotId = nimRaidBot.BotId });
                if (chat < 0 || activeChatIds.Count() <= chat)
                {
                    await bot.SendTextMessageAsync(chatId, "Der ausgewählt Wert liegt nicht im Wertebereich, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return false;
                }

                this.setChatForManualRaidCommand.Execute(new SetChatForManualRaidAndInitializeRequest { UserId = userId, ChatId = activeChatIds.ElementAt(chat).ChatId });
            }

            return await this.Step3(message, text, bot, step);
        }

        protected async Task<bool> Step3(Message message, string text, TelegramBotClient bot, int step)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            var raid = this.getCurrentManualRaidQuery.Execute(new GetCurrentManualRaidRequest { UserId = userId } );
            var currentChatSettings = this.getCurrentChatSettingsQuery.Execute(new GetCurrentChatSettingsRequest { ChatId = raid.ChatId ?? 0 });
            var gyms = this.getGymsByChatQuery.Execute(new GetGymsByChatRequest { Chat = currentChatSettings });
            var special = this.getSpecialGymsQuery.Execute(new GetSpecialGymsForChatsRequest { ChatIds = new[] { raid.ChatId ?? 0 } });

            StringBuilder msg = new StringBuilder();

            int i = 0;
            foreach (var gym in gyms.Where(x => !special.Any(y => y.GymId == x.Id && y.Type == (int)GymType.Exclude)))
            {
                var gymName = special.FirstOrDefault(x => x.GymId == gym.Id && x.Type == (int)GymType.AlternativeName)?.Data ?? gym.Name;
                var line = $"{i} - {gymName}";
                if (line.Length + msg.Length > 4096)
                {
                    await bot.SendTextMessageAsync(chatId, msg.ToString());
                    msg.Clear();
                }

                msg.AppendLine(line);
                i++;
            }

            await bot.SendTextMessageAsync(chatId, msg.ToString());

            base.NextState(message, 4);
            return false;
        }

        protected async Task<bool> Step4(Message message, string text, TelegramBotClient bot, int step)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            if (!SkipCurrentStep(text))
            {
                if (!int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int gymIndex))
                {
                    await bot.SendTextMessageAsync(chatId, "Das Gym konnte nicht erkannt werden, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return false;
                }

                var raid = this.getCurrentManualRaidQuery.Execute(new GetCurrentManualRaidRequest { UserId = userId });
                var currentChatSettings = this.getCurrentChatSettingsQuery.Execute(new GetCurrentChatSettingsRequest { ChatId = raid.ChatId ?? 0 });
                var gyms = this.getGymsByChatQuery.Execute(new GetGymsByChatRequest { Chat = currentChatSettings });
                var special = this.getSpecialGymsQuery.Execute(new GetSpecialGymsForChatsRequest { ChatIds = new[] { raid.ChatId ?? 0 }, Type = GymType.Exclude });

                var filteredGyms = gyms.Where(x => !special.Any(y => y.GymId == x.Id && y.Type == (int)GymType.Exclude));

                if (gymIndex < 0 || filteredGyms.Count() < gymIndex)
                {
                    await bot.SendTextMessageAsync(chatId, "Das Gym konnte nicht erkannt werden, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return false;
                }

                this.setGymForManualRaidCommand.Execute(new SetGymForManualRaidRequest { UserId = userId, GymId = filteredGyms.ElementAt(gymIndex).Id });
            }

            return await this.Step5(message, text, bot, step);
        }

        protected async Task<bool> Step5(Message message, string text, TelegramBotClient bot, int step)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            // set raid level = 6 (can not be blocked at the moment)    
            this.setRaidLevelForManualRaidCommand.Execute(new SetRaidLevelForManualRaidRequest { UserId = userId, Level = 6 });

            return await this.Step6(message, text, bot, step);
        }

        protected async Task<bool> Step6(Message message, string text, TelegramBotClient bot, int step)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            // configure begin date
            this.setTimeModeForManualRaidCommand.Execute(new SetTimeModeForManualRaidRequest { UserId = userId, TimeMode = 1 });

            await this.Step7(message, text, bot, step);
            return false;
        }


        protected async Task<bool> Step7(Message message, string text, TelegramBotClient bot, int step)
        {
            var chatId = base.GetChatId(message);
            await bot.SendTextMessageAsync(chatId, "Titel (Max 40):").ConfigureAwait(false);


            base.NextState(message, 8);
            return false;
        }

        protected async Task<bool> Step8(Message message, string text, TelegramBotClient bot, int step)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            if (!SkipCurrentStep(text))
            {

                if (text.Length > 40)
                {
                    await bot.SendTextMessageAsync(chatId, "Titel ist zu lang!").ConfigureAwait(false);
                    return false;
                }

                this.setTitleForManualRaidCommand.Execute(new SetTitleForManualRaidRequest { UserId = userId, Title = text });
            }   

            return await this.Step9(message, text, bot, step);
        }

        protected async Task<bool> Step9(Message message, string text, TelegramBotClient bot, int step)
        {
            var chatId = base.GetChatId(message);
            await bot.SendTextMessageAsync(chatId, "Termin (yyyy.mm.dd hh:mm):").ConfigureAwait(false);


            base.NextState(message, 10);
            return false;
        }

        TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneIds.Local);

        protected async Task<bool> Step10(Message message, string text, TelegramBotClient bot, int step)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            if (!SkipCurrentStep(text))
            {
                if (!DateTime.TryParseExact(text, "yyyy/MM/dd HH:mm", new CultureInfo("de-de"), DateTimeStyles.None, out DateTime date))
                {
                    await bot.SendTextMessageAsync(chatId, "Datum konnte nicht erkannt werden, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return false;
                }

                if (date < DateTime.Now)
                {
                    await bot.SendTextMessageAsync(chatId, "Das Startdatum liegt in der Vergangenheit, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return false;
                }
                
                var x = TimeZoneInfo.ConvertTimeToUtc(date, timezone);
                this.setNowForManualRaidCommand.Execute(new SetNowForManualRaidRequest { UserId = userId, Start = x });
            }   

            return await this.Step13(message, text, bot, step);
        }

        protected async Task<bool> Step13(Message message, string text, TelegramBotClient bot, int step)        
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            this.createManuelRaidCommand.Execute(new CreateManuelRaidRequest { UserId = userId });

            var current = this.getCurrentManualRaidQuery.Execute(new GetCurrentManualRaidRequest { UserId = userId });

            await bot.SendTextMessageAsync(chatId, $"Fertig. Aktualisierung per Befehl: /update_{current.RaidId}").ConfigureAwait(false);

            return true;
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
