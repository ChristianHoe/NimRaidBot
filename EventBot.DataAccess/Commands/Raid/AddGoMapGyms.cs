// using EventBot.DataAccess.Database;
// using EventBot.DataAccess.Models;
// using EventBot.Models.GoMap;
// using System;
// using System.Collections.Generic;
// using System.Linq;

// namespace EventBot.DataAccess.Commands.Raid
// {
//     public class AddGoMapGymsRequest
//     {
//         public IEnumerable<ArenaMapResult> Gyms;
//     }

//     public interface IAddGoMapGymsCommand : ICommand<AddGoMapGymsRequest>
//     {
//     }

//     public class AddGoMapGymsCommand : IAddGoMapGymsCommand
//     {
//         readonly DatabaseFactory databaseFactory;

//         public AddGoMapGymsCommand(DatabaseFactory databaseFactory)
//         {
//             this.databaseFactory = databaseFactory;
//         }


//         public void Execute(AddGoMapGymsRequest request)
//         {
//             using (var db = databaseFactory.CreateNew())
//             {
//                 var gyms = request.Gyms.Select(x => new PogoGyms { Latitude = Math.Round((decimal)x.latitude, 4), Longitude = Math.Round((decimal)x.longitude), Name = x.name.Length > 200 ? x.name.Substring(0, 200) : x.name }).ToList();

//                 db.PogoGyms.AddRange(gyms);
//                 db.SaveChanges();
//             }
//         }
//     }
// }
