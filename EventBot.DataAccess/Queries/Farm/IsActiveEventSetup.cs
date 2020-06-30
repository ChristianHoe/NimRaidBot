﻿using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Queries.Farm
{
    public class IsActiveEventSetupRequest
    {
        public long ChatId;
        public long MessageId;
    }

    public interface IIsActiveEventSetupQuery : IQuery<IsActiveEventSetupRequest, bool>
    {
    }

    public class IsActiveEventSetup : IIsActiveEventSetupQuery
    {
        readonly DatabaseFactory databaseFactory;

        public IsActiveEventSetup(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public bool Execute(IsActiveEventSetupRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.EventSetups.Any(x => x.ChatId == request.ChatId && x.MessageId == request.MessageId);
            }
        }
    }
}
