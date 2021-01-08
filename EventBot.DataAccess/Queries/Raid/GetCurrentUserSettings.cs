using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public record GetCurrentUserSettingsRequest(
        long UserId
    );

    public interface IGetCurrentUserSettingsQuery : IQuery<GetCurrentUserSettingsRequest, PogoUser>
    {
    }

    public class GetCurrentUserSettings : IGetCurrentUserSettingsQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetCurrentUserSettings(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public PogoUser Execute(GetCurrentUserSettingsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoUser.SingleOrDefault(x => x.UserId == request.UserId);
            }
        }
    }
}
