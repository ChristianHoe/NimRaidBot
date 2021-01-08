using EventBot.Business.Interfaces;
using EventBot.DataAccess.Commands.Base;
using EventBot.DataAccess.Commands.Raid;
using EventBot.DataAccess.Queries.Raid;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands
{
    public class MemberAdded : IMemberAdded
    {
        protected readonly IGetCurrentChatSettingsQuery getCurrentChatSettingsQuery;
        protected readonly IAddChatCommand addChatCommand;
        protected readonly Raid.IUserAdd userAdd;
        protected readonly IBotAddCommand addBotCommand;

        public MemberAdded(
            IBotAddCommand addBotCommand,
            Raid.IUserAdd userAdd,
            IGetCurrentChatSettingsQuery getCurrentChatSettingsQuery,
            IAddChatCommand addChatCommand
            )
        {
            this.addBotCommand = addBotCommand;
            this.userAdd = userAdd;
            this.getCurrentChatSettingsQuery = getCurrentChatSettingsQuery;
            this.addChatCommand = addChatCommand;
        }

        public virtual Task WelcomeUserAsync(TelegramBotClient proxy, long chatId, string name)
        {
            return Task.CompletedTask;
        }

        public async Task Execute(Message message, TelegramBotClient proxy, long botId)
        {
            var chatId = message.Chat.Id;
            foreach (var member in message.NewChatMembers)
            {
                if (member.Id == botId)
                {
                    var currentSettings = this.getCurrentChatSettingsQuery.Execute(new GetCurrentChatSettingsRequest(ChatId: chatId));
                    if (currentSettings == null)
                    {
                        this.addChatCommand.Execute(new AddChatRequest(ChatId: chatId, Name: message.Chat.Title));
                    }

                    this.addBotCommand.Execute(new BotAddRequest { ChatId = chatId, BotId = botId });

                    // you do not get a list of all current members, so just take the admins
                    var admins = await proxy.GetChatAdministratorsAsync(message.Chat.Id);
                    foreach (var user in admins)
                    {
                        this.userAdd.Execute(new Commands.Raid.UserAddRequest { UserId = user.User.Id, ChatId = chatId, UserName = user.User.Username });
                    }

                    return;
                }
                else
                {
                    if (member.IsBot)
                        return;

                    var name = member.Username;
                    if (string.IsNullOrEmpty(name))
                        name = member.FirstName;

                    if (string.IsNullOrEmpty(name))
                        name = member.LastName;

                    if (string.IsNullOrEmpty(name))
                        name = "?";

                    this.userAdd.Execute(new Commands.Raid.UserAddRequest { UserId = member.Id, ChatId = chatId, UserName = name });

                    // you can not initialize a conversation from a bot, just leave a message here
                    await WelcomeUserAsync(proxy, chatId, name);

                    return;

                }
            }
        }
    }
}
