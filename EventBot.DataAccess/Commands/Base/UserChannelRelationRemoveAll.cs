﻿using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Base
{
    public sealed record UserChannelRelationRemoveAllRequest(
        long ChatId
    );

    public interface IUserChannelRelationRemoveAllCommand : ICommand<UserChannelRelationRemoveAllRequest>
    {
    }

    public sealed class UserChannelRelationRemoveAll : IUserChannelRelationRemoveAllCommand
    {
        readonly DatabaseFactory databaseFactory;

        public UserChannelRelationRemoveAll(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(UserChannelRelationRemoveAllRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.Memberships.Where(x => x.GroupId == request.ChatId);

                if (result != null)
                {
                    db.Memberships.RemoveRange(result);
                    db.SaveChanges();
                }
            }
        }
    }
}
