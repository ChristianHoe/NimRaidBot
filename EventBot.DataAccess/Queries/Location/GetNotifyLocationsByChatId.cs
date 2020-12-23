using System;
using System.Collections.Generic;
using System.Linq;
using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;

namespace EventBot.DataAccess.Queries.Location
{
    public class GetNotifyLocationsByChatIdRequest
    {
        public IEnumerable<long> ChatIds;
     }

    public interface IGetNotifyLocationsByChatIdQuery : IQuery<GetNotifyLocationsByChatIdRequest, IEnumerable<NotifyLocation>>
    {
    }

    public class GetNotifyLocationsByChatId : IGetNotifyLocationsByChatIdQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetNotifyLocationsByChatId(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<NotifyLocation> Execute(GetNotifyLocationsByChatIdRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.NotifyLocation
                    .Where(x => request.ChatIds.Contains(x.ChatId))
                    .ToList();
            }
        }
    }
}