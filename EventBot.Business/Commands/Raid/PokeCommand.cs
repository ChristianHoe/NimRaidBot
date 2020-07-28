using EventBot.Business.Interfaces;
using EventBot.DataAccess.Commands.Minun;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Commands.Raid
{
    public interface IPokeCommand : ICommand
    { }

    public class PokeCommand : Command, IPokeCommand
    {
        private readonly IAddPokeToNotificationListCommand addPokeCommand;
        private readonly IRemovePokeFromNotificationListCommand removePokeCommand;

        public PokeCommand(
            IAddPokeToNotificationListCommand addPokeCommand,
            IRemovePokeFromNotificationListCommand removePokeCommand
            )
        {
            this.addPokeCommand = addPokeCommand;
            this.removePokeCommand = removePokeCommand;
        }

        public override string HelpText => "De-/Aktiviert die Benachrichtung zu einem Pokemon. Ein negativer IV entfernt die Benachrichtigung. /poke [id+] [iv*] [M|W*]";
        public override string Key => "/poke";

        public override async Task ExecuteAsync(Message message, string text, TelegramBotClient bot)
        {
            var chatId = base.GetChatId(message);

            var cmd = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (cmd.Length < 1 || 3 < cmd.Length)
            {
                await bot.SendTextMessageAsync(chatId, "Zu viele/wenige Parameter.").ConfigureAwait(false);
                return;
            }

            if (!int.TryParse(cmd[0], NumberStyles.Any, CultureInfo.InvariantCulture, out int pokeId))
            {
                await bot.SendTextMessageAsync(chatId, "PokeId konnte nicht erkannt werden, bitte probiere es noch einmal.").ConfigureAwait(false);
                return;
            }

            if (pokeId < 1)
                pokeId = 1;

            int? iv = null;
            if (cmd.Length > 1)
            {
                if (!int.TryParse(cmd[1], NumberStyles.Any, CultureInfo.InvariantCulture, out int tmpiv))
                {
                    await bot.SendTextMessageAsync(chatId, "IV konnte nicht erkannt werden, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return;
                }

                if (tmpiv < 0)
                {
                    this.removePokeCommand.Execute(new RemovePokeFromNotificationListRequest { ChatId = chatId, PokeId = pokeId });
                    await bot.SendTextMessageAsync(chatId, $"Benachrichtigungen für {pokeId} deaktiviert.").ConfigureAwait(false);

                    return;
                }

                if (tmpiv > 100)
                    tmpiv = 100;

                iv = tmpiv;

                if (tmpiv == 0)
                    iv = null;
            }

            char? gender = null;
            if (cmd.Length > 2)
            {
                var g = cmd[2];

                if (string.Compare(g, "w", StringComparison.InvariantCultureIgnoreCase) == 0)
                    gender = 'w';

                if (string.Compare(g, "m", StringComparison.InvariantCultureIgnoreCase) == 0)
                    gender = 'm';


                if (gender == null)
                {
                    await bot.SendTextMessageAsync(chatId, "Geschlecht konnte nicht erkannt werden, bitte probiere es noch einmal.").ConfigureAwait(false);
                    return;
                }
            }

            this.addPokeCommand.Execute(new AddPokeToNotificationListRequest { ChatId = chatId, Gender = gender, IV = iv, PokeId = pokeId, Show = true });

            await bot.SendTextMessageAsync(chatId, $"Benachrichtigungen für {pokeId} {iv} {gender} aktiviert.").ConfigureAwait(false);

            return;
        }
    }
}
