using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries.Pokes
{
    public record GetPokeByIdRequest(
        int PokeId
    );

    public interface IGetPokeByIdQuery : IQuery<GetPokeByIdRequest, PogoPoke?>
    {
    }

    public class GetPokeById : IGetPokeByIdQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetPokeById(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public PogoPoke? Execute(GetPokeByIdRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoPokes.SingleOrDefault(x => x.Id == request.PokeId);
            }
        }
    }
}
