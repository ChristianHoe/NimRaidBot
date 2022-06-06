using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Minun
{
    public sealed record GetCurrentGamesRequest(
        DateTime Until
    );

    public interface IGetCurrentGamesQuery : IQuery<GetCurrentGamesRequest, IEnumerable<PogoGamePoke>>
    {
    }

    public sealed class GetCurrentGames : IGetCurrentGamesQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetCurrentGames(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoGamePoke> Execute(GetCurrentGamesRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoGamePokes.Where(x => x.Finish >= request.Until).ToList();
            }
        }
    }
}