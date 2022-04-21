using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries.Farm
{
    public record EventLocationByIdRequest(
        long EventId
    );

    public interface IEventLocationByIdQuery : IQuery<EventLocationByIdRequest, DataAccess.Models.Location>
    {
    }

    public class EventLocationById : IEventLocationByIdQuery
    {
        readonly DatabaseFactory databaseFactory;

        public EventLocationById(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public DataAccess.Models.Location Execute(EventLocationByIdRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.Locations.SingleOrDefault(x => x.Id == request.EventId);
            }
        }
    }
}
