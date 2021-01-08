using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Minun
{
    public record GetRaidBossPreferencesRequest(
        long ChatId
    );

    public interface IGetRaidBossPreferencesQuery : IQuery<GetRaidBossPreferencesRequest, IEnumerable<PogoRaidPreference>>
    {
    }

    public class GetRaidBossPreferences : IGetRaidBossPreferencesQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetRaidBossPreferences(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoRaidPreference> Execute(GetRaidBossPreferencesRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoRaidPreference.Where(x => x.ChatId == request.ChatId).ToList();
            }
        }
    }
}
