using EventBot.Business.Interfaces;
using EventBot.DataAccess.Commands.Base;
using EventBot.DataAccess.Queries.Base;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands
{
    public class MemberRemoved : IMemberRemoved
    {
        private readonly Raid.IUserRemove userRemove;
        private readonly IBotRemoveCommand removeBotCommand;
        private readonly IUserChannelRelationRemoveAllCommand userChannelRelationRemoveAllCommand;
        private readonly INumberOfBotsInChatQuery numberOfBotsInChatQuery;

        public MemberRemoved(
            Raid.IUserRemove userRemove,
            IBotRemoveCommand removeBotCommand,
            IUserChannelRelationRemoveAllCommand userChannelRelationRemoveAllCommand,
            INumberOfBotsInChatQuery numberOfBotsInChatQuery
            )
        {
            this.userRemove = userRemove;
            this.removeBotCommand = removeBotCommand;
            this.userChannelRelationRemoveAllCommand = userChannelRelationRemoveAllCommand;
            this.numberOfBotsInChatQuery = numberOfBotsInChatQuery;
        }

        public Task Execute(Message message, TelegramBotClient bot, long botId)
        {
            if (message.LeftChatMember.Id == botId)
            {
                this.removeBotCommand.Execute(new BotRemoveRequest { ChatId = message.Chat.Id, BotId = botId });
                var botsCount = this.numberOfBotsInChatQuery.Execute(new NumberOfBotsInChatRequest(ChatId: message.Chat.Id));

                if (botsCount == 0)
                    this.userChannelRelationRemoveAllCommand.Execute(new UserChannelRelationRemoveAllRequest { ChatId = message.Chat.Id });
            }
            else
            {
                this.userRemove.Execute(new Commands.Raid.UserRemoveRequest { UserId = message.LeftChatMember.Id, ChatId = message.Chat.Id });
            }

            return Task.CompletedTask;
        }
    }
}
