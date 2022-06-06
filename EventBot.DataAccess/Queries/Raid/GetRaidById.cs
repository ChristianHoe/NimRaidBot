using System;
using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public sealed record GetRaidByIdRequest(
        long RaidId
    );

    public interface IGetRaidByIdQuery : IQuery<GetRaidByIdRequest, Raid?>
    {
    }

    public sealed class GetRaidById : IGetRaidByIdQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetRaidById(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public Raid? Execute(GetRaidByIdRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoRaids.Join(
                    db.PogoGyms,
                    r => r.GymId,
                    g => g.Id,
                    (r, g) => new Raid { GymId = g.Id, GymName = g.Name, Latitude = g.Latitude, Longitude = g.Longitude, PokeId = r.PokeId, PokeForm = r.PokeForm, Start = r.Start, Until = r.Finished, Level = r.Level, Id = r.Id, ChatId = r.ChatId, MoveId = r.Move2, Owner = r.OwnerId, Title = r.Title }
                    )
                    .Where(x => x.Id == request.RaidId)
                    .SingleOrDefault();
            }
        }
    }
}
