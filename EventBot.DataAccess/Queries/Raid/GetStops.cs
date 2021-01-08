using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public record GetStopsRequest
    {
    }

    public interface IGetStopsQuery : IQuery<GetStopsRequest, IEnumerable<PogoStops>>
    {
    }

    public class GetStops : IGetStopsQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetStops(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoStops> Execute(GetStopsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoStops.ToList();
            }
        }
    }
}