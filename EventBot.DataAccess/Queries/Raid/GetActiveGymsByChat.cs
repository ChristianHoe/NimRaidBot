using System.Collections.Generic;
using System.Linq;
using EventBot.DataAccess.Database;
using EventBot.DataAccess.ModelsEx;
using Microsoft.EntityFrameworkCore;

namespace EventBot.DataAccess.Queries.Raid
{
    public class GetActiveGymsByChatRequest
    {
        public long ChatId;
    }

    public interface IGetActiveGymsByChatQuery : IQuery<GetActiveGymsByChatRequest, IEnumerable<PogoGymsExtended>>
    {
    }

    public class GetActiveGymsByChat : IGetActiveGymsByChatQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetActiveGymsByChat(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoGymsExtended> Execute(GetActiveGymsByChatRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoGyms.FromSqlInterpolated(
$@"SELECT g.*
FROM POGO_RAID_USERS c
LEFT JOIN POGO_GYMS g ON ( c.LAT_MIN <= g.LATITUDE AND g.LATITUDE <= c.LAT_MAX AND c.LON_MIN <= g.LONGITUDE AND g.LONGITUDE <= c.LON_MAX  )
LEFT JOIN POGO_SPECIAL_GYMS s ON (g.ID = s.GYM_ID AND s.TYPE = 0) 
WHERE c.CHAT_ID = {request.ChatId}
AND s.TYPE IS NULL"
                ).OrderBy(x => x.Name)
                .Select(x => new PogoGymsExtended { Id = x.Id, Name = x.Name })
                .ToList();
            }
        }
    }
}
