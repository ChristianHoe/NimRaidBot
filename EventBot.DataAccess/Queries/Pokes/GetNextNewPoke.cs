using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Pokes
{
    public record GetNextNewPokeRequest
    {
    }

    public interface IGetNextNewPokeQuery : IQuery<GetNextNewPokeRequest, IEnumerable<PogoPokes>>
    {
    }

    public class GetNextNewPoke : IGetNextNewPokeQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetNextNewPoke(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoPokes> Execute(GetNextNewPokeRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoPokesMeta.Where(x => x.Poke == null)
                    .Join(db.PogoPokes,
                    m => m.PogoPokeId,
                    p => p.Id,
                    (m, p) => p)
                    .ToList();
            }
        }
    }
}
