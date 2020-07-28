using EventBot.Business.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.PoGo
{
    public interface IAusCommand : ICommand
    { }

    public class AusCommand : Command, IAusCommand
    {
        readonly DataAccess.Commands.PoGo.IAusCommand anCommand;

        public AusCommand(
            DataAccess.Commands.PoGo.IAusCommand anCommand
            )
            : base()
        {
            this.anCommand = anCommand;
        }

        public override string HelpText
        {
            get { return "Deaktiviert die Benachrichtigungen. /aus"; }
        }

        public override string Key
        {
            get { return "/aus"; }
        }

        public override async Task ExecuteAsync(Message message, string text, TelegramBotClient bot)
        {
            var userId = message.From.Id;

            this.anCommand.Execute(new DataAccess.Commands.PoGo.AusRequest { UserId = userId });

            await bot.SendTextMessageAsync(message.Chat.Id, "Benachrichtigungen ausgeschaltet").ConfigureAwait(false);
            return;
        }
    }
}
