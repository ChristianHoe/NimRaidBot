using EventBot.Business.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.Minun
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

        public override string Key => "/an";
        public override string HelpText => "Aktiviert die Benachrichtigungen. /an";

        public override async Task ExecuteAsync(Message message, string text, TelegramBotClient bot)
        {
            this.anCommand.Execute(new DataAccess.Commands.PoGo.AnRequest(UserId: GetUserId(message), BotId: bot.BotId));
            await bot.SendTextMessageAsync(GetChatId(message), "Benachrichtigungen eingeschaltet").ConfigureAwait(false);

            return;
        }
    }
}
