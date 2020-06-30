using EventBot.Business.Intrastructure;
using EventBot.Business.Tasks;
using EventBot.Business.TelegramProxies;

namespace EventBot.Business.NimRaidBot
{
    public class MessageProcessor : BaseMessageProcessor
    {
        public MessageProcessor(
            TelegramProxies.NimRaidBot proxy,
            RaidDispatcher dispatcher,
            Queries.StatePeakQuery lastStateQuery,
            DataAccess.Commands.Raid.IModifyChatTitleCommand modifyChatTitleCommand
            ) : base(proxy, dispatcher, lastStateQuery, modifyChatTitleCommand)

        {
        }

        public override string Name => "NimRaidBot.MessageProcessor";

        public override int IntervallInMilliseconds => 60 * 60 * 1000;
    }
}
