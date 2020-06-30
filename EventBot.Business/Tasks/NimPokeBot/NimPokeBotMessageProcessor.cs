using EventBot.Business.Intrastructure;

namespace EventBot.Business.Tasks.NimPokeBot
{
    public class MessageProcessor : BaseMessageProcessor
    {
        public MessageProcessor(
            TelegramProxies.PogoTelegramProxy proxy,
            RaidDispatcher dispatcher,
            Queries.StatePeakQuery lastStateQuery,
            DataAccess.Commands.Raid.IModifyChatTitleCommand modifyChatTitleCommand
            ) : base(proxy, dispatcher, lastStateQuery, modifyChatTitleCommand)

        {
        }

        public override string Name => "NimPokeBot.MessageProcessor";

        public override int IntervallInMilliseconds => 60 * 60 * 1000;
    }
}
