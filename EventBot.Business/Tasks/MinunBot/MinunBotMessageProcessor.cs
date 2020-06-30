using EventBot.Business.Intrastructure;

namespace EventBot.Business.Tasks.MinunBot
{
    public class MessageProcessor : BaseMessageProcessor
    {
        public MessageProcessor(
            TelegramProxies.MinunBot proxy,
            MinunDispatcher dispatcher,
            Queries.StatePeakQuery lastStateQuery,
            DataAccess.Commands.Raid.IModifyChatTitleCommand modifyChatTitleCommand
            ) : base(proxy, dispatcher, lastStateQuery, modifyChatTitleCommand)
        {
        }

        public override string Name => "MinunBot.MessageProcessor";

        public override int IntervallInMilliseconds => 60 * 60 * 1000;
    }
}
