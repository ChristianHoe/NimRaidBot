using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Pokes
{
    public class GetCurrentPokesRequest
    {
        public int MapId;
    }

    public interface IGetCurrentPokesQuery : IQuery<GetCurrentPokesRequest, IEnumerable<PogoPokes>>
    {
    }

    public class GetCurrentPokes : IGetCurrentPokesQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetCurrentPokes(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoPokes> Execute(GetCurrentPokesRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var date = DateTime.UtcNow.AddMinutes(-61);
                return db.PogoPokes.Where(x => x.Finished >= date && x.MapId == request.MapId).ToList();
            }
        }
    }
}
