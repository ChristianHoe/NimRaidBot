using Telegram.Bot;

namespace EventBot.Business.TelegramProxies
{
    public class NimFarmBot : TelegramBotClient
    {
        public NimFarmBot(string token)
            : base(token)
        {
        }
    }
}
