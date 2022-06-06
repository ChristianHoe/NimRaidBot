using System.Linq;
using EventBot.DataAccess.Database;
using EventBot.DataAccess.ModelsEx;

namespace EventBot.DataAccess.Queries.Raid
{
    public sealed record GetQuestByStopIdRequest(
        long StopId
    );

    public interface IGetQuestByStopIdQuery : IQuery<GetQuestByStopIdRequest, Quest>
    {
    }

    public sealed class GetQuestByStopId : IGetQuestByStopIdQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetQuestByStopId(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public Quest Execute(GetQuestByStopIdRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoQuests.Join(
                    db.PogoStops,
                    q => q.StopId,
                    s => s.Id,
                    (q, s) => new Quest { Reward = q.Reward, Task = q.Task, Latitude = s.Latitude, Longitude = s.Longitude, StopName = s.Name, Id = s.Id }
                    )
                    .Where(x => x.Id == request.StopId)
                    .SingleOrDefault();
            }
        }
    }
}