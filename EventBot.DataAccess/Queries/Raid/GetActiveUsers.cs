using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public class GetActivePogoGroupsRequest
    {
        public long[] BotIds;
    }

    public interface IGetActivePogoGroups : IQuery<GetActivePogoGroupsRequest, IEnumerable<PogoRaidUsersEx>>
    {
    }

    public class PogoRaidUsersEx : PogoRaidUsers
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


        public IEnumerable<PogoRaidUsersEx> Execute(GetActivePogoGroupsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoRaidUsers
                    .Where(x => x.Active == true && x.Ingress == false)
                    .Join(db.RelChatBot.Where(y => request.BotIds.Contains(y.BotId)), 
                        c => c.ChatId,
                        b => b.ChatId,
                        (c, b) => new PogoRaidUsersEx { Active = c.Active, ChatId = c.ChatId, CleanUp = c.CleanUp, Ingress = c.Ingress, LatMax = c.LatMax, LatMin = c.LatMin, LonMax = c.LonMax, LonMin = c.LonMin, MinPokeLevel = c.MinPokeLevel, Name = c.Name, RaidLevel = c.RaidLevel, TimeOffsetId = c.TimeOffsetId, BotId = b.BotId }
                        )
                    .ToList();
            }
        }
    }
}
