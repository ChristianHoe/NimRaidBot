using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Pokes
{
    public sealed record GetCurrentPokesRequest(
        int MapId
    );

    public interface IGetCurrentPokesQuery : IQuery<GetCurrentPokesRequest, IEnumerable<PogoPoke>>
    {
    }

    public sealed class GetCurrentPokes : IGetCurrentPokesQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetCurrentPokes(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoPoke> Execute(GetCurrentPokesRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var date = DateTime.UtcNow.AddMinutes(-61);
                return db.PogoPokes.Where(x => x.Finished >= date && x.MapId == request.MapId).ToList();
            }
        }
    }
}
