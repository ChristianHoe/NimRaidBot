using System;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventBot.Business.Helper;
using EventBot.Business.TelegramProxies;
using EventBot.DataAccess.Queries.Raid;
using Telegram.Bot.Types.Enums;

namespace EventBot.Business.Tasks.NimPokeBot
{
    public class QuestAnnouncer : IScheduledTask
    {
        private readonly PichuProxy pichuProxy;
        private readonly BaseTelegramBotClient proxy;
        private readonly BaseTelegramBotClient raidBotProxy;

        private readonly IGetActivePogoGroups activeUsers;
        private readonly DataAccess.Queries.Raid.IGetNextQuestToProcessQuery getNextQuestToProcessQuery;
        private readonly DataAccess.Queries.Raid.IMarkQuestAsProcessingQuery markQuestAsProcssingQuery;
        private readonly DataAccess.Queries.Raid.IMarkQuestAsProcessedQuery markQuestAsProcessedQuery;
        private readonly DataAccess.Queries.Raid.IGetQuestByStopIdQuery getQuestByStopIdQuery;


        public QuestAnnouncer(
            PichuProxy pichuProxy,
            PogoTelegramProxy proxy,
            TelegramProxies.NimRaidBot raidBotProxy,
            DataAccess.Queries.Raid.IGetActivePogoGroups activeUsers,
            DataAccess.Queries.Raid.IGetNextQuestToProcessQuery getNextQuestToProcessQuery,
            DataAccess.Queries.Raid.IMarkQuestAsProcessingQuery markQuestAsProcssingQuery,
            DataAccess.Queries.Raid.IMarkQuestAsProcessedQuery markQuestAsProcessedQuery,
            DataAccess.Queries.Raid.IGetQuestByStopIdQuery getQuestByStopIdQuery
            )
        {
            this.pichuProxy = pichuProxy;
            this.proxy = proxy;
            this.raidBotProxy = raidBotProxy;
            this.activeUsers = activeUsers;

            this.getNextQuestToProcessQuery = getNextQuestToProcessQuery;
            this.markQuestAsProcssingQuery = markQuestAsProcssingQuery;
            this.markQuestAsProcessedQuery = markQuestAsProcessedQuery;
            this.getQuestByStopIdQuery = getQuestByStopIdQuery;
        }


        public string Name { get { return "NimPokeBot.QuestAnnouncer"; } }

        public bool RunExclusive => true;

        public int IntervallInMilliseconds { get { return 10 * 1000; } }


        const string CLIP_BOARD = "\U0001F4CB";

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                await this.SendQuests(cancellationToken).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                await Operator.SendMessage(this.proxy, string.Format("Es ist ein Fehler aufgetreten: {0}", ex.Message)).ConfigureAwait(false);
            }
        }

        private async Task SendQuests(CancellationToken cancellationToken)
        {
            var questToNotify = this.getNextQuestToProcessQuery.Execute(null);
            if (questToNotify == null)
                return;

            var canProcess = this.markQuestAsProcssingQuery.Execute(new MarkQuestAsProcessingRequest { Id = questToNotify.StopId });
            if (!canProcess)
                return;

            // var chats = this.activeUsers.Execute(new GetActivePogoGroupsRequest { BotIds = new long[] { this.proxy.BotId } });
            // int numberOfCurrentActiveUsers = chats.Count(x => x.RaidLevel.HasValue);
            // if (numberOfCurrentActiveUsers <= 0)
            //     return;

            // var specials = this.getSpecialGymsForChatsQuery.Execute(new GetSpecialGymsForChatsRequest { ChatIds = chats.Select(x => x.ChatId).ToArray() });

            var quest = this.getQuestByStopIdQuery.Execute(new GetQuestByStopIdRequest { StopId = questToNotify.StopId });

            var text = new StringBuilder();
            text.AppendLine($"[{quest.StopName}](https://maps.google.com/?q={quest.Latitude.ToString(CultureInfo.InvariantCulture)},{quest.Longitude.ToString(CultureInfo.InvariantCulture)})");
            text.AppendLine($"{quest.Task}");
            text.AppendLine($"{quest.Reward}");

            await this.proxy.SendTextMessageAsync(Operator.TelegramId, text.ToString(), parseMode: ParseMode.Markdown, disableWebPagePreview: true).ConfigureAwait(false);

            var isProcessed = this.markQuestAsProcessedQuery.Execute(new MarkQuestAsProcessedRequest { Id = questToNotify.StopId });

            if (!isProcessed)
            {
                await Operator.SendMessage(this.proxy, $"NimRaidBot: Nachricht konnte nicht auf processed gesetzt werden für {questToNotify.StopId}.").ConfigureAwait(false);
            }

            return;
        }
    }
}