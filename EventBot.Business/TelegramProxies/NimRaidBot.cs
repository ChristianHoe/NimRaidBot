using Telegram.Bot;

namespace EventBot.Business.TelegramProxies
{
    public class NimRaidBot : TelegramBotClient
    {
        public NimRaidBot(string token)
            : base(token)
        {
        }
    }
}
