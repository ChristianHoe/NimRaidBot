using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Farm
{
    public record EventLocations2Request();

    public interface IEventLocations2Query : IQuery<EventLocations2Request, IEnumerable<Models.Location>>
    {
    }

    public class EventLocations2 : IEventLocations2Query
    {
        readonly DatabaseFactory databaseFactory;

        public EventLocations2(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<Models.Location> Execute(EventLocations2Request request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.Locations.OrderBy(x => x.Name).ToList();
            }
        }
    }
}
