using EventBot.Business.Intrastructure;

namespace EventBot.Business.Tasks.NimFarmBot
{
    public class MessageProcessor : BaseMessageProcessor
    {
        public MessageProcessor(
            TelegramProxies.NimFarmBot proxy,
            FarmDispatcher dispatcher,
            Queries.StatePeakQuery lastStateQuery,
            DataAccess.Commands.Raid.IModifyChatTitleCommand modifyChatTitleCommand
            ) : base(proxy, dispatcher, lastStateQuery, modifyChatTitleCommand)

        {
        }

        public override string Name => "NimFarmBot.MessageProcessor";

        public override int IntervallInMilliseconds => 60 * 60 * 1000;
    }
}
