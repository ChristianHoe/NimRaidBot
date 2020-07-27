using EventBot.Business.Interfaces;
using EventBot.Business.Queries;
using EventBot.DataAccess.Commands.Raid;
using EventBot.DataAccess.ModelsEx;
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
    public interface ICreateRaidCommand : ICommand
    { }

    public class CreateRaidCommand : StatefulCommand, ICreateRaidCommand
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
        private readonly ISetPokeIdForManualRaidCommand setPokeIdForManualRaidCommand;
        private readonly ICreateManuelRaidCommand createManuelRaidCommand;
        private readonly IGetSpecialGymsForChatsQuery getSpecialGymsQuery;
        private readonly IGetPogoConfigurationQuery getPogoConfigurationQuery;
        private readonly TelegramProxies.NimRaidBot nimRaidBot;

        public CreateRaidCommand
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
            ISetPokeIdForManualRaidCommand setPokeIdForManualRaidCommand,
            ICreateManuelRaidCommand createManuelRaidCommand,
            IGetSpecialGymsForChatsQuery getSpecialGymsQuery,
            IGetPogoConfigurationQuery getPogoConfigurationQuery,
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
            this.setPokeIdForManualRaidCommand = setPokeIdForManualRaidCommand;
            this.createManuelRaidCommand = createManuelRaidCommand;
            this.getSpecialGymsQuery = getSpecialGymsQuery;
            this.getPogoConfigurationQuery = getPogoConfigurationQuery;
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

            base.Steps.Add(11, this.Step11);
            base.Steps.Add(12, this.Step12);
        }

        public override string HelpText => "Erzeugt einen Raid-Poll";
        public override string Key => "/raid";
        public override ChatRestrictionType ChatRestriction => ChatRestrictionType.Private;

        protected async Task<bool> Step0(Message message, string text, TelegramBotClient bot, int step)
        {
            var chatId = base.GetChatId(message);

            await bot.SendTextMessageAsync(chatId, "Anlegen eines Raids").ConfigureAwait(false);

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

            return await this.Step9(message, text, bot, step);
        }

        protected async Task<bool> Step9(Message message, string text, TelegramBotClient bot, int step)
        {
            var chatId = base.GetChatId(message);
            await bot.SendTextMessageAsync(chatId, "Raidlevel");


            base.NextState(message, 10);
            return false;
        }

        protected async Task<bool> Step10(Message message, string text, TelegramBotClient bot, int step)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            if (!SkipCurrentStep(text))
            {
                if (!int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int raidLevel))
                {
                    await bot.SendTextMessageAsync(chatId, "Das Raidlevel konnte nicht erkannt werden, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return false;
                }

                if (raidLevel < 0 || 5 < raidLevel)
                {
                    await bot.SendTextMessageAsync(chatId, "Raidlevel liegen außerhalb des gültigen Bereichs (0 - 5), bitte probiere es noch einmal.").ConfigureAwait(false);
                    return false;
                }

                this.setRaidLevelForManualRaidCommand.Execute(new SetRaidLevelForManualRaidRequest { UserId = userId, Level = raidLevel });
            }

            return await this.Step5(message, text, bot, step);
        }

        protected async Task<bool> Step5(Message message, string text, TelegramBotClient bot, int step)
        {
            var chatId = base.GetChatId(message);
            await bot.SendTextMessageAsync(chatId, "Wähle: 1 - Zeit bis zum Start, 2 - Zeit bis zum Ende").ConfigureAwait(false);

            base.NextState(message, 6);
            return false;
        }

        protected async Task<bool> Step6(Message message, string text, TelegramBotClient bot, int step)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            if (!SkipCurrentStep(text))
            {
                if (!int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int timeMode))
                {
                    await bot.SendTextMessageAsync(chatId, "Die Zeitwahl konnte nicht erkannt werden, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return false;
                }

                if (timeMode < 1 || 2 < timeMode)
                {
                    await bot.SendTextMessageAsync(chatId, "Die Zeitwahl konnte nicht erkannt werden, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return false;
                }


                this.setTimeModeForManualRaidCommand.Execute(new SetTimeModeForManualRaidRequest { UserId = userId, TimeMode = timeMode });
            }

            await this.Step7(message, text, bot, step);
            return false;
        }

        protected async Task<bool> Step7(Message message, string text, TelegramBotClient bot, int step)
        {
            var chatId = base.GetChatId(message);
            await bot.SendTextMessageAsync(chatId, "Minuten bis zum Start/Ende").ConfigureAwait(false);


            base.NextState(message, 8);
            return false;
        }

        protected async Task<bool> Step8(Message message, string text, TelegramBotClient bot, int step)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            if (!SkipCurrentStep(text))
            {
                if (!int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int minuten))
                {
                    await bot.SendTextMessageAsync(chatId, "Die Minuten konnte nicht erkannt werden, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return false;
                }

                if (minuten < 0 || 60 < minuten)
                {
                    await bot.SendTextMessageAsync(chatId, "Die Minuten liegen außerhalb des gültigen Bereichs (0 - 60), bitte probiere es noch einmal.").ConfigureAwait(false);
                    return false;
                }

                var current = this.getCurrentManualRaidQuery.Execute(new GetCurrentManualRaidRequest { UserId = userId });

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

                this.setNowForManualRaidCommand.Execute(new SetNowForManualRaidRequest { UserId = userId, Start = start });

                //var current = this.getCurrentManualRaidQuery.Execute(new GetCurrentManualRaidRequest { UserId = userId });
                if (current.TimeMode == 2)
                    return await this.Step11(message, text, bot, step);
            }   

            return await this.Step13(message, text, bot, step);
        }

        protected async Task<bool> Step11(Message message, string text, TelegramBotClient bot, int step)
        {
            var chatId = base.GetChatId(message);
            await bot.SendTextMessageAsync(chatId, "Poke-Id:");


            base.NextState(message, 12);
            return false;
        }

        protected async Task<bool> Step12(Message message, string text, TelegramBotClient bot, int step)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            if (!SkipCurrentStep(text))
            {
                if (!int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int pokeId))
                {
                    await bot.SendTextMessageAsync(chatId, "Die Poke-Id konnte nicht erkannt werden, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return false;
                }

                if (pokeId < 0 || 2000 < pokeId)
                {
                    await bot.SendTextMessageAsync(chatId, "Keine gültige Poke-Id, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return false;
                }

                this.setPokeIdForManualRaidCommand.Execute(new SetPokeIdForManualRaidRequest { UserId = userId, PokeId = pokeId });
            }

            return await this.Step13(message, text, bot, step);
        }

        protected async Task<bool> Step13(Message message, string text, TelegramBotClient bot, int step)        
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            var pogoConfiguration = this.getPogoConfigurationQuery.Execute(new GetPogoConfigurationRequest());

            this.createManuelRaidCommand.Execute(new CreateManuelRaidRequest { UserId = userId, DurationInMinutes = pogoConfiguration.RaidDurationInMin });

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
