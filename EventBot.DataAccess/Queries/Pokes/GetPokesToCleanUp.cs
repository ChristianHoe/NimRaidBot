using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Pokes
{
    public record GetPokesToCleanUpRequest(
        long ChatId,
        DateTime ExpiredBefore
    );

    public interface IGetPokesToCleanUpQuery : IQuery<GetPokesToCleanUpRequest, IEnumerable<PogoRelPokesChats>>
    {
    }

    public class GetPokesToCleanUp : IGetPokesToCleanUpQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetPokesToCleanUp(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoRelPokesChats> Execute(GetPokesToCleanUpRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoRelPokesChats.Join(
                    db.PogoPokes,
                    c => c.PokeId,
                    p => p.Id,
                    (c, p) => new { Chat = c, Until = p.Finished }
                    )
                    .Where(x => x.Chat.ChatId == request.ChatId && x.Until < request.ExpiredBefore && x.Chat.Deleted != true)
                    .Select(x => x.Chat)
                    .ToList();
            }
        }
    }
}
