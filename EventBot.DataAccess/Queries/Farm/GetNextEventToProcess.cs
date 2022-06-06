using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries.Farm
{
    public sealed record GetNextEventToProcessRequest();

    public interface IGetNextEventToProcessQuery : IQuery<GetNextEventToProcessRequest, IngrEvent?>
    {
    }

    public sealed class GetNextEventToProcess : IGetNextEventToProcessQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetNextEventToProcess(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IngrEvent? Execute(GetNextEventToProcessRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.IngrEvents
                    .Join(db.IngrEventsMeta.Where(x => x.Farm == null),
                    p => p.Id,
                    m => m.EventId,
                    (p, m) => p
                    ).FirstOrDefault();
            }
        }
    }
}
