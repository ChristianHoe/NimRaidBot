using System.Collections.Generic;
using System.Linq;
using EventBot.DataAccess.Database;
using EventBot.DataAccess.ModelsEx;

namespace EventBot.DataAccess.Queries.Raid
{
    public record GetQuestsRequest(
    );

    public interface IGetQuestsQuery : IQuery<GetQuestsRequest, IList<Quest>>
    {
    }

    public class GetQuests : IGetQuestsQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetQuests(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IList<Quest> Execute(GetQuestsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoStops.GroupJoin(
                        db.PogoQuests,
                        s => s.Id,
                        q => q.StopId,
                        (s, qs) => new { Stop = s, Quests = qs })
                    .SelectMany(
                        x => x.Quests.DefaultIfEmpty(),
                        (g, q) => new Quest { Reward = q.Reward, Task = q.Task, Latitude = g.Stop.Latitude, Longitude = g.Stop.Longitude, StopName = g.Stop.Name, Id = g.Stop.Id })
                    .ToList();
            }
        }
    }
}