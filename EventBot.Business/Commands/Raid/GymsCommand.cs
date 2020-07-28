using EventBot.Business.Interfaces;
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

namespace EventBot.Business.Commands.Raid
{
    public interface IGymsCommand : ICommand
    { }

    public class GymsCommand : StatefulCommand, IGymsCommand
    {
        private readonly IAddSpecialGymCommand addSpecialGymCommand;
        private readonly IDeleteSpecialGymCommand deleteSpecialGymCommand;
        private readonly IGetSpecialGymsForChatsQuery getSpecialGymsQuery;
        private readonly IGetGymsByChatQuery getGymsByChatQuery;
        private readonly IGetCurrentChatSettingsQuery getCurrentChatSettingsQuery;

        public GymsCommand
        (
            DataAccess.Commands.IStateUpdateCommand stateUpdateCommand,
            DataAccess.Commands.IStatePopCommand statePopCommand,
            DataAccess.Commands.IStatePushCommand statePushCommand,
            Queries.StatePeakQuery statePeakQuery,

            IAddSpecialGymCommand addSpecialGymCommand,
            IDeleteSpecialGymCommand deleteSpecialGymCommand,
            IGetSpecialGymsForChatsQuery getSpecialGymsQuery,
            IGetGymsByChatQuery getGymsByChatQuery,
            IGetCurrentChatSettingsQuery getCurrentChatSettingsQuery

            )
            : base(stateUpdateCommand, statePushCommand, statePopCommand, statePeakQuery)
        {
            this.addSpecialGymCommand = addSpecialGymCommand;
            this.deleteSpecialGymCommand = deleteSpecialGymCommand;
            this.getSpecialGymsQuery = getSpecialGymsQuery;
            this.getGymsByChatQuery = getGymsByChatQuery;
            this.getCurrentChatSettingsQuery = getCurrentChatSettingsQuery;

            base.Steps.Add(0, this.Step0);

            base.Steps.Add(1, this.Step1);
        }

        public override string HelpText => "De-/Aktivierung einzelner Gyms";
        public override string Key => "/gyms";
        public override ChatRestrictionType ChatRestriction => ChatRestrictionType.Group;


        protected async Task<bool> Step0(Message message, string text, TelegramBotClient bot)
        {
            var userId = base.GetUserId(message);
            var chatId = base.GetChatId(message);

            var currentChatSettings = this.getCurrentChatSettingsQuery.Execute(new GetCurrentChatSettingsRequest { ChatId = chatId });
            var gyms = this.getGymsByChatQuery.Execute(new GetGymsByChatRequest { Chat = currentChatSettings });
            var special = this.getSpecialGymsQuery.Execute(new GetSpecialGymsForChatsRequest { ChatIds = new[] { chatId } });

            StringBuilder msg = new StringBuilder("Bitte wähle ein Gym\r\n");

            int i = 0;
            foreach (var gym in gyms)
            {
                var line = $"{i} - {(special.Any(x => x.GymId == gym.Id && x.Type == (int)GymType.Exclude) ? "X " : "")}{(special.Any(x => x.GymId == gym.Id && x.Type == (int)GymType.ExRaid) ? "+ " : "")}{gym.Name}";
                if (line.Length + msg.Length > 4096)
                {
                    await bot.SendTextMessageAsync(chatId, msg.ToString());
                    msg.Clear();
                }

                msg.AppendLine(line);
                i++;
            }

            await bot.SendTextMessageAsync(chatId, msg.ToString());

            base.NextState(message, 1);
            return false;
        }

        protected async Task<bool> Step1(Message message, string text, TelegramBotClient bot)
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

                var currentChatSettings = this.getCurrentChatSettingsQuery.Execute(new GetCurrentChatSettingsRequest { ChatId = chatId });
                var gyms = this.getGymsByChatQuery.Execute(new GetGymsByChatRequest { Chat = currentChatSettings });

                if (gymIndex < 0 || gyms.Count() < gymIndex)
                {
                    await bot.SendTextMessageAsync(chatId, "Das Gym konnte nicht erkannt werden, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return false;
                }


                // TODO: if multiple queries
                var specials = this.getSpecialGymsQuery.Execute(new GetSpecialGymsForChatsRequest { ChatIds = new[] { chatId } });
                var gym = gyms.ElementAt(gymIndex);

                if (specials.Any(x => x.GymId == gym.Id))
                {
                    this.deleteSpecialGymCommand.Execute(new DeleteSpecialGymRequest { ChatId = chatId, GymId = gym.Id, Type = GymType.Exclude });
                    await bot.SendTextMessageAsync(chatId, $"{gym.Name} aktiviert.").ConfigureAwait(false);
                }
                else
                {
                    this.addSpecialGymCommand.Execute(new AddSpecialGymRequest { ChatId = chatId, GymId = gym.Id, Type = GymType.Exclude });
                    await bot.SendTextMessageAsync(chatId, $"{gym.Name} deaktiviert.").ConfigureAwait(false);
                }
            }

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
