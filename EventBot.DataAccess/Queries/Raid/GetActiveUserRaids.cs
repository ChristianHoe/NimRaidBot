using EventBot.DataAccess.Database;
using System;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public record GetActiveUserRaidsRequest(
        long UserId
    );

    public interface IGetActiveUserRaids : IQuery<GetActiveUserRaidsRequest, int[]>
    {
    }

    public class GetActiveUserRaids : IGetActiveUserRaids
    {
        readonly DatabaseFactory databaseFactory;

        public GetActiveUserRaids(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public int[] Execute(GetActiveUserRaidsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoRaids.Where(x => x.OwnerId == request.UserId  && x.Finished <= DateTime.UtcNow.AddMinutes(5)).Select(x => x.Id).ToArray();
            }
        }
    }
}
