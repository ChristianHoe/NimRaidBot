using Telegram.Bot;

namespace EventBot.Business.TelegramProxies
{
    public class TelegramProxy : TelegramBotClient
    {
        public TelegramProxy(string token)
            : base(token)
        {
        }
    }
}
