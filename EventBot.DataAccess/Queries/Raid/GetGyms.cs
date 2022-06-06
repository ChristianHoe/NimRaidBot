using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public sealed record GetGymsRequest
    {
    }

    public interface IGetGymsQuery : IQuery<GetGymsRequest, IEnumerable<PogoGym>>
    {
    }

    public sealed class GetGyms : IGetGymsQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetGyms(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoGym> Execute(GetGymsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoGyms.ToList();
            }
        }
    }
}
