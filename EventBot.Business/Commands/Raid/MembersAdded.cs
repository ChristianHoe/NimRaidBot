using System.Threading.Tasks;
using EventBot.DataAccess.Commands.Base;
using EventBot.DataAccess.Commands.Raid;
using EventBot.DataAccess.Queries.Raid;
using Telegram.Bot;

namespace EventBot.Business.Commands.Raid
{
    public class MembersAdded : MemberAdded
    {
        public MembersAdded(
            IBotAddCommand addBotCommand,
            IUserAdd userAdd,
            IGetCurrentChatSettingsQuery getCurrentChatSettingsQuery,
            IAddChatCommand addChatCommand
            ) : base(addBotCommand, userAdd, getCurrentChatSettingsQuery, addChatCommand)
        {
        }

        public override async Task WelcomeUserAsync(TelegramBotClient proxy, long chatId, string name)
        {
            // Ask user to configure user settings for raid polls
            var currentSettings = getCurrentChatSettingsQuery.Execute(new GetCurrentChatSettingsRequest { ChatId = chatId });
            if (currentSettings?.RaidLevel != null)
                await proxy.SendTextMessageAsync(chatId, $"Hi {name}! Bitte wende dich an @MinunBot für weitere Einstellungen.").ConfigureAwait(false);
        }
    }
}
