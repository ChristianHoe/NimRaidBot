using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public sealed record GetActiveChatsForUserRequest(
        long UserId,
        long? BotId
    );

    public interface IGetActiveChatsForUser : IQuery<GetActiveChatsForUserRequest, PogoRaidUser[]>
    {
    }

    public sealed class GetActiveChatsForUser : IGetActiveChatsForUser
    {
        readonly DatabaseFactory databaseFactory;

        public GetActiveChatsForUser(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public PogoRaidUser[] Execute(GetActiveChatsForUserRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.Memberships.Where(x => x.UserId == request.UserId)
                    .Join(db.PogoRaidUsers,
                    rel => rel.GroupId,
                    c => c.ChatId,
                    (rel, c) => c
                    ).Join(db.RelChatBots.Where(x => x.BotId == request.BotId),
                    c => c.ChatId,
                    b => b.ChatId,
                    (c, b) => c
                    ).OrderBy(x => x.Name).ThenBy(x => x.ChatId).ToArray();
            }
        }
    }
}
