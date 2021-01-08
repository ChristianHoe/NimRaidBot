using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Farm
{
    public record EventLocationsRequest(
        string Name
    );

    public interface IEventLocationsQuery : IQuery<EventLocationsRequest, IEnumerable<Locations>>
    {
    }

    public class EventLocations : IEventLocationsQuery
    {
        readonly DatabaseFactory databaseFactory;

        public EventLocations(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<Locations> Execute(EventLocationsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.Locations.FromSqlInterpolated(
$@"
(
SELECT * 
FROM LOCATIONS 
WHERE NAME <  @p0 
ORDER BY NAME DESC 
LIMIT 1 
) 
UNION 
( 
SELECT * 
FROM LOCATIONS 
WHERE NAME >= { request.Name }
ORDER BY NAME ASC 
LIMIT 2 
)").ToList();

                //return db.Locations.ToList().OrderBy(x => x.Id);
            }
        }
    }
}
