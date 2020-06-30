using EventBot.Business.Interfaces;
using EventBot.Business.TelegramProxies;
using EventBot.DataAccess.Commands.Base;
using EventBot.DataAccess.Commands.Minun;
using EventBot.DataAccess.Commands.Raid;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.Minun
{
    public interface IStartCommand : ICommand
    { }

    public class StartCommand : Command, IStartCommand
    {
        private readonly IBotAddCommand botAddCommand;
        private readonly IAddUserCommand addMinunUserCommand;
        private readonly IDeactiveMinunUserCommand deactiveMinunUserCommand;

        public StartCommand(
            IBotAddCommand botAddCommand,
            IAddUserCommand addMinunUserCommand,
            IDeactiveMinunUserCommand deactiveMinunUserCommand
            )
        {
            this.botAddCommand = botAddCommand;
            this.addMinunUserCommand = addMinunUserCommand;
            this.deactiveMinunUserCommand = deactiveMinunUserCommand;
        }

        public override string Key => "/start";
        public override string HelpText => "Willkommensnachricht";

        public override async Task<bool> Execute(Message message, string text, BaseTelegramBotClient bot, int step)
        {
            this.botAddCommand.Execute(new BotAddRequest { ChatId = base.GetChatId(message), BotId = bot.BotId });
            this.addMinunUserCommand.Execute(new AddUserRequest { UserId = base.GetUserId(message), FirstName = message.From.Username });
            try
            {
                await bot.SendTextMessageAsync(GetChatId(message), "Mit /nutzer kann der Bot konfiguriert werden.").ConfigureAwait(false);
            }
            catch(ApiRequestException ex)
            {
                if (ex.ErrorCode == 403) // blocked by user
                    this.deactiveMinunUserCommand.Execute(new DeactiveMinunUserRequest { UserId = base.GetUserId(message), BotId = bot.BotId });
                else
                    throw;
            }

            return true;
        }
    }
}
