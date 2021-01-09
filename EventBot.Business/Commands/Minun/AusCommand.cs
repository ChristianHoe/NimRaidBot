using EventBot.Business.Interfaces;
using EventBot.Business.TelegramProxies;
using EventBot.DataAccess.Commands.Minun;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.Minun
{
    public interface IAusCommand : ICommand
    { }

    public class AusCommand : Command, IAusCommand
    {
        private readonly IDisableNotificationsCommand disableNotificationCommand;
        private readonly IDeactiveMinunUserCommand deactiveMinunUserCommand;

        public AusCommand(
            IDisableNotificationsCommand disableNotificationCommand,
            IDeactiveMinunUserCommand deactiveMinunUserCommand
            )
            : base()
        {
            this.disableNotificationCommand = disableNotificationCommand;
            this.deactiveMinunUserCommand = deactiveMinunUserCommand;
        }

        public override string Key => "/aus";
        public override string HelpText => "Deaktiviert die Benachrichtigungen. /aus";

        public override async Task ExecuteAsync(Message message, string text, TelegramBotClient bot)
        {
            this.disableNotificationCommand.Execute(new DisableNotificationsRequest(UserId: GetUserId(message), BotId: bot.BotId));

            try
            {
                await bot.SendTextMessageAsync(GetChatId(message), "Benachrichtigungen ausgeschaltet").ConfigureAwait(false);
            }
            catch (ApiRequestException ex)
            {
                if (ex.ErrorCode == 403) // blocked by user
                    this.deactiveMinunUserCommand.Execute(new DeactiveMinunUserRequest(UserId: base.GetUserId(message), BotId: bot.BotId));
                else
                    throw;
            }

            return;
        }
    }
}
