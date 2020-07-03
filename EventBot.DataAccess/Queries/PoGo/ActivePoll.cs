﻿using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries.PoGo
{
    public class ActivePollRequest
    {
        public long ChatId;
        public long MessageId;
    }

    public interface IActivePoll : IQuery<ActivePollRequest, ActivePolls>
    {
    }

    public class ActivePoll : IActivePoll
    {
        readonly DatabaseFactory databaseFactory;

        public ActivePoll(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public ActivePolls Execute(ActivePollRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.ActivePolls.SingleOrDefault(x => x.ChatId == request.ChatId && x.MessageId == request.MessageId);
            }
        }
    }
}