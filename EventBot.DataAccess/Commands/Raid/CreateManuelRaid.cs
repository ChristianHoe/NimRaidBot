using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public sealed record CreateManuelRaidRequest(
        long UserId,
        int DurationInMinutes
    );

    public interface ICreateManuelRaidCommand : ICommand<CreateManuelRaidRequest>
    {
    }

    public sealed class CreateManuelRaidCommand : ICreateManuelRaidCommand
    {
        readonly DatabaseFactory databaseFactory;

        public CreateManuelRaidCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(CreateManuelRaidRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var userraid = db.PogoUserRaids.SingleOrDefault(x => x.UserId == request.UserId);

                if (userraid != null)
                {
                    var raid = new PogoRaid { ChatId = null, Finished = (userraid.Start ?? DateTime.UtcNow).AddMinutes(request.DurationInMinutes), Start = (userraid.Start ?? DateTime.UtcNow), GymId = userraid.GymId ?? 0, Level = userraid.Level ?? 0, PokeId = userraid.PokeId ?? 0, PokeForm = userraid.PokeForm, OwnerId = userraid.UserId, Title = userraid.Title };
                    db.PogoRaids.Add(raid);
                    db.SaveChanges();

                    userraid.RaidId = raid.Id;
                    db.SaveChanges();
                }

                return;
            }
        }
    }
}
