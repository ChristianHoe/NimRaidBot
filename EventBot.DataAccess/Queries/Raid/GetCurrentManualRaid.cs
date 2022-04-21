using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public record GetCurrentManualRaidRequest(
        long UserId
    );

    public interface IGetCurrentManualRaidQuery : IQuery<GetCurrentManualRaidRequest, PogoUserRaid?>
    {
    }

    public class GetCurrentManualRaid : IGetCurrentManualRaidQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetCurrentManualRaid(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public PogoUserRaid? Execute(GetCurrentManualRaidRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoUserRaids.SingleOrDefault(x => x.UserId == request.UserId);
            }
        }
    }
}
