using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using EventBot.Models.RocketMap;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public class AddRocketMapGymsRequest
    {
        public IEnumerable<PokeMapGym> Gyms;
    }

    public interface IAddRocketMapGymsCommand : ICommand<AddRocketMapGymsRequest>
    {
    }

    public class AddRocketMapGymsCommand : IAddRocketMapGymsCommand
    {
        readonly DatabaseFactory databaseFactory;

        public AddRocketMapGymsCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(AddRocketMapGymsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var gyms = request.Gyms.Select(x => new PogoGyms { Latitude = Math.Round(x.latitude, 4), Longitude = Math.Round(x.longitude, 4), Name = x.name.Length > 200 ? x.name.Substring(0, 200) : x.name }).ToList();

                db.PogoGyms.AddRange(gyms);
                db.SaveChanges();
            }
        }
    }
}
