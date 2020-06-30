using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventBot.DataAccess.Queries.Farm
{
    public class GetEventsRequest
    {
        public int Id;
        public long ChatId;
    }

    public interface IGetEventsQuery : IQuery<GetEventsRequest, IEnumerable<IngrEvents>>
    {
    }

    public class GetEvents : IGetEventsQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetEvents(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<IngrEvents> Execute(GetEventsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.IngrEvents
                    .Where(x => x.Id > request.Id && (x.ChatId == null || x.ChatId == request.ChatId))
                    .ToList();
            }
        }
    }
}
