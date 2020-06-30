using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Farm
{
    public class GetActiveGroupsRequest
    {
    }

    public interface IGetActiveGroups : IQuery<GetActiveGroupsRequest, IEnumerable<PogoRaidUsers>>
    {
    }

    public class GetActiveGroups : IGetActiveGroups
    {
        readonly DatabaseFactory databaseFactory;

        public GetActiveGroups(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoRaidUsers> Execute(GetActiveGroupsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoRaidUsers.Where(x =>  x.Active == true && x.Ingress == true).ToList();
            }
        }
    }
}
