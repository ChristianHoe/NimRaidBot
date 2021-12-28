using EventBot.Business.Helper;
using EventBot.Business.Queries;
using EventBot.Business.Tasks.NimGoMapBot.Configurations;
using EventBot.Business.TelegramProxies;
using EventBot.DataAccess.Commands.Pokes;
using EventBot.DataAccess.Commands.Raid;
using EventBot.DataAccess.Models;
using EventBot.DataAccess.ModelsEx;
using EventBot.DataAccess.Queries.Pokes;
using EventBot.DataAccess.Queries.Raid;
using EventBot.DataAccess.Queries.Scan;
using EventBot.Models.RocketMap;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace EventBot.Business.Tasks.NimGoMapBot
{
    public class RocketMapImporter<T> : IScheduledTask where T : MapConfiguration
    {
        private T Configuration { get; }
        string IScheduledTask.Name => Configuration.Name;
        bool IScheduledTask.RunExclusive => Configuration.RunExclusive;
        int IScheduledTask.IntervallInMilliseconds => Configuration.IntervallInMilliseconds;


        private class GymMap
        {
            public PokeMapGym GoMapGym;
            public PogoGyms PogoGym;
        }

        private class StopMap
        {
            public PokeMapStop GoMapStop;
            public Quest? Quest;
        }

        protected long start = 0;

        private readonly TelegramBotClient proxy;
        private readonly IAddPokesCommand addPokesCommand;
        private readonly IUpdatePokesCommand updatePokesCommand;

        private readonly IGetActiveAreas getActiveAreas;

        private readonly IGetGymsQuery getGyms;
        private readonly IGetAllRaidsQuery getRaids;

        private readonly IAddRocketMapGymsCommand addGoMapGymsCommand;
        private readonly IClearRaidsCommand clearRaidsCommand;
        private readonly IAddRaidsCommand addRaidsCommand;
        private readonly IUpdateRaidsCommand updateRaidsCommand;

        private readonly Queries.IGetCurrentPokesQuery getCurrentPokesQuery2;
        private readonly Queries.IGetPokeQueueQuery getPokeQueueQuery;

        private readonly DataAccess.Queries.Base.IGetRocketMapMovesQuery getRocketMapMovesQuery;

        private readonly DataAccess.Queries.Raid.IGetStopsQuery getStopsQuery;
        private readonly IAddRocketMapStopsCommand addRocketMapStopsCommand;

        private readonly IGetAllQuestsQuery getAllQuestsQuery;
        private readonly IAddQuestsCommand addQuestsCommand;
        private readonly IUpdateQuestsCommand updateQuestsCommand;

        private readonly IGetCurrentQuestsQuery getCurrentQuestsQuery;


        private HttpClient httpClient;

        public RocketMapImporter(
            T configuration,
            TelegramProxies.NimRaidBot proxy,
            IAddPokesCommand addPokesCommand,
            IUpdatePokesCommand updatePokesCommand,
            IGetActiveAreas getActiveAreas,
            IGetGymsQuery getGymsQuery,
            IGetAllRaidsQuery getAllRaidsQuery,
            IAddRocketMapGymsCommand addGoMapGymsCommand,
            IClearRaidsCommand clearRaidsCommand,
            IAddRaidsCommand addRaidsCommand,
            IUpdateRaidsCommand updateRaidsCommand,
            Queries.IGetCurrentPokesQuery getCurrentPokesQuery2,
            Queries.IGetPokeQueueQuery getPokeQueueQuery,
            DataAccess.Queries.Base.IGetRocketMapMovesQuery getRocketMapMovesQuery,
            IGetStopsQuery getStopsQuery,
            IAddRocketMapStopsCommand addRocketMapStopsCommand,
            IGetAllQuestsQuery getAllQuestsQuery,
            IAddQuestsCommand addQuestsCommand,
            IUpdateQuestsCommand updateQuestsCommand,
            IGetCurrentQuestsQuery getCurrentQuestsQuery
            )
        {
            this.Configuration = configuration;
            this.proxy = proxy;
            this.addPokesCommand = addPokesCommand;
            this.updatePokesCommand = updatePokesCommand;

            this.getActiveAreas = getActiveAreas;

            this.getGyms = getGymsQuery;
            this.getRaids = getAllRaidsQuery;
            this.addGoMapGymsCommand = addGoMapGymsCommand;
            this.clearRaidsCommand = clearRaidsCommand;
            this.addRaidsCommand = addRaidsCommand;
            this.updateRaidsCommand = updateRaidsCommand;

            this.getCurrentPokesQuery2 = getCurrentPokesQuery2;
            this.getPokeQueueQuery = getPokeQueueQuery;

            this.getRocketMapMovesQuery = getRocketMapMovesQuery;

            this.getStopsQuery = getStopsQuery;
            this.addRocketMapStopsCommand = addRocketMapStopsCommand;

            this.getAllQuestsQuery = getAllQuestsQuery;
            this.addQuestsCommand = addQuestsCommand;
            this.updateQuestsCommand = updateQuestsCommand;
            this.getCurrentQuestsQuery = getCurrentQuestsQuery;

            this.httpClient = new HttpClient();
            this.Configuration.SetCredentials(httpClient);
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                var areas = this.getActiveAreas.Execute(new GetActiveAreasRequest(MapId: this.Configuration.MAP_ID));
                if (areas.Count() <= 0)
                    return;

                var now = DateTimeOffset.UtcNow.AddSeconds(-30).ToUnixTimeMilliseconds(); // GO MAP delivers sometimes obsolete data

                if (start == 0)
                    start = now;

                foreach (var area in areas)
                {
                    var west = (area.LonMin ?? 0).ToString("0.######", CultureInfo.InvariantCulture);
                    var east = (area.LonMax ?? 0).ToString("0.######", CultureInfo.InvariantCulture);
                    var north = (area.LatMax ?? 0).ToString("0.######", CultureInfo.InvariantCulture);
                    var south = (area.LatMin ?? 0).ToString("0.######", CultureInfo.InvariantCulture);

                    string url = $"{this.Configuration.Url}raw_data?userAuthCode=&timestamp={now}{this.Configuration.UrlParameter}&swLat={south}&swLng={west}&neLat={north}&neLng={east}&oSwLat={south}&oSwLng={west}&oNeLat={north}&oNeLng={east}&reids=&eids=&_={start}";

                    start++;


                    Pokes2? result = await JsonSerializer.DeserializeAsync<Pokes2>(await httpClient.GetStreamAsync(url));
                    
                    // TODO: leere Quests gar nicht erst importieren
                    foreach(var pokestop in result.pokestops)
                    {
                        if (pokestop.Value.quest_raw == null)
                            continue;

                        if (!pokestop.Value.quest_raw.is_quest)
                            pokestop.Value.quest_raw = null;
                    }

                    if (result.pokemons == null)
                        result.pokemons = new PokeMapPoke[0];

                    if (result.gyms == null)
                        result.gyms = new Dictionary<string, PokeMapGym>();

                    var distinctPokes = result.pokemons.GroupBy(g => new { latitude = Math.Round(g.latitude, 7), longitude = Math.Round(g.longitude, 7) }).Select(g => g.OrderByDescending(y => y.last_modified).First());

                    var knownPokes2 = this.getCurrentPokesQuery2.Execute(new Queries.GetCurrentPokesRequest(MapId: this.Configuration.MAP_ID));
                    var pokeQueue = this.getPokeQueueQuery.Execute(new Queries.GetPokeQueueRequest { });

                    var newPokes = new List<PogoPokes>();
                    var updatedPokes = new List<PogoPokes>();

                    foreach (var poke in distinctPokes)
                    {
                        int? iiv = (poke.individual_attack.HasValue && poke.individual_defense.HasValue && poke.individual_stamina.HasValue) ? poke.individual_attack + poke.individual_defense + poke.individual_stamina : null;
                        int? iv = iiv == null ? (int?)null : (int)((iiv.Value / 45d) * 100);
                        int? level = CalculateLevel(poke.cp_multiplier);

                        var databasePoke = new PogoPokes { MapId = this.Configuration.MAP_ID, Finished = DateTimeOffset.FromUnixTimeMilliseconds(poke.disappear_time).UtcDateTime, Iv = iv, Cp = poke.cp, Gender = poke.gender, Level = level, Latitude = Math.Round(poke.latitude, 7), Longitude = Math.Round(poke.longitude, 7), /* Modified = true, */ PokeId = poke.pokemon_id, WeatherBoosted = poke.weather_boosted_condition };
                        var hash = Models.Utils.S2.GetPokeCell(databasePoke);
                        knownPokes2.TryGetValue(hash, out var known);

                        if (known == null || databasePoke.Finished > known.Finished)
                        {
                            var updated = knownPokes2.AddOrUpdate(hash, databasePoke, (key, old) => databasePoke.Finished > old.Finished ? databasePoke : old);
                            if (updated == databasePoke)
                            {
                                newPokes.Add(databasePoke);
                            }
                        }
                        else
                        {
                            if (iv.HasValue && known.Iv != iv)
                            {
                                databasePoke.Id = known.Id;
                                var updated = knownPokes2.AddOrUpdate(hash, databasePoke, (key, old) => databasePoke.Finished == old.Finished ? databasePoke : old);
                                if (updated == databasePoke)
                                {
                                    updatedPokes.Add(databasePoke);
                                }

                                if (known.Iv.HasValue)
                                {
                                    await Operator.SendMessage(this.proxy, $"UPDATE: ALT: ID :{known.Id}, {known.Finished}, POKE: {known.PokeId} LON: {known.Longitude}, LAT: {known.Latitude}, CP: {known.Cp} IV: {known.Iv} \r\n NEU: ID :{databasePoke.Id}, {databasePoke.Finished}, POKE: {databasePoke.PokeId} LON: {databasePoke.Longitude}, LAT: {databasePoke.Latitude}, CP: {databasePoke.Cp} IV: {databasePoke.Iv} ").ConfigureAwait(false);
                                }
                            }
                        }
                    }

                    if (newPokes.Count > 0)
                    {
                        this.addPokesCommand.Execute(new AddPokesRequest(Pokes: newPokes));
                        foreach (var poke in newPokes)
                        {
                            pokeQueue.Enqueue(poke);
                        }
                    }

                    if (updatedPokes.Count > 0)
                    {
                        this.updatePokesCommand.Execute(new UpdatePokesRequest(Pokes: updatedPokes));
                        foreach (var poke in updatedPokes)
                        {
                            pokeQueue.Enqueue(poke);
                        }
                    }


                    // filter new gyms
                    if (result.gyms != null)
                    {
                        var gyms = this.getGyms.Execute(new DataAccess.Queries.Raid.GetGymsRequest { });

                        var mappedGyms = new List<GymMap>();
                        foreach (var gym in result.gyms.Values)
                        {
                            var knownGym = gyms.FirstOrDefault(x => Math.Round(x.Latitude, 4) == Math.Round(gym.latitude, 4) && Math.Round(x.Longitude, 4) == Math.Round(gym.longitude, 4));
                            mappedGyms.Add(new GymMap { GoMapGym = gym, PogoGym = knownGym });
                        }

                        var newGyms = mappedGyms.Where(x => x.PogoGym == null && !string.IsNullOrWhiteSpace(x.GoMapGym.name)).ToList();
                        if (newGyms.Count() > 0)
                        {
                            this.addGoMapGymsCommand.Execute(new AddRocketMapGymsRequest(Gyms: newGyms.Select(x => x.GoMapGym).ToList()));

                            gyms = this.getGyms.Execute(new GetGymsRequest { });

                            foreach (var gym in mappedGyms.Where(x => x.PogoGym == null))
                            {
                                var knownGym = gyms.FirstOrDefault(x => x.Latitude == (decimal)gym.GoMapGym.latitude && x.Longitude == (decimal)gym.GoMapGym.longitude);
                                gym.PogoGym = knownGym;
                            }
                        }

                        // update raids
                        //this.clearRaidsCommand.Execute(new ClearRaidsRequest());

                        var utc = DateTime.UtcNow;

                        var knownRaids = this.getRaids.Execute(new GetAllRaidsRequest(Date: utc));
                        var currentRaids = mappedGyms.Where(x => x.GoMapGym.raid != null && x.PogoGym != null && x.GoMapGym.raid?.level > 0 && Models.GoMap.Helper.TimeWithoutMilliseconds(Models.GoMap.Helper.PokeMapTimeToUtc(x.GoMapGym.raid.end)) >= Models.GoMap.Helper.TimeWithoutMilliseconds(utc)).ToList();

                        var newRaids = new List<PogoRaids>();
                        var updateRaids = new List<PogoRaids>();
                        foreach (var gym in currentRaids)
                        {
                            var currentRaid = knownRaids.FirstOrDefault(x => x.GymId == gym.PogoGym.Id && Models.GoMap.Helper.TimeWithoutMilliseconds(Models.GoMap.Helper.PokeMapTimeToUtc(gym.GoMapGym.raid.end)) == Models.GoMap.Helper.TimeWithoutMilliseconds(x.Finished));
                            if (currentRaid == null)
                            {
                                newRaids.Add(new PogoRaids { GymId = gym.PogoGym.Id, Start = Models.GoMap.Helper.TimeWithoutMilliseconds(Models.GoMap.Helper.PokeMapTimeToUtc(gym.GoMapGym.raid.start)), Finished = Models.GoMap.Helper.TimeWithoutMilliseconds(Models.GoMap.Helper.PokeMapTimeToUtc(gym.GoMapGym.raid.end)), Level = gym.GoMapGym.raid.level, PokeId = (gym.GoMapGym.raid.pokemon_id ?? 0), Move2 = gym.GoMapGym.raid.move_2 });
                            }
                            else
                            {
                                if (currentRaid.PokeId != gym.GoMapGym.raid.pokemon_id && (gym.GoMapGym.raid.pokemon_id ?? 0) != 0)
                                {
                                    updateRaids.Add(new PogoRaids { Id = currentRaid.Id, PokeId = (gym.GoMapGym.raid.pokemon_id ?? 0), Move2 = gym.GoMapGym.raid.move_2 });
                                }
                            }
                        }

                        if (newRaids.Count > 0)
                            this.addRaidsCommand.Execute(new AddRaidsRequest(Raids: newRaids));

                        if (updateRaids.Count > 0)
                            this.updateRaidsCommand.Execute(new UpdateRaidsRequest(Raids: updateRaids));
                    }

                    // filter new pokestops/quests
                    if (result.pokestops != null && result.pokestops.Any())
                    {
                        var stopsWithQuests = this.getCurrentQuestsQuery.Execute(new Business.Queries.GetCurrentQuestsRequest {});

                        // merge internal <-> external stops
                        var mappedStops = new List<StopMap>();
                        foreach (var stop in result.pokestops)
                        {
                            // structs are sometimes *bäh*
                            var knownStop = stopsWithQuests.Values
                                .Where(x => Math.Round(x.Latitude, 4) == Math.Round(stop.Value.latitude, 4) && Math.Round(x.Longitude, 4) == Math.Round(stop.Value.longitude, 4))
                                .Cast<Quest?>()
                                .FirstOrDefault();

                            mappedStops.Add(new StopMap { GoMapStop = stop.Value, Quest = knownStop });
                        }

                        // update stops
                        var newStops = mappedStops.Where(x => x.Quest == null && !string.IsNullOrWhiteSpace(x.GoMapStop.name)).ToList();
                        // newStops.Add(new StopMap {GoMapStop = new PokeMapStop {name="debug",latitude=0.0m,longitude=0.0m}});
                        var remberId = 0;
                        if (newStops.Count() > 0)
                        {
                            var request = new AddRocketMapStopsRequest(Stops: newStops.Select(x => new PogoStops{ Latitude = x.GoMapStop.latitude, Longitude = x.GoMapStop.longitude, Name = x.GoMapStop.name }).ToList());
                            this.addRocketMapStopsCommand.Execute(request);

                            foreach(var newStop in request.Stops)
                            {
                                var quest = new Quest { Id = newStop.Id, Latitude = newStop.Latitude, Longitude = newStop.Longitude, StopName = newStop.Name };
                                stopsWithQuests.TryAdd(newStop.Id, quest);
                                var y = newStops.FirstOrDefault(x => Math.Round(x.GoMapStop.latitude, 4) == Math.Round(newStop.Latitude, 4) && Math.Round(x.GoMapStop.longitude, 4) == Math.Round(newStop.Longitude, 4));
                                y.Quest = quest;
                            }
                            remberId = request.Stops.First().Id;
                        }

                        // update quests
                        var utc = DateTime.UtcNow;

                        var currentQuests = mappedStops.Where(x => x.GoMapStop.quest_raw != null && x.Quest != null).ToList();

                        var newQuests = new List<PogoQuests>();
                        var updateQuests = new List<PogoQuests>();
                        foreach (var quest in currentQuests)
                        {
var reward_text = $"{quest.GoMapStop.quest_raw.item_type} : {(quest.GoMapStop.quest_raw.item_type == "Pokemon" ? quest.GoMapStop.quest_raw.quest_pokemon_name : quest.GoMapStop.quest_raw.item_amount?.ToString())}";

                            var currentQuest = string.IsNullOrWhiteSpace(quest.Quest?.Reward) ? null : quest.Quest;
                            if (currentQuest == null)
                            {
                                newQuests.Add(new PogoQuests { StopId = quest.Quest.Value.Id, Created = utc, Task = quest.GoMapStop.quest_raw.quest_task, Reward = reward_text });
                            }
                            else
                            {
                                if (currentQuest.Value.Task != quest.GoMapStop.quest_raw.quest_task && currentQuest.Value.Reward != reward_text)
                                {
                                    updateQuests.Add(new PogoQuests { StopId = currentQuest.Value.Id, Created = utc, Task = quest.GoMapStop.quest_raw.quest_task, Reward = reward_text });
                                }
                            }
                        }

                        // newQuests.Add(new PogoQuests { StopId = remberId, Created = utc, Task = "tue was", Reward = "erhalt was" });


                        if (newQuests.Count > 0)
                            this.addQuestsCommand.Execute(new AddQuestsRequest(Quests: newQuests));

                        foreach(var quest in newQuests)
                        {
                            var copy = stopsWithQuests.GetValueOrDefault(quest.StopId);
                            var update = new Quest { Id = copy.Id, Latitude = copy.Latitude, Longitude = copy.Longitude, StopName = copy.StopName, Reward = copy.Reward, Task = copy.Task };
                            var old =    new Quest { Id = copy.Id, Latitude = copy.Latitude, Longitude = copy.Longitude, StopName = copy.StopName, Reward = null, Task = null };
                            if (stopsWithQuests.TryUpdate(quest.StopId, update, old))
                            {
                                bool ok = true;
                            }
                            else
                            {
                                bool nok = true;
                            }
                        }


                        if (updateQuests.Count > 0)
                        {
                            // foreach(var tmp in updateQuests)
                            // {
                            //     try
                            //     {
// disabeled                                    this.updateQuestsCommand.Execute(new UpdateQuestsRequest { Quests =  updateQuests });
                            //     }
                            //     catch (Exception ex)
                            //     {
                            //         await Operator.SendMessage(this.proxy, "Quest on Stop " + tmp.StopId + " - ", ex).ConfigureAwait(false);
                            //         await Task.Delay(500, cancellationToken);
                            //     }
                            // }
                        }

                        foreach(var quest in updateQuests)
                        {
                            var copy = stopsWithQuests.GetValueOrDefault(quest.StopId);
                            var old = new Quest { Id = copy.Id, Latitude = copy.Latitude, Longitude = copy.Longitude, StopName = copy.StopName, Reward = copy.Reward, Task = copy.Task };
                            var update =    new Quest { Id = copy.Id, Latitude = copy.Latitude, Longitude = copy.Longitude, StopName = copy.StopName, Reward = quest.Reward, Task = quest.Task };
                            if (stopsWithQuests.TryUpdate(quest.StopId, update, old))
                            {
                                bool ok = true;
                            }
                            else
                            {
                                bool nok = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Operator.SendMessage(this.proxy, this.Configuration.Name + " - ", ex).ConfigureAwait(false);
                if (ex.InnerException != null)
                    await Operator.SendMessage(this.proxy, this.Configuration.Name + " - ", ex.InnerException).ConfigureAwait(false);
                
                await Task.Delay(500, cancellationToken);
            }
        }

        private int? CalculateLevel(double? cp_multiplier)
        {
            if (cp_multiplier == null)
                return null;

            if (cp_multiplier < 0.734)
                return (int)((58.35178527 * cp_multiplier * cp_multiplier) - (2.838007664 * cp_multiplier) + 0.8539209906);

            var tmp = 171.0112688 * cp_multiplier.Value - 95.20425243;
            return (int)((Math.Round(tmp, 0) * 2) / 2);
        }
    }
}
