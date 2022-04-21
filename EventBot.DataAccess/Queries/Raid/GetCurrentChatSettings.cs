using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public record GetCurrentChatSettingsRequest(
        long ChatId
    );

    public interface IGetCurrentChatSettingsQuery    : IQuery<GetCurrentChatSettingsRequest, PogoRaidUser?>
    {
    }

    public class GetCurrentChatSettings : IGetCurrentChatSettingsQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetCurrentChatSettings(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public PogoRaidUser? Execute(GetCurrentChatSettingsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoRaidUsers.SingleOrDefault(x => x.ChatId == request.ChatId);
            }
        }
    }
}
