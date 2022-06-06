using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public sealed record GetActivePollByRaidRequest(
        int RaidId,
        long ChatId
    );

    public interface IGetActivePollByRaidId : IQuery<GetActivePollByRaidRequest, ActivePoll?>
    {
    }

    public sealed class GetActivePollByRaidId : IGetActivePollByRaidId
    {
        readonly DatabaseFactory databaseFactory;

        public GetActivePollByRaidId(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public ActivePoll? Execute(GetActivePollByRaidRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.ActivePolls.SingleOrDefault(x => x.RaidId == request.RaidId && x.ChatId == request.ChatId);
            }
        }
    }
}
