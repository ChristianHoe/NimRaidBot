using EventBot.Business.Interfaces;
using EventBot.Business.Queries;
using EventBot.Business.TelegramProxies;
using EventBot.DataAccess.Commands;
using EventBot.DataAccess.Commands.Location;
using EventBot.DataAccess.Models;
using EventBot.DataAccess.Queries.Location;
using EventBot.DataAccess.Queries.Raid;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.Minun
{
    public interface IGymsCommand : ICommand
    { }

    public class GymsCommand : StatefulCommand, IGymsCommand
    {
        private readonly TelegramProxies.NimRaidBot nimRaidBot;
        private readonly IGetActiveChatsForUser getActiveChatsForUser;
        private readonly IGetNotifyLocationsByChatIdQuery getNotifyLocationsByChatIdQuery;
        private readonly IGetActiveGymsForChatQuery getActiveGymsForChatQuery;
        private readonly IRemoveNotifyLocationCommand removeNotifyLocationCommand;
        private readonly IAddNotifyLocationCommand addNotifyLocationCommand;

        public GymsCommand(
            TelegramProxies.NimRaidBot nimRaidBot,
            IGetActiveChatsForUser getActiveChatsForUser,
            IGetNotifyLocationsByChatIdQuery getNotifyLocationsByChatIdQuery,
            IGetActiveGymsForChatQuery getActiveGymsForChatQuery,
            IRemoveNotifyLocationCommand removeNotifyLocationCommand,
            IAddNotifyLocationCommand addNotifyLocationCommand,
            IStateUpdateCommand stateUpdateCommand, 
            IStatePushCommand statePushCommand, 
            IStatePopCommand statePopCommand, 
            StatePeakQuery statePeakQuery)
        : base(stateUpdateCommand, statePushCommand, statePopCommand, statePeakQuery)
        {
            base.Steps.Add(0, Step0);
            base.Steps.Add(1, Step1);
            this.nimRaidBot = nimRaidBot;
            this.getActiveChatsForUser = getActiveChatsForUser;
            this.getNotifyLocationsByChatIdQuery = getNotifyLocationsByChatIdQuery;
            this.getActiveGymsForChatQuery = getActiveGymsForChatQuery;
            this.removeNotifyLocationCommand = removeNotifyLocationCommand;
            this.addNotifyLocationCommand = addNotifyLocationCommand;
        }

        public override string Key => "/gyms";
        public override string HelpText => "De-/Aktiviert die Benachrichtigung für einzelne Gyms";

        protected async Task<StateResult> Step0(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            if (!batchMode)
            {
                var chatId = base.GetChatId(message);
                var notifications = this.getNotifyLocationsByChatIdQuery.Execute(new GetNotifyLocationsByChatIdRequest(ChatIds: new[] { chatId } ));
                var gyms = GetCurrentGyms(chatId);

                await Helper.Business.SendGymList(gyms, notifications, chatId, bot);
            }

            return StateResult.ContinueWith(1);
        }

        private IEnumerable<PogoGym> GetCurrentGyms(long chatId)
        {
            var chats = this.getActiveChatsForUser.Execute(new GetActiveChatsForUserRequest(BotId: nimRaidBot.BotId, UserId: chatId));
            var gyms = this.getActiveGymsForChatQuery.Execute(new GetActiveGymsForChatRequest(ChatIds: chats.Select(x => x.ChatId)));

            return gyms;
        }

        protected async Task<StateResult> Step1(Message message, string text, TelegramBotClient bot, bool batchMode)
        {
            var chatId = base.GetChatId(message);
            var gyms = GetCurrentGyms(chatId);

            var gymId = await Helper.Business.GetGymId(text, gyms, chatId, bot);
            if (gymId == null)
                return StateResult.TryAgain;

            var gym = gyms.SingleOrDefault(x => x.Id == gymId.Value);
            if (gym == null)
                return StateResult.TryAgain;

            var notifications = this.getNotifyLocationsByChatIdQuery.Execute(new GetNotifyLocationsByChatIdRequest(ChatIds: new[] { chatId } ));

            if (notifications.Any(x => x.LocationId == gymId))
            {
                this.removeNotifyLocationCommand.Execute(new RemoveNotifyLocationRequest(ChatId: chatId, LocationId: gymId.Value));
                await bot.SendTextMessageAsync(chatId, $"{gym.Name} entfernt.").ConfigureAwait(false);
            }
            else
            {
                this.addNotifyLocationCommand.Execute(new AddNotifyLocationRequest(ChatId: chatId, LocationId: gymId.Value));
                await bot.SendTextMessageAsync(chatId, $"{gym.Name} hinzugefügt.").ConfigureAwait(false);
            }
            
            return StateResult.Finished;
        }
    }
}
