using System.Collections.Generic;
using System.Linq;
using EventBot.DataAccess.Models;
using EventBot.DataAccess.Database;

namespace EventBot.DataAccess.Queries.Raid
{
    public sealed record GetGymsByChatRequest(
        PogoRaidUser Chat
    );

    public interface IGetGymsByChatQuery : IQuery<GetGymsByChatRequest, IEnumerable<PogoGym>>
    {
    }

    public sealed class GetGymsByChat : IGetGymsByChatQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetGymsByChat(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoGym> Execute(GetGymsByChatRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoGyms
                    .Where(g => g.Latitude >= request.Chat.LatMin && g.Latitude <= request.Chat.LatMax && g.Longitude >= request.Chat.LonMin && g.Longitude <= request.Chat.LonMax).OrderBy(x => x.Name).ToList();

                //return
                //db.PogoGyms.GroupJoin(
                //  db.PogoSpecialGyms.Where(x => x.ChatId == request.Chat.ChatId),
                //  foo => foo.Id,
                //  bar => bar.GymId,
                //  (f, bs) => new { Gym = f, Special = bs.SingleOrDefault() })
                //  .Where(g => g.Gym.Latitude >= request.Chat.LatMin && g.Gym.Latitude <= request.Chat.LatMax && g.Gym.Longitude >= request.Chat.LonMin && g.Gym.Longitude <= request.Chat.LonMax && (g.Special != null && g.Special.Exclude != true))
                //  .Select(x => x.Gym)
                //  .OrderBy(x => x.Name).ToList();

            }
        }
    }
}
