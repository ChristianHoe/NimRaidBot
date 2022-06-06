using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Interfaces
{
    public struct AnswerResult
    {
        public AnswerResult(string message)
        {
            NotifiyUser = true;
            NotificationMessage = message;
        }

        public bool NotifiyUser { get; }
        public string NotificationMessage { get; }
    }

    public interface IAnswer
    {
        bool CanExecute(CallbackQuery message);
        Task<AnswerResult> ExecuteAsync(CallbackQuery message, string text, TelegramBotClient bot);
    }

    public abstract class Answer : IAnswer
    {
        public abstract bool CanExecute(CallbackQuery message);
        public abstract Task<AnswerResult> ExecuteAsync(CallbackQuery message, string text, TelegramBotClient bot);

        protected long GetChatId(CallbackQuery message)
        {
            return message.Message.Chat.Id;
        }

        protected int GetMessageId(CallbackQuery message)
        {
            return message.Message.MessageId;
        }

        protected long GetUserId(CallbackQuery message)
        {
            return message.From.Id;
        }
    }
}
