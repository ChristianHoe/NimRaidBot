using EventBot.DataAccess.Models;
using EventBot.Business.Helper;
using EventBot.Business.TelegramProxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using EventBot.Business.Tasks;
using System.Threading;
using EventBot.DataAccess.Queries.Raid;

namespace EventBot.Business.NimPokeBot
{
    public class Announcer2 : IScheduledTask
    {
        private readonly TelegramBotClient proxy;
        private readonly TelegramBotClient raidBotProxy;

        private readonly IGetActivePogoGroups activeUsers;
        private readonly DataAccess.Queries.Pokes.IMarkAsProcessedQuery markAsProcessedQuery;
        private readonly DataAccess.Queries.Pokes.IGetPokesForChatQuery getPokesForChatQuery;
        private readonly DataAccess.Commands.Pokes.IAddPokeNotificationCommand addPokeNotificationCommand;

        private readonly Queries.IGetPokeQueueQuery getPokeQueueQuery;
        private readonly Queries.IGetThrottleQuery getThrottleQuery;
        private readonly DataAccess.Queries.PoGo.IGetAllChatsForArea getAllChatsForArea;

        public Announcer2(
            PogoTelegramProxy proxy,
            TelegramProxies.NimRaidBot raidBotProxy,
            DataAccess.Queries.Raid.IGetActivePogoGroups activeUsers,
            DataAccess.Queries.Pokes.IMarkAsProcessedQuery markAsProcessedQuery,
            DataAccess.Queries.Pokes.IGetPokesForChatQuery getPokesForChatQuery,
            DataAccess.Commands.Pokes.IAddPokeNotificationCommand addPokeNotificationCommand,
            Queries.IGetPokeQueueQuery getPokeQueueQuery,
            DataAccess.Queries.PoGo.IGetAllChatsForArea getAllChatsForArea,
            Queries.IGetThrottleQuery getThrottleQuery
            )
        {
            this.proxy = proxy;
            this.raidBotProxy = raidBotProxy;
            this.activeUsers = activeUsers;

            this.markAsProcessedQuery = markAsProcessedQuery;
 
            this.getPokesForChatQuery = getPokesForChatQuery;
            this.addPokeNotificationCommand = addPokeNotificationCommand;

            this.getPokeQueueQuery = getPokeQueueQuery;
            this.getThrottleQuery = getThrottleQuery;
            this.getAllChatsForArea = getAllChatsForArea;
        }

        public string Name { get { return "NimPokeBot.Announcer2"; } }

        public bool RunExclusive => true;
        public int IntervallInMilliseconds { get { return 10 * 60 * 1000; } }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                await this.SendPokes(cancellationToken).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                await Operator.SendMessage(this.proxy, string.Format("Es ist ein Fehler aufgetreten: {0}", ex.Message)).ConfigureAwait(false);
            }
        }



        private async Task SendPokes(CancellationToken cancellationToken)
        {
            TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneIds.Local);


            var chats = this.activeUsers.Execute(new GetActivePogoGroupsRequest(BotIds: new long[] { this.proxy.BotId, this.raidBotProxy.BotId } ));
            int numberOfCurrentActiveUsers = chats.Count();
            if (numberOfCurrentActiveUsers <= 0)
                return;

            var queue = this.getPokeQueueQuery.Execute(new Queries.GetPokeQueueRequest { });
            var chatsForArea = this.getAllChatsForArea.Execute(new DataAccess.Queries.PoGo.GetAllChatsForAreaRequest { });

            while (!cancellationToken.IsCancellationRequested)
            {
                queue.TryDequeue(out PogoPoke? poke);
                if (poke == null)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false);
                    continue;
                }

                var throttle = this.getThrottleQuery.Execute(new Queries.GetThrottleRequest { });

                //var poke = this.getPokeByIdQuery.Execute(new DataAccess.Queries.Pokes.GetPokeByIdRequest { PokeId = pokeToNotify.PogoPokeId });
                var until = TimeZoneInfo.ConvertTimeFromUtc(poke.Finished, timezone);

                var text1 = $"{EventBot.Models.GoMap.Helper.PokeNames[poke.PokeId].Trim()}";
                var text1Advanced = $"{(poke.Iv.HasValue ? $" {poke.Iv}%" : "")}\r\n{(poke.Level.HasValue ? $" L{poke.Level}" : "")}{(poke.WeatherBoosted.HasValue ? $" {GetWeather(poke.WeatherBoosted)}" : "")}{(poke.Gender.HasValue ? $" {(poke.Gender == 1 ? "M" : (poke.Gender == 2 ? "W" : ""))}" : "")}";
                var text2 = $"{until.ToString("HH:mm:ss")}";
                var text2Advanced = $"{(poke.Cp.HasValue ? $" (CP {poke.Cp})" : "")}";

                var newNotifications = new List<PogoRelPokesChat>();

                foreach (var group in chats.Where(x => x.LatMin <= poke.Latitude && poke.Latitude <= x.LatMax && x.LonMin <= poke.Longitude && poke.Longitude <= x.LonMax))
                {
                    try
                    {
                        if (!chatsForArea.Any(x => x.ChatId == group.ChatId && x.ScanAreaId == poke.MapId))
                            continue;

                        //var poke = this.getPokeQueueQuery.Execute(new Queries.GetPokeQueueRequest { MapId = group.map })

                        var selectedPokes = this.getPokesForChatQuery.Execute(new DataAccess.Queries.Pokes.GetPokesForChatRequest(ChatId: group.ChatId));
                        var toNotify = selectedPokes.SingleOrDefault(x => x.PokeId == poke.PokeId);

                        if (toNotify == null)
                        {
                            if (!group.MinPokeLevel.HasValue || !poke.Iv.HasValue || poke.Iv < group.MinPokeLevel)
                                continue;
                        }
                        else
                        {
                            if (toNotify.Iv.HasValue)
                            {
                                if (!poke.Iv.HasValue)
                                    continue;

                                if (toNotify.Gender != null)
                                {
                                    if (toNotify.Gender == 'm' && poke.Gender.HasValue && poke.Gender == 1)
                                    {
                                        if (poke.Iv < toNotify.Iv)
                                            continue;
                                    }
                                    else
                                    {
                                        if (!group.MinPokeLevel.HasValue || !poke.Iv.HasValue || poke.Iv < group.MinPokeLevel)
                                            continue;
                                    }

                                    if (toNotify.Gender == 'w' && poke.Gender.HasValue && poke.Gender == 2)
                                    {
                                        if (poke.Iv < toNotify.Iv)
                                            continue;
                                    }
                                    else
                                    {
                                        if (!group.MinPokeLevel.HasValue || !poke.Iv.HasValue || poke.Iv < group.MinPokeLevel)
                                            continue;
                                    }
                                }
                                else
                                {
                                    if (poke.Iv < toNotify.Iv)
                                        continue;
                                }
                            }
                            else
                            {
                                if (toNotify.Gender != null)
                                {
                                    if (toNotify.Gender == 'm' && (poke.Gender == 2 || poke.Gender == 0))
                                    {
                                        if (!group.MinPokeLevel.HasValue || !poke.Iv.HasValue || poke.Iv < group.MinPokeLevel)
                                            continue;
                                    }

                                    if (toNotify.Gender == 'w' && (poke.Gender == 1 || poke.Gender == 0))
                                    {
                                        if (!group.MinPokeLevel.HasValue || !poke.Iv.HasValue || poke.Iv < group.MinPokeLevel)
                                            continue;
                                    }
                                }

                            }
                        }

                        var now = DateTime.UtcNow;
                        now = now.AddTicks(-(now.Ticks % TimeSpan.TicksPerSecond));

                        var entry = new Queries.ThroughPut(Ticks: now.Ticks);
                        var update = throttle.AddOrUpdate(group.ChatId, entry, (key, old) => { if (old.Ticks == now.Ticks) { old.Increment(); return old; } else { return entry; } });

                        if (update.Count > 9)
                            continue;

                        var proxy = this.GetProxy(group);
                        var t1 = text1 + (group.MinPokeLevel.HasValue ? text1Advanced : "");
                        var t2 = text2 + (group.MinPokeLevel.HasValue ? text2Advanced : "");

                        var msg = await proxy.SendVenueAsync(group.ChatId, (float)poke.Latitude, (float)poke.Longitude, t1, t2).ConfigureAwait(false);

                        var newNOtification = new PogoRelPokesChat { ChatId = msg.Chat.Id, MessageId = msg.MessageId, PokeId = poke.Id };
                        newNotifications.Add(newNOtification);
                    }
                    catch (Exception ex)
                    {
                        await Operator.SendMessage(this.proxy, string.Format("NimRaidBot: Es ist ein Fehler für Gruppe {1} aufgetreten: {0}", ex.Message, group.ChatId)).ConfigureAwait(false);
                    }
                }

                if (newNotifications.Count() > 0)
                {
                    this.addPokeNotificationCommand.Execute(new DataAccess.Commands.Pokes.AddPokeNotificationRequest(Notifications: newNotifications));
                }

                var isProcessed = this.markAsProcessedQuery.Execute(new DataAccess.Queries.Pokes.MarkAsProcessedRequest(Id: poke.Id));

                if (!isProcessed)
                {
                    await Operator.SendMessage(this.proxy, $"NimPokeBot: Nachricht konnte nicht auf processed gesetzt werden für {poke.Id}, {poke.PokeId} LAT: {poke.Latitude} LONG: {poke.Longitude} IV: {poke.Iv} Finished: {poke.Finished} Map: {poke.MapId}.").ConfigureAwait(false);
                }
            }
        }

        private TelegramBotClient GetProxy(DataAccess.Queries.Raid.PogoRaidUserEx chat)
        {
            if (this.proxy.BotId == chat.BotId)
                return this.proxy;

            if (this.raidBotProxy.BotId == chat.BotId)
                return this.raidBotProxy;

            return this.raidBotProxy;
        }

        const string CLEAR = "\u2600";
        const string RAIN = "\U0001F327";
        const string PARTLY_CLOUDY = "\U0001F324";
        const string CLOUDY = "\u2601";
        const string WINDY = "\U0001F32A";
        const string SNOW = "\u2744";
        const string FOG = "\U0001F32B";

        private string GetWeather(int? weather)
        {
            if (!weather.HasValue)
                return string.Empty;

            switch (weather.Value)
            {
                case 1:
                    return CLEAR;
                case 2:
                    return RAIN;
                case 3:
                    return PARTLY_CLOUDY;
                case 4:
                    return CLOUDY;
                case 5:
                    return WINDY;
                case 6:
                    return SNOW;
                case 7:
                    return FOG;

                default:
                    return "";
            }
        }
    }
}
