using EventBot.Business.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace EventBot.Business.Commands.PoGo
{
    public interface IPollCommand : ICommand
    { }

    public class PollCommand : Command, IPollCommand
    {
        readonly DataAccess.Commands.PoGo.INewPollCommand newPollCommand;

        public PollCommand(
            DataAccess.Commands.PoGo.INewPollCommand newPollCommand
            )
            : base()
        {
            this.newPollCommand = newPollCommand;
        }

        public override string HelpText
        {
            get { return "..."; }
        }

        public override string Key
        {
            get { return "/poll"; }
        }

        public override async Task ExecuteAsync(Message message, string text, TelegramBotClient bot)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                await bot.SendTextMessageAsync(message.Chat.Id, "Keine Anzeigetext gefunden.").ConfigureAwait(false);
                return;
            }

            var inlineKeyboard = new InlineKeyboardMarkup(new[] { new InlineKeyboardButton("0") { CallbackData = "0" }, new InlineKeyboardButton("+1") { CallbackData = "1" }, new InlineKeyboardButton("+2") { CallbackData = "2"} } );

            var msg = await bot.SendTextMessageAsync(message.Chat.Id, text, replyMarkup: inlineKeyboard).ConfigureAwait(false);

            this.newPollCommand.Execute(new DataAccess.Commands.PoGo.NewPollRequest(ChatId: msg.Chat.Id, MessageId: msg.MessageId));

            return;
        }
    }
}
