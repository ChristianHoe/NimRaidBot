using Telegram.Bot;

namespace EventBot.Business.TelegramProxies
{
    public class PogoTelegramProxy : TelegramBotClient
    {
        public PogoTelegramProxy(string token)
            : base(token)
        {
        }
    }
}
