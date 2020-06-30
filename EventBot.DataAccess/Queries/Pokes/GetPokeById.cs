using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries.Pokes
{
    public class GetPokeByIdRequest
    {
        public int PokeId;
    }

    public interface IGetPokeByIdQuery : IQuery<GetPokeByIdRequest, PogoPokes>
    {
    }

    public class GetPokeById : IGetPokeByIdQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetPokeById(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public PogoPokes Execute(GetPokeByIdRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoPokes.SingleOrDefault(x => x.Id == request.PokeId);
            }
        }
    }
}
