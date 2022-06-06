//using EventBot.DataAccess.Database;
//using EventBot.DataAccess.Models;
//using System.Collections.Generic;
//using System.Linq;

//namespace EventBot.DataAccess.Queries.Pokes
//{
//    public class GetModifiedPokesdRequest
//    {
//    }

//    public interface IGetModifiedPokesQuery : IQuery<GetModifiedPokesdRequest, IEnumerable<PokeNotification>>
//    {
//    }

//    public class PokeNotification
//    {
//        public PogoPokes Poke;
//        public PogoRelPokesChats Nofication;
//    }

//    public sealed class GetModifiedPokes : IGetModifiedPokesQuery
//    {
//        readonly DatabaseFactory databaseFactory;

//        public GetModifiedPokes(DatabaseFactory databaseFactory)
//        {
//            this.databaseFactory = databaseFactory;
//        }


//        public IEnumerable<PokeNotification> Execute(GetModifiedPokesdRequest request)
//        {
//            using (var db = databaseFactory.CreateNew())
//            {
//                return db.PogoPokes.GroupJoin(
//                  db.PogoRelPokesChats,
//                  p => p.Id,
//                  c => c.PokeId,
//                  (p, c) => new { Poke = p, Chat = c })
//                .SelectMany(
//                  pokes => pokes.Chat.DefaultIfEmpty(),
//                  (x, y) => new PokeNotification { Poke = x.Poke, Nofication = y })
//                .Where(x => x.Poke.Modified == true)
//                .ToList();
//            }
//        }
//    }
//}
