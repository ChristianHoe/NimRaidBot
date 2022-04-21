using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Farm
{
    public record GetEventsRequest(
        int Id,
        long ChatId
    );

    public interface IGetEventsQuery : IQuery<GetEventsRequest, IEnumerable<IngrEvent>>
    {
    }

    public class GetEvents : IGetEventsQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetEvents(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<IngrEvent> Execute(GetEventsRequest request)
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
