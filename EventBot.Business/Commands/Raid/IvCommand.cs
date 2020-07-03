using EventBot.Business.Interfaces;
using EventBot.DataAccess.Commands.Raid;
using System.Globalization;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.Raid
{
    public interface IIvCommand : ICommand
    { }

    public class IvCommand : Command, IIvCommand
    {
        private readonly ISetIvCommand setIvCommand;

        public IvCommand(
            ISetIvCommand setIvCommand
            )
        {
            this.setIvCommand = setIvCommand;
        }

        public override string HelpText => "Setzt das Minimum-IV-Level (0-100) welches gemeldet wird. -1 ist deaktiviert das Feature.";
        public override string Key => "/iv";

        public override async Task<bool> Execute(Message message, string text, TelegramBotClient bot, int step)
        {
            var chatId = base.GetChatId(message);

            if (!int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int iv))
            {
                await bot.SendTextMessageAsync(chatId, "IV-Wert konnte nicht erkannt werden, bitte probiere es noch einmal.").ConfigureAwait(false);
                return true;
            }

            if (iv < 0)
                iv = -1;

            if (iv > 100)
                iv = 100;

            this.setIvCommand.Execute(new SetIvRequest { ChatId = chatId, Iv = iv });

            await bot.SendTextMessageAsync(chatId, $"Minimum-IV-Level auf {iv} gestetzt.").ConfigureAwait(false);

            return true;
        }
    }
}
