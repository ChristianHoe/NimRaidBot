using EventBot.Business.Intrastructure;
using Telegram.Bot;

namespace EventBot.Business.Tasks.NimDebugBot
{
    public class MessageProcessor : BaseMessageProcessor
    {
        private readonly TelegramBotClient proxy;
        private readonly BaseDispatcher dispatcher;

        readonly private Queries.StatePeakQuery lastStateQuery;

        public MessageProcessor(
            TelegramProxies.PogoTelegramProxy proxy,
            RaidDispatcher dispatcher,
            Queries.StatePeakQuery lastStateQuery,
            DataAccess.Commands.Raid.IModifyChatTitleCommand modifyChatTitleCommand
            )
            : base(proxy, dispatcher, lastStateQuery, modifyChatTitleCommand)
        {
            this.proxy = proxy;
            this.dispatcher = dispatcher;
            this.lastStateQuery = lastStateQuery;
        }

        public override string Name => "NimDebugBot.MessageProcessor";

        public override int IntervallInMilliseconds => 1*1000;
    }
}
