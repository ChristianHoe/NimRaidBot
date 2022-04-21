using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public record GetActivePogoGroupsRequest(
        long[] BotIds
    );

    public interface IGetActivePogoGroups : IQuery<GetActivePogoGroupsRequest, IEnumerable<PogoRaidUserEx>>
    {
    }

    public class PogoRaidUserEx : PogoRaidUser
    {
        public long BotId;
    }

    public class GetActivePogoGroups : IGetActivePogoGroups
    {
        readonly DatabaseFactory databaseFactory;

        public GetActivePogoGroups(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoRaidUserEx> Execute(GetActivePogoGroupsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoRaidUsers
                    .Where(x => x.Active == true && x.Ingress == false)
                    .Join(db.RelChatBots.Where(y => request.BotIds.Contains(y.BotId)), 
                        c => c.ChatId,
                        b => b.ChatId,
                        (c, b) => new PogoRaidUserEx { Active = c.Active, ChatId = c.ChatId, CleanUp = c.CleanUp, Ingress = c.Ingress, LatMax = c.LatMax, LatMin = c.LatMin, LonMax = c.LonMax, LonMin = c.LonMin, MinPokeLevel = c.MinPokeLevel, Name = c.Name, RaidLevel = c.RaidLevel, TimeOffsetId = c.TimeOffsetId, BotId = b.BotId }
                        )
                    .ToList();
            }
        }
    }
}
