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
        Task<AnswerResult> Execute(CallbackQuery message, string text, TelegramBotClient bot);
    }
}
