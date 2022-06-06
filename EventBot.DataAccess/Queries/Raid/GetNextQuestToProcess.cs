using System.Linq;
using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;

namespace EventBot.DataAccess.Queries.Raid
{
    public sealed record GetNextQuestToProcessRequest
    {
    }

    public interface IGetNextQuestToProcessQuery : IQuery<GetNextQuestToProcessRequest, PogoQuest?>
    {
    }

    public sealed class GetNextQuestToProcess : IGetNextQuestToProcessQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetNextQuestToProcess(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public PogoQuest? Execute(GetNextQuestToProcessRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoQuests
                    .Join(db.PogoQuestsMeta.Where(x => x.Processed == null),
                    q => q.StopId,
                    m => m.StopId,
                    (p, m) => p
                    ).FirstOrDefault();
            }
        }
    }
}