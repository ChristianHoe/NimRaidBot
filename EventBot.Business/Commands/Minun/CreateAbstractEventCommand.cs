using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventBot.Business.Interfaces;
using EventBot.Business.Queries;
using EventBot.DataAccess.Commands.Raid;
using EventBot.DataAccess.ModelsEx;
using EventBot.DataAccess.Queries.Raid;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.Minun
{
    public abstract class CreateAbstractEventCommand : StatefulCommand
    {
        private readonly IGetActiveChatsForUser getActiveChatsForUser;
        private readonly ISetChatForManualRaidAndInitializeCommand setChatForManualRaidCommand;
        protected readonly IGetCurrentManualRaidQuery getCurrentManualRaidQuery;
        private readonly IGetActiveGymsByChatQuery getActiveGymsByChatQuery;
        private readonly IGetSpecialGymsForChatsQuery getSpecialGymsQuery;
        private readonly ISetGymForManualRaidCommand setGymForManualRaidCommand;
        private readonly ICreateManuelRaidCommand createManuelRaidCommand;
        protected readonly IGetPogoConfigurationQuery getPogoConfigurationQuery;
        private readonly TelegramProxies.NimRaidBot nimRaidBot;

        public CreateAbstractEventCommand(
            DataAccess.Commands.IStateUpdateCommand stateUpdateCommand,
            DataAccess.Commands.IStatePushCommand statePushCommand,
            DataAccess.Commands.IStatePopCommand statePopCommand,
            Queries.StatePeakQuery statePeakQuery,

            IGetActiveChatsForUser getActiveChatsForUser,
            ISetChatForManualRaidAndInitializeCommand setChatForManualRaidCommand,
            IGetCurrentManualRaidQuery getCurrentManualRaidQuery,
            IGetActiveGymsByChatQuery getActiveGymsByChatQuery,
            IGetSpecialGymsForChatsQuery getSpecialGymsQuery,
            ISetGymForManualRaidCommand setGymForManualRaidCommand,
            ICreateManuelRaidCommand createManuelRaidCommand,
            IGetPogoConfigurationQuery getPogoConfigurationQuery,

            TelegramProxies.NimRaidBot nimRaidBot

        ) : base(stateUpdateCommand, statePushCommand, statePopCommand, statePeakQuery)
        {
            this.getActiveChatsForUser = getActiveChatsForUser;
            this.setChatForManualRaidCommand = setChatForManualRaidCommand;
            this.getCurrentManualRaidQuery = getCurrentManualRaidQuery;
            this.getActiveGymsByChatQuery = getActiveGymsByChatQuery;
            this.getSpecialGymsQuery = getSpecialGymsQuery;
            this.setGymForManualRaidCommand = setGymForManualRaidCommand;
            this.createManuelRaidCommand = createManuelRaidCommand;
            this.getPogoConfigurationQuery = getPogoConfigurationQuery;
            this.nimRaidBot = nimRaidBot;

            base.Steps.Add(1, this.Step1);
            base.Steps.Add(2, this.Step2);

            base.Steps.Add(3, this.Step3);
            base.Steps.Add(4, this.Step4);

            base.Steps.Add(13, this.Step13);
        }

        protected async Task<StateResult> Step1(Message message, string text, TelegramBotClient bot)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            var chats = this.getActiveChatsForUser.Execute(new GetActiveChatsForUserRequest { UserId = userId, BotId = nimRaidBot.BotId });

            if (chats.Count() == 0)
            {
                await bot.SendTextMessageAsync(chatId, $"Für den Befehl musst du mindestens in einer Gruppe Mitglied sein.").ConfigureAwait(false);
                return StateResult.Finished;
            }

            StringBuilder msg = new StringBuilder();
            for (int i = 0; i < chats.Count(); i++)
            {
                msg.AppendLine($"{i} - {chats[i].Name}");
            }

            if (chats.Count() > 1)
            {
                await bot.SendTextMessageAsync(chatId, $"Bitte wähle eine Gruppe\n\r{msg.ToString()}").ConfigureAwait(false);
                return StateResult.AwaitUserAt(2);
            }
            else
            {
                await bot.SendTextMessageAsync(chatId, $"Folgende Gruppe wird verwendet\n\r{msg.ToString()}").ConfigureAwait(false);
                setChatForManualRaidCommand.Execute(new SetChatForManualRaidAndInitializeRequest { UserId = userId, ChatId = chats.First().ChatId });
                return StateResult.AwaitUserAt(3);
            }
        }

        protected async Task<StateResult> Step2(Message message, string text, TelegramBotClient bot)
        {
            if (!SkipCurrentStep(text))
            {
                var userId = base.GetUserId(message);
                var chatId = base.GetChatId(message);

                if (!int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int chat))
                {
                    await bot.SendTextMessageAsync(chatId, "Es wurde keine Zahl ausgewählt, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return StateResult.TryAgain;
                }

                var activeChatIds = this.getActiveChatsForUser.Execute(new GetActiveChatsForUserRequest { UserId = userId, BotId = nimRaidBot.BotId });
                if (chat < 0 || activeChatIds.Count() <= chat)
                {
                    await bot.SendTextMessageAsync(chatId, "Der ausgewählt Wert liegt nicht im Wertebereich, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return StateResult.TryAgain;
                }

                this.setChatForManualRaidCommand.Execute(new SetChatForManualRaidAndInitializeRequest { UserId = userId, ChatId = activeChatIds.ElementAt(chat).ChatId });
            }

            return await this.Step3(message, text, bot);
        }

        protected async Task<StateResult> Step3(Message message, string text, TelegramBotClient bot)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            var raid = this.getCurrentManualRaidQuery.Execute(new GetCurrentManualRaidRequest { UserId = userId } );
            var gyms = this.getActiveGymsByChatQuery.Execute(new GetActiveGymsByChatRequest { ChatId = raid.ChatId ?? 0 });
            var special = this.getSpecialGymsQuery.Execute(new GetSpecialGymsForChatsRequest { ChatIds = new[] { raid.ChatId ?? 0 } });

            StringBuilder msg = new StringBuilder();

            int i = 0;
            foreach (var gym in gyms)
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

            return StateResult.AwaitUserAt(4);
        }

        protected virtual async Task<StateResult> Step4(Message message, string text, TelegramBotClient bot)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            if (!SkipCurrentStep(text))
            {
                var raid = this.getCurrentManualRaidQuery.Execute(new GetCurrentManualRaidRequest { UserId = userId });
                var gyms = this.getActiveGymsByChatQuery.Execute(new GetActiveGymsByChatRequest { ChatId = raid.ChatId ?? 0 });

                var gymId = 0;

                if (!int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int gymIndex))
                {
                    var names = gyms.Where(x => x.Name?.Contains(text, StringComparison.InvariantCultureIgnoreCase) ?? false);
                    if (names.Count() == 0)
                    {
                        await bot.SendTextMessageAsync(chatId, "Das Gym konnte nicht erkannt werden, bitte probiere es noch einmal.").ConfigureAwait(false);
                        return StateResult.TryAgain;
                    }

                    if (names.Count() > 1)
                    {
                        var msg = "Gym nicht eindeutig!" + Environment.NewLine + string.Join(Environment.NewLine, names.Select(x => $"Name: {x.Name}"));
                        await bot.SendTextMessageAsync(chatId, msg).ConfigureAwait(false);
                        return StateResult.TryAgain;
                    }

                    gymId = names.First().Id;
                }
                else
                {
                    if (gymIndex < 0 || gyms.Count() < gymIndex)
                    {
                        await bot.SendTextMessageAsync(chatId, "Das Gym konnte nicht erkannt werden, bitte probiere es noch einmal.").ConfigureAwait(false);
                        return StateResult.TryAgain;
                    }
                    
                    gymId = gyms.ElementAt(gymIndex).Id;
                }


                this.setGymForManualRaidCommand.Execute(new SetGymForManualRaidRequest { UserId = userId, GymId = gymId });
            }

            return StateResult.Finished;
        }

        protected async Task<StateResult> Step13(Message message, string text, TelegramBotClient bot)        
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            var pogoConfiguration = this.getPogoConfigurationQuery.Execute(new GetPogoConfigurationRequest());

            this.createManuelRaidCommand.Execute(new CreateManuelRaidRequest { UserId = userId, DurationInMinutes = pogoConfiguration.RaidDurationInMin });

            var current = this.getCurrentManualRaidQuery.Execute(new GetCurrentManualRaidRequest { UserId = userId });

            await bot.SendTextMessageAsync(chatId, $"Fertig. Aktualisierung per Befehl: /update_{current.RaidId}").ConfigureAwait(false);

            return StateResult.Finished;
        }

        protected bool SkipCurrentStep(string text)
        {
            // geht eigentlich nicht
            if (string.IsNullOrWhiteSpace(text))
                return true;

            return text.Trim().ToLowerInvariant().Equals("x");
        }
    }
}