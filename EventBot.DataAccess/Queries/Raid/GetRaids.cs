using EventBot.DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace EventBot.DataAccess.Queries.Raid
{
//     public class GetGetRaidsRequest
//     {
//         public decimal LON_MIN;
//         public decimal LON_MAX;
//         public decimal LAT_MIN;
//         public decimal LAT_MAX;
//         public int RAID_LEVEL;
//         public int ID;
//     }

//     public interface IGetRaidsQuery : IQuery<GetGetRaidsRequest, IEnumerable<Raid>>
//     {
//     }

    // public class Raid
    // {
    //     public int Id;
    //     public decimal Latitude;
    //     public decimal Longitude;
    //     public int PokeId;
    //     public string GymName;
    //     public int GymId;
    //     public DateTime? Start;
    //     public DateTime Until;
    //     public int Level;
    //     // public int OldId;
    //     public long? ChatId;
    //     public int? MoveId;
    //     public long? Owner;
    // }

//     public sealed class GetRaids : IGetRaidsQuery
//     {
//         readonly DatabaseFactory databaseFactory;

//         public GetRaids(DatabaseFactory databaseFactory)
//         {
//             this.databaseFactory = databaseFactory;
//         }


//         public IEnumerable<Raid> Execute(GetGetRaidsRequest request)
//         {
//             using (var db = databaseFactory.CreateNew())
//             {
//                 var result = new List<Raid>();

//                 using (var conn = db.Database.GetDbConnection())
//                 {
//                     //await conn.OpenAsync();
//                     conn.Open();
//                     using (var command = conn.CreateCommand())
//                     {
//                         string query =

// @"SELECT CUR.*, OLD.ID, GYM.* 
// FROM POGO_RAIDS CUR
// INNER JOIN POGO_GYMS GYM ON CUR.GYM_ID = GYM.ID
// LEFT OUTER JOIN POGO_RAIDS OLD ON CUR.GYM_ID = OLD.GYM_ID AND CUR.START = OLD.START AND CUR.POKE_ID != OLD.POKE_ID
// WHERE CUR.ID > @p0 AND CUR.LEVEL >= @p1 AND GYM.LATITUDE >= @p2 AND GYM.LATITUDE <= @p3 AND GYM.LONGITUDE >= @p4 AND GYM.LONGITUDE <= @p5
// ORDER BY CUR.START";

//                         command.CommandText = query;
//                         command.Parameters.Add(new MySqlParameter("@p0", request.ID));
//                         command.Parameters.Add(new MySqlParameter("@p1", request.RAID_LEVEL));
//                         command.Parameters.Add(new MySqlParameter("@p2", request.LAT_MIN));
//                         command.Parameters.Add(new MySqlParameter("@p3", request.LAT_MAX));
//                         command.Parameters.Add(new MySqlParameter("@p4", request.LON_MIN));
//                         command.Parameters.Add(new MySqlParameter("@p5", request.LON_MAX));

//                         //DbDataReader reader = await command.ExecuteReaderAsync();
//                         using (DbDataReader reader = command.ExecuteReader())
//                         {
//                             if (reader.HasRows)
//                             {
//                                 //while (await reader.ReadAsync())
//                                 while (reader.Read())
//                                 {
//                                     var row = new Raid { Id = reader.GetInt32(0), Start = reader.GetDateTime(2), Until = reader.GetDateTime(3), PokeId = reader.GetInt32(4), Level = reader.GetInt32(5), ChatId = reader.IsDBNull(6) ? (long?)null : reader.GetInt64(6), MoveId = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7), OldId = reader.IsDBNull(8) ? 0 : reader.GetInt32(8), GymId = reader.GetInt32(9), Latitude = reader.GetDecimal(11), Longitude = reader.GetDecimal(12), GymName = reader.GetString(13) };
//                                     result.Add(row);
//                                 }
//                             }
//                         }
//                     }
//                 }

//                 return result;


//                 //var z = db.PogoRaids
//                 //    .Join(
//                 //    db.PogoGyms,
//                 //    r => r.GymId,
//                 //    g => g.Id,
//                 //    (r, g) => new Raid { GymName = g.Name, Latitude = g.Latitude, Longitude = g.Longitude, PokeId = r.PokeId, Start = r.Start, Until = r.Finished, Level = r.Level, Id = r.Id, ChatId = r.ChatId })
//                 //    .Where(x => x.Latitude >= request.LAT_MIN && x.Latitude <= request.LAT_MAX && x.Longitude >= request.LON_MIN && x.Longitude <= request.LON_MAX && x.Level >= request.RAID_LEVEL && x.Id > request.ID)
//                 //    .OrderBy(x => x.Until)
//                 //    .ToList();

//                 //foreach (var y in z)
//                 //{
//                 //    if (y.PokeId != 0)
//                 //    {
//                 //        var empty = z.FirstOrDefault(x => x.Id != y.Id && x.Latitude == y.Latitude && x.Longitude == y.Longitude && x.PokeId == 0);

//                 //        if (empty != null)
//                 //            y.OldId = empty.Id;
//                 //    }
//                 //}


//                 //return z;

// //                x => new POGO_RAID_EX
// //                {
// //                    Finished = x.Finished,
// //                    GymId = x.GymId,
// //                    Id = x.Id,
// //                    Level = x.Level,
// //                    PokeId = x.PokeId,
// //                    Start = x.Start,
// //                    ChatId = x.ChatId,
// //                    OLD_ID =
// //db.PogoRaids.Where(y => y.GymId == x.GymId && y.Finished == x.Finished && y.Id != x.Id).Select(y => y.Id).FirstOrDefault()
// //                }
// //                    );


// //                    .Join(
// //                    db.PogoGyms,
// //                    r => r.GymId,
// //                    g => g.Id,
// //                    (r, g) => new Raid { GymName = g.Name, Latitude = g.Latitude, Longitude = g.Longitude, PokeId = r.PokeId, Start = r.Start, Until = r.Finished, Level = r.Level, Id = r.Id, OldId = r.OLD_ID, ChatId = r.ChatId }
// //                    )


//                 //return db.POGO_RAIDS.GroupJoin(
//                 //    db.POGO_RAIDS,
//                 //    l => new { l.FINISHED, l.GYM_ID },
//                 //    r => new { r.FINISHED, r.GYM_ID },
//                 //    (l, r) => new POGO_RAID_EX { FINISHED = l.FINISHED, GYM_ID = l.GYM_ID, ID = l.ID, LEVEL = l.LEVEL, POKE_ID = l.POKE_ID, START = l.START, OLD_ID = r.Select(x => x.ID).FirstOrDefault() }
//                 //    )
//                 //    .Join(
//                 //    db.POGO_GYMS,
//                 //    r => r.GYM_ID,
//                 //    g => g.ID,
//                 //    (r, g) => new Raid { GymName = g.NAME, Latitude = g.LATITUDE, Longitude = g.LONGITUDE, PokeId = r.POKE_ID, Start = r.START, Until = r.FINISHED, Level = r.LEVEL, Id = r.ID, OldId = r.OLD_ID }
//                 //    )
//                 //    .Where(x => x.Latitude >= request.LAT_MIN && x.Latitude <= request.LAT_MAX && x.Longitude >= request.LON_MIN && x.Longitude <= request.LON_MAX && x.Level >= request.RAID_LEVEL && x.Id > request.ID)
//                 //    .OrderBy(x => x.Until)
//                 //    .ToList();


//                 //return db.POGO_RAIDS.Join(
//                 //    db.POGO_GYMS,
//                 //    r => r.GYM_ID,
//                 //    g => g.ID,
//                 //    (r, g) => new Raid { GymName = g.NAME, Latitude = g.LATITUDE, Longitude = g.LONGITUDE, PokeId = r.POKE_ID, Start = r.START, Until = r.FINISHED, Level = r.LEVEL, Id = r.ID }
//                 //    )
//                 //    .Where(x => x.Latitude >= request.LAT_MIN && x.Latitude <= request.LAT_MAX && x.Longitude >= request.LON_MIN && x.Longitude <= request.LON_MAX && x.Level >= request.RAID_LEVEL && x.Id > request.ID)
//                 //    .OrderBy(x => x.Until)
//                 //    .ToList();
//             }
//         }
//     }
}
