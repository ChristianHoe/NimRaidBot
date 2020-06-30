using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public class GetGymsRequest
    {
    }

    public interface IGetGymsQuery : IQuery<GetGymsRequest, IEnumerable<PogoGyms>>
    {
    }

    public class GetGyms : IGetGymsQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetGyms(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoGyms> Execute(GetGymsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoGyms.ToList();
            }
        }
    }
}
