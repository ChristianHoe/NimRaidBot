using System.Collections.Generic;
using System.Linq;
using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;

namespace EventBot.DataAccess.Queries.Location
{
    public class GetActiveGymsForChatRequest
    {
        public IEnumerable<long> ChatIds;
     }

    public interface IGetActiveGymsForChatQuery : IQuery<GetActiveGymsForChatRequest, IEnumerable<PogoGyms>>
    {
    }

    public class GetActiveGymsForChat : IGetActiveGymsForChatQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetActiveGymsForChat(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoGyms> Execute(GetActiveGymsForChatRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.NotifyLocation
                .Where(x => request.ChatIds.Contains(x.ChatId))
                .Join(
                    db.PogoGyms,
                    n => n.LocationId,
                    g => g.Id,
                    (n, g) => g
                )
                .OrderBy(x => x.Name)
                .ToList();
            }
        }
    }
}