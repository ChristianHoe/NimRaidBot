using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public class GetCurrentManualRaidRequest
    {
        public long UserId;
    }

    public interface IGetCurrentManualRaidQuery : IQuery<GetCurrentManualRaidRequest, PogoUserRaids>
    {
    }

    public class GetCurrentManualRaid : IGetCurrentManualRaidQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetCurrentManualRaid(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public PogoUserRaids Execute(GetCurrentManualRaidRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoUserRaids.SingleOrDefault(x => x.UserId == request.UserId);
            }
        }
    }
}
