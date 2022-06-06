using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace EventBot.Business.Helper
{
    public static class Operator
    {
        public static int? TelegramId;

        public static async Task SendMessage(TelegramBotClient proxy, string userDefinedText, [CallerFilePath] string filePath = "", [CallerMemberName] string callee = "")
        {
            await SendMessage(proxy, userDefinedText, null, filePath, callee);
        }

        public static async Task SendMessage(TelegramBotClient proxy, string userDefinedText, Exception? exception, [CallerFilePath] string filePath = "", [CallerMemberName] string callee = "")
        {
            if (TelegramId == null)
                return;

            StringBuilder errorMessage = new StringBuilder(filePath + "-" + callee + "-" + userDefinedText);
            var ex = exception;

            if (ex != null)
            {
                errorMessage.Append("Exception: ");
            }

            while (ex != null)
            {
                errorMessage.AppendLine(ex.Message);
                ex = ex.InnerException;
            }

            await proxy.SendTextMessageAsync(TelegramId, errorMessage.ToString(), parseMode: ParseMode.Markdown).ConfigureAwait(false);
        }
    }
}
