using EventBot.Business.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.PoGo
{
    public interface IIgnoreCommand : ICommand
    { }

    public class IgnoreCommand : Command, IIgnoreCommand
    {
        readonly DataAccess.Commands.PoGo.IIgnoreCommand anCommand;

        public IgnoreCommand(
            DataAccess.Commands.PoGo.IIgnoreCommand anCommand
            )
            : base()
        {
            this.anCommand = anCommand;
        }

        public override string HelpText
        {
            get { return "Aktiviert die Benachrichtigungen. /ignore 100"; }
        }

        public override string Key
        {
            get { return "/ignore"; }
        }

        public override async Task ExecuteAsync(Message message, string text, TelegramBotClient bot)
        {
            var userId = message.From.Id;
            int monsterId;

            if (!int.TryParse(text, out monsterId))
            {
                await bot.SendTextMessageAsync(message.Chat.Id, "Unbekannte Id").ConfigureAwait(false);
                return;
            }

            this.anCommand.Execute(new DataAccess.Commands.PoGo.IgnoreRequest { UserId = userId, MonsterId = monsterId });

            await bot.SendTextMessageAsync(message.Chat.Id, string.Format("{0} wird nun ignoriert", monsterId)).ConfigureAwait(false);
            return;
        }
    }
}
