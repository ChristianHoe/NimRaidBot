﻿using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System;
using System.Linq;

namespace EventBot.DataAccess.Commands.Base
{
    public class UserChannelRelationAddRequest
    {
        public long UserId;
        public long ChatId;
    }

    public interface IUserChannelRelationAddCommand : ICommand<UserChannelRelationAddRequest>
    {
    }

    public class UserChannelRelationAdd : IUserChannelRelationAddCommand
    {
        readonly DatabaseFactory databaseFactory;

        public UserChannelRelationAdd(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(UserChannelRelationAddRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.Memberships.SingleOrDefault(x => x.UserId == request.UserId && x.GroupId == request.ChatId);

                if (result == null)
                {
                    db.Memberships.Add(new Memberships { UserId = request.UserId, GroupId = request.ChatId, LastAccess = DateTime.UtcNow, Created = DateTime.UtcNow });
                    db.SaveChanges();
                }
            }
        }
    }
}
