//using EventBot.DataAccess.Database;
//using EventBot.DataAccess.Models;
//using System.Collections.Generic;
//using System.Linq;

//namespace EventBot.DataAccess.Queries.Minun
//{
//    public class GetPokesRequest
//    {
//        //public decimal LatMin;
//        //public decimal LatMax;
//        //public decimal LonMin;
//        //public decimal LonMax;
//        //public int[] PokesIds;
//    }

//    public interface IGetPokes : IQuery<GetPokesRequest, IEnumerable<PogoPokes>>
//    {
//    }

//    public class GetPokes : IGetPokes
//    {
//        readonly DatabaseFactory databaseFactory;

//        public GetPokes(DatabaseFactory databaseFactory)
//        {
//            this.databaseFactory = databaseFactory;
//        }


//        public IEnumerable<PogoPokes> Execute(GetPokesRequest request)
//        {
//            using (var db = databaseFactory.CreateNew())
//            {
//                return db.PogoPokes.Where(x => /*request.PokesIds.Contains(x.POKE_ID) && request.LatMin <= x.LATITUDE && x.LATITUDE <= request.LatMax && request.LonMin <= x.LONGITUDE && x.LONGITUDE <= request.LonMax &&*/ x.Minun == false).ToList();
//            }
//        }
//    }
//}
