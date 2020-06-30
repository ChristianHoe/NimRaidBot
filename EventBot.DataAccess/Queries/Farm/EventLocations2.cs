using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Farm
{
    public class EventLocations2Request
    {
    }

    public interface IEventLocations2Query : IQuery<EventLocations2Request, IEnumerable<Locations>>
    {
    }

    public class EventLocations2 : IEventLocations2Query
    {
        readonly DatabaseFactory databaseFactory;

        public EventLocations2(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<Locations> Execute(EventLocations2Request request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.Locations.OrderBy(x => x.Name).ToList();
            }
        }
    }
}
