using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public sealed record GetCurrentUserSettingsRequest(
        long UserId
    );

    public interface IGetCurrentUserSettingsQuery : IQuery<GetCurrentUserSettingsRequest, PogoUser?>
    {
    }

    public sealed class GetCurrentUserSettings : IGetCurrentUserSettingsQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetCurrentUserSettings(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public PogoUser? Execute(GetCurrentUserSettingsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoUsers.SingleOrDefault(x => x.UserId == request.UserId);
            }
        }
    }
}
