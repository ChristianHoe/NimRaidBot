using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries.Farm
{
    public class EventLocationByIdRequest
    {
        public long EventId;
    }

    public interface IEventLocationByIdQuery : IQuery<EventLocationByIdRequest, Locations>
    {
    }

    public class EventLocationById : IEventLocationByIdQuery
    {
        readonly DatabaseFactory databaseFactory;

        public EventLocationById(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public Locations Execute(EventLocationByIdRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.Locations.SingleOrDefault(x => x.Id == request.EventId);
            }
        }
    }
}
