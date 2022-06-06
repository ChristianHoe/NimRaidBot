using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Minun
{
    public sealed record GetActiveUsersRequest(
        long BotId
    );

    public interface IGetActiveUsers : IQuery<GetActiveUsersRequest, IEnumerable<PogoUser>>
    {
    }

    public sealed class GetActiveUsers : IGetActiveUsers
    {
        readonly DatabaseFactory databaseFactory;

        public GetActiveUsers(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoUser> Execute(GetActiveUsersRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoUsers.Where(x => x.Active)
                    .Join(db.RelChatBots.Where(x => x.BotId == request.BotId),
                    u => u.UserId,
                    b => b.ChatId,
                    (u, b) => u
                    )
                    .ToList();
            }
        }
    }
}
