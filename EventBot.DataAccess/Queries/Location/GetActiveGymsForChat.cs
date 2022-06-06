using System.Collections.Generic;
using System.Linq;
using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;

namespace EventBot.DataAccess.Queries.Location
{
    public sealed record GetActiveGymsForChatRequest(
        IEnumerable<long> ChatIds
    );

    public interface IGetActiveGymsForChatQuery : IQuery<GetActiveGymsForChatRequest, IEnumerable<PogoGym>>
    {
    }

    public sealed class GetActiveGymsForChat : IGetActiveGymsForChatQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetActiveGymsForChat(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoGym> Execute(GetActiveGymsForChatRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.NotifyLocations
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