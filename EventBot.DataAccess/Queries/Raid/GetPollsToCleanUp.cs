using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public sealed record GetPollsToCleanUpRequest(
        long ChatId,
        DateTime ExpiredBefore
    );

    public interface IGetPollsToCleanUpsQuery : IQuery<GetPollsToCleanUpRequest, IEnumerable<ActivePoll>>
    {
    }

    public sealed class GetPollsToCleanUp : IGetPollsToCleanUpsQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetPollsToCleanUp(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<ActivePoll> Execute(GetPollsToCleanUpRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.ActivePolls.Join(
                    db.PogoRaids,
                    p => p.RaidId,
                    r => r.Id,
                    (p, r) => new { Poll = p,  Until = r.Finished }
                    )
                    .Where(x => x.Poll.ChatId == request.ChatId && x.Until < request.ExpiredBefore && x.Poll.Deleted != true)
                    .Select(x => x.Poll)
                    .ToList();
            }
        }
    }
}
