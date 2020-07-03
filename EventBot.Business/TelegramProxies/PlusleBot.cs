using Telegram.Bot;

namespace EventBot.Business.TelegramProxies
{
    public class PlusleBot : TelegramBotClient
    {
        public PlusleBot(string token)
            : base(token)
        {
        }
    }
}
