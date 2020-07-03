using Telegram.Bot;

namespace EventBot.Business.TelegramProxies
{
    public class MinunBot : TelegramBotClient
    {
        public MinunBot(string token)
            : base(token)
        {
        }
    }
}
