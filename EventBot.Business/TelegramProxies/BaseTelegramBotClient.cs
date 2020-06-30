using Telegram.Bot;

namespace EventBot.Business.TelegramProxies
{
    public class BaseTelegramBotClient : TelegramBotClient
    {
        public BaseTelegramBotClient(string token)
            : base(token)
        {
        }
    }
}
