using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventBot.DataAccess.Queries.Base
{
    public class NumberOfBotsInChatRequest
    {
        public long ChatId;
    }

    public interface INumberOfBotsInChatQuery : IQuery<NumberOfBotsInChatRequest, int>
    {
    }

    public class NumberOfBotsInChat : INumberOfBotsInChatQuery
    {
        readonly DatabaseFactory databaseFactory;

        public NumberOfBotsInChat(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public int Execute(NumberOfBotsInChatRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.RelChatBot.Count(x => x.ChatId == request.ChatId);
            }
        }
    }
}
