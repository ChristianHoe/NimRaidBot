using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Commands.Raid
{
    public record AddRocketMapStopsRequest(
        IEnumerable<PogoStop> Stops
    );

    public interface IAddRocketMapStopsCommand : ICommand<AddRocketMapStopsRequest>
    {
    }

    public class AddRocketMapStopsCommand : IAddRocketMapStopsCommand
    {
        readonly DatabaseFactory databaseFactory;

        public AddRocketMapStopsCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(AddRocketMapStopsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                // carefull, returns created ID implicit
                foreach(var stop in request.Stops)
                {
                    stop.Latitude = Math.Round((decimal)stop.Latitude, 4);
                    stop.Longitude = Math.Round((decimal)stop.Longitude, 4);     
                    stop.Name = stop.Name.Length > 200 ? stop.Name.Substring(0, 200) : stop.Name;             
                }

                db.PogoStops.AddRange(request.Stops);
                db.SaveChanges();
            }
        }
    }
}
