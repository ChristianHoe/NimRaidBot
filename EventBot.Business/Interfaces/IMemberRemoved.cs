using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventBot.Business.Interfaces
{
    public interface IMemberRemoved
    {
        Task Execute(Message message, TelegramBotClient proxy, long? botId);
    }
}
