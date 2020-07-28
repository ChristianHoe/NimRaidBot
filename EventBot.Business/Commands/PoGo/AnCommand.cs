using EventBot.Business.Helper;
using EventBot.Business.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.PoGo
{
    public interface IAnCommand : ICommand
    { }

    public class AnCommand : Command, IAnCommand
    {
        readonly DataAccess.Commands.PoGo.IAnCommand anCommand;

        public AnCommand(
            DataAccess.Commands.PoGo.IAnCommand anCommand
            )
            : base()
        {
            this.anCommand = anCommand;
        }

        public override string HelpText
        {
            get { return "Aktiviert die Benachrichtigungen. /an"; }
        }

        public override string Key
        {
            get { return "/an"; }
        }

        public override async Task ExecuteAsync(Message message, string text, TelegramBotClient bot)
        {
            var userId = message.From.Id;

            if (message.Chat.Id == Operator.TelegramId)
            {
                this.anCommand.Execute(new DataAccess.Commands.PoGo.AnRequest { UserId = userId });
                await bot.SendTextMessageAsync(message.Chat.Id, "Benachrichtigungen eingeschaltet").ConfigureAwait(false);
            }
            else
            {
                await bot.SendTextMessageAsync(message.Chat.Id, "Bitte warte auf Freischaltung").ConfigureAwait(false);
                await Helper.Operator.SendMessage(bot, $"Nutzer {message.Chat.Id} wartet auf Freischaltung").ConfigureAwait(false);
            }
            return;
        }
    }
}
