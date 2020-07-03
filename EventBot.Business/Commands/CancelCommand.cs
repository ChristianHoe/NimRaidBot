using EventBot.Business.Interfaces;
using EventBot.DataAccess.Commands;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands
{
    public interface ICancelCommand : ICommand
    { }


    public class CancelCommand : Command, ICancelCommand
    {
        private readonly IStateClearCommand stateClearCommand;

        public CancelCommand(IStateClearCommand stateClearCommand)
            : base()
        {
            this.stateClearCommand = stateClearCommand;
        }

        public override string HelpText
        {
            get { return "Bricht die Verarbeitung des aktuellen Befehls ab."; }
        }

        public override string Key
        {
            get { return "/cancel"; }
        }

        public override async Task<bool> Execute(Message message, string text, TelegramBotClient bot, int step)
        {
            this.stateClearCommand.Execute(new StateClearRequest { ChatId = message.Chat.Id });
            await bot.SendTextMessageAsync(message.Chat.Id, "Verarbeitung abgebrochen.");
            return true;
        }
    }
}
