using EventBot.Business.Helper;
using EventBot.Business.Tasks;
using EventBot.Business.TelegramProxies;
using EventBot.DataAccess.Commands.Minun;
using EventBot.DataAccess.ModelsEx;
using EventBot.DataAccess.Queries.Location;
using EventBot.DataAccess.Queries.Minun;
using EventBot.DataAccess.Queries.Raid;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace EventBot.Business.NimRaidBot
{
    public class Announcer : IScheduledTask
    {
        readonly private TelegramBotClient proxy;
        private readonly MinunBot minunBot;
        private readonly IGetNextNewRaidQuery getNextNewRaidQuery;
        private readonly IMarkEventAsProcessedQuery markAsProcessedQuery;
        private readonly IMarkEventAsProcessingQuery markAsProcessingQuery;

        private readonly IGetNextPollToProcessQuery getNextPollToProcessQuery;
        private readonly IMarkPollAsProcessingQuery markPollAsProcessingQuery;
        private readonly IMarkPollAsProcessedQuery markPollAsProcessedQuery;

        readonly private IGetActivePogoGroups activeUsers;
        private readonly IGetSpecialGymsForChatsQuery getSpecialGymsForChatsQuery;

        readonly private Commands.Raid.ICreatePollCommand pollCommand;
        private readonly Commands.Raid.ICreatePollText createPollText;

        private readonly DataAccess.Queries.PoGo.IPollVotesUsers pollVotesUsers;
        private readonly IGetActivePollByRaidId getActivePollByRaidId;
        private readonly IGetRaidByIdQuery getRaidByIdQuery;

        private readonly IGetRaidBossPreferencesAllQuery getRaidBossPreferencesQuery;
        private readonly IGetCurrentNotificationsQuery getCurrentNotificationsQuery;
        private readonly IDeactiveMinunUserCommand deactiveMinunUserCommand;
        private readonly DataAccess.Queries.Pokes.IGetPokesForChatQuery getPokesForChatQuery;

        private readonly DataAccess.Queries.Raid.IGetRaidTimeOffsetsQuery getRaidTimeOffsetsQuery;

        public Announcer(
            TelegramProxies.NimRaidBot proxy,
            TelegramProxies.MinunBot minunBot,
            IGetNextNewRaidQuery getNextNewRaidQuery,
            IMarkEventAsProcessedQuery markAsProcessedQuery,
            IMarkEventAsProcessingQuery markAsProcessingQuery,

            DataAccess.Queries.Raid.IGetActivePogoGroups activeUsers,
            IGetSpecialGymsForChatsQuery getSpecialGymsForChatsQuery,
            Commands.Raid.ICreatePollCommand pollCommand,
            Commands.Raid.ICreatePollText createPollText

            , DataAccess.Queries.PoGo.IPollVotesUsers pollVotesUsers
            , DataAccess.Queries.Raid.IGetActivePollByRaidId getActivePollByRaidId
            , DataAccess.Queries.Raid.IGetRaidTimeOffsetsQuery getRaidTimeOffsetsQuery

            , IGetNextPollToProcessQuery getNextPollToProcessQuery,
            IMarkPollAsProcessedQuery markPollAsProcessedQuery,
            IMarkPollAsProcessingQuery markPollAsProcessingQuery,
            IGetRaidByIdQuery getRaidByIdQuery,
            DataAccess.Queries.Pokes.IGetPokesForChatQuery getPokesForChatQuery,
            IGetRaidBossPreferencesAllQuery getRaidBossPreferencesQuery,
            DataAccess.Queries.Location.IGetCurrentNotificationsQuery getCurrentNotificationsQuery,
            IDeactiveMinunUserCommand deactiveMinunUserCommand
            )
        {
            this.proxy = proxy;
            this.minunBot = minunBot;
            this.getNextNewRaidQuery = getNextNewRaidQuery;
            this.markAsProcessedQuery = markAsProcessedQuery;
            this.markAsProcessingQuery = markAsProcessingQuery;

            this.activeUsers = activeUsers;

            this.pollCommand = pollCommand;

            this.createPollText = createPollText;

            this.pollVotesUsers = pollVotesUsers;
            this.getActivePollByRaidId = getActivePollByRaidId;
            this.getRaidTimeOffsetsQuery = getRaidTimeOffsetsQuery;

            this.getNextPollToProcessQuery = getNextPollToProcessQuery;
            this.markPollAsProcessedQuery = markPollAsProcessedQuery;
            this.markPollAsProcessingQuery = markPollAsProcessingQuery;
            this.getRaidByIdQuery = getRaidByIdQuery;

            this.getSpecialGymsForChatsQuery = getSpecialGymsForChatsQuery;
            this.getPokesForChatQuery = getPokesForChatQuery;
            this.getRaidBossPreferencesQuery = getRaidBossPreferencesQuery;
            this.getCurrentNotificationsQuery = getCurrentNotificationsQuery;
            this.deactiveMinunUserCommand = deactiveMinunUserCommand;
        }

        public string Name { get { return "NimRaidBot.Announcer"; } }

        public bool RunExclusive => false;
        public int IntervallInMilliseconds { get { return 200; } }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                await this.UpdateEventPolls().ConfigureAwait(false);
                await this.CreateEventPolls().ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                await Operator.SendMessage(this.proxy, "Es ist ein Fehler aufgetreten:", ex);
            }
        }

        private async Task UpdateEventPolls()
        {
            var pollToUpdate = this.getNextPollToProcessQuery.Execute(null);
            if (pollToUpdate == null)
                return;

            var canProcess = this.markPollAsProcessingQuery.Execute(new MarkPollAsProcessingRequest(Id: pollToUpdate.Id));
            if (!canProcess)
                return;

            var currentResults = this.pollVotesUsers.Execute(new DataAccess.Queries.PoGo.PollVotesRequest(ChatId: pollToUpdate.ChatId, MessageId: pollToUpdate.MessageId));

            var specials = this.getSpecialGymsForChatsQuery.Execute(new GetSpecialGymsForChatsRequest(ChatIds: new[] { pollToUpdate.ChatId }));

            var raidToNotify = this.getRaidByIdQuery.Execute(new GetRaidByIdRequest(RaidId: pollToUpdate.RaidId ?? 0));
            var bossPreferences = this.getRaidBossPreferencesQuery.Execute(new GetRaidBossPreferencesAllRequest { });
            var raidTimeOffsets = this.getRaidTimeOffsetsQuery.Execute(new GetRaidTimeOffsetsRequest(OffsetId: pollToUpdate.TimeOffsetId));
            var pollText = this.createPollText.Execute(new Commands.Raid.SendRaidPollRequest(PokeNames: Models.GoMap.Helper.PokeNames, Raid: raidToNotify, Votes: currentResults, SpecialGymSettings: specials.Where(x => x.ChatId == pollToUpdate.ChatId && x.GymId == raidToNotify.GymId && x.Type == (int)GymType.ExRaid), RaidPreferences: bossPreferences, TimeOffsets: raidTimeOffsets));

            try
            {
                await proxy.EditMessageTextAsync(pollToUpdate.ChatId, (int)pollToUpdate.MessageId, pollText.Text, parseMode: pollText.ParseMode, replyMarkup: pollText.InlineKeyboardMarkup, disableWebPagePreview: true).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await Operator.SendMessage(this.proxy, $"{this.Name}: Nachricht '{pollText?.Text}' für Gruppe {pollToUpdate.ChatId} konnte nicht aktualisiert werden.", ex).ConfigureAwait(false);
            }

            var isProcessed = this.markPollAsProcessedQuery.Execute(new MarkPollAsProcessedRequest(Id: pollToUpdate.Id));
            if (!isProcessed)
            {
                await Operator.SendMessage(this.proxy, $"{this.Name} - Poll konnte nicht auf processed gesetzt werden für {pollToUpdate.Id}.").ConfigureAwait(false);
            }

            return;
        }

        private async Task CreateEventPolls()
        {
            var raidToNotify = this.getNextNewRaidQuery.Execute(null);
            if (raidToNotify == null)
                return;

            var canProcess = this.markAsProcessingQuery.Execute(new MarkEventAsProcessingRequest(Id: raidToNotify.Id));
            if (!canProcess)
                return;

            var locationsToNotify = getCurrentNotificationsQuery.Execute(new GetCurrentNotificationsRequest(Threshold: DateTime.UtcNow.AddDays(-7), LocationId: raidToNotify.GymId));

            var chats = this.activeUsers.Execute(new GetActivePogoGroupsRequest(BotIds: new long[] { this.proxy.BotId }));
            int numberOfCurrentActiveUsers = chats.Count(x => x.RaidLevel.HasValue);
            if (numberOfCurrentActiveUsers <= 0)
                return;

            var specials = this.getSpecialGymsForChatsQuery.Execute(new GetSpecialGymsForChatsRequest(ChatIds: chats.Select(x => x.ChatId).ToArray()));

            foreach (var chat in chats.Where(x => x.LatMin <= raidToNotify.Latitude && raidToNotify.Latitude <= x.LatMax && x.LonMin <= raidToNotify.Longitude && raidToNotify.Longitude <= x.LonMax && x.RaidLevel.HasValue))
            {
                try
                {
                    var pokesToNotify = getPokesForChatQuery.Execute(new DataAccess.Queries.Pokes.GetPokesForChatRequest(ChatId: chat.ChatId));

                    if (raidToNotify.ChatId.HasValue && raidToNotify.ChatId != chat.ChatId)
                        continue;

                    if (raidToNotify.Level < chat.RaidLevel && !pokesToNotify.Any(x => x.PokeId == raidToNotify.PokeId))
                        continue;

                    if (specials.Any(x => x.ChatId == chat.ChatId && x.GymId == raidToNotify.GymId && x.Type == (int)GymType.Exclude))
                        continue;

                    var raidTimeOffsets = this.getRaidTimeOffsetsQuery.Execute(new GetRaidTimeOffsetsRequest(OffsetId: chat.TimeOffsetId));
                    var bossPreferences = this.getRaidBossPreferencesQuery.Execute(new GetRaidBossPreferencesAllRequest { });

                    var poll = this.getActivePollByRaidId.Execute(new DataAccess.Queries.Raid.GetActivePollByRaidRequest(RaidId: raidToNotify.Id, ChatId: chat.ChatId));
                    if (poll == null)
                    {
                        var result = this.createPollText.Execute(new Commands.Raid.SendRaidPollRequest(PokeNames: Models.GoMap.Helper.PokeNames, Raid: raidToNotify, Votes: Enumerable.Empty<DataAccess.Queries.PoGo.PollVoteResponse>(), SpecialGymSettings: specials.Where(x => x.ChatId == chat.ChatId && x.GymId == raidToNotify.GymId && x.Type == (int)GymType.ExRaid), RaidPreferences: bossPreferences, TimeOffsets: raidTimeOffsets));

                        await this.pollCommand.Execute(new Commands.Raid.CreatePollRequest(InlineKeyboardMarkup: result.InlineKeyboardMarkup, Raid: raidToNotify, Text: result.Text, ParseMode: result.ParseMode, TimeOffsetId: chat.TimeOffsetId), chat.ChatId, this.proxy);
                    }
                    else
                    {
                        var currentResults = this.pollVotesUsers.Execute(new DataAccess.Queries.PoGo.PollVotesRequest(ChatId: poll.ChatId, MessageId: poll.MessageId));
                        var pollText = this.createPollText.Execute(new Commands.Raid.SendRaidPollRequest(PokeNames: Models.GoMap.Helper.PokeNames, Raid: raidToNotify, Votes: currentResults, SpecialGymSettings: specials.Where(x => x.ChatId == chat.ChatId && x.GymId == raidToNotify.GymId && x.Type == (int)GymType.ExRaid), RaidPreferences: bossPreferences, TimeOffsets: raidTimeOffsets));

                        try
                        {
                            await proxy.EditMessageTextAsync(poll.ChatId, (int)poll.MessageId, pollText.Text, parseMode: pollText.ParseMode, replyMarkup: pollText.InlineKeyboardMarkup, disableWebPagePreview: true).ConfigureAwait(false);
                        }
                        catch(Exception ex)
                        {
                            await Operator.SendMessage(this.proxy, $"NimRaidBot: Nachricht '{pollText?.Text}' für Gruppe {chat.ChatId} konnte nicht aktualisiert werden.", ex).ConfigureAwait(false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    await Operator.SendMessage(this.proxy, string.Format("NimRaidBot: Es ist ein Fehler für Gruppe {1} aufgetreten: {0}", ex.Message, chat.ChatId));
                }
            }

            string msg = $"{raidToNotify.GymName} {raidToNotify.Level} {raidToNotify.Start}";
            foreach (var notification in locationsToNotify.Where(x => x.BotId == minunBot.BotId))
            {
                try
                {
                    await minunBot.SendTextMessageAsync(notification.ChatId, msg).ConfigureAwait(false);
                }
                catch (ApiRequestException ex)
                {
                    if (ex.ErrorCode == 403) // blocked by user
                        this.deactiveMinunUserCommand.Execute(new DeactiveMinunUserRequest { UserId = notification.ChatId, BotId = notification.BotId });
                    else
                        await Operator.SendMessage(this.proxy, string.Format("NimRaidBot: Es ist ein Fehler für Gruppe {1} aufgetreten: {0}", ex.Message, notification.ChatId));
                 }
            }

            var isProcessed = this.markAsProcessedQuery.Execute(new MarkEventAsProcessedRequest(Id: raidToNotify.Id));

            if (!isProcessed)
            {
                await Operator.SendMessage(this.proxy, $"NimRaidBot: Nachricht konnte nicht auf processed gesetzt werden für {raidToNotify.Id}.").ConfigureAwait(false);
            }

            return;
        }
    }
}
