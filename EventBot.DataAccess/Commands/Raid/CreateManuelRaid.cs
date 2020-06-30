using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public class CreateManuelRaidRequest
    {
        public long UserId;
    }

    public interface ICreateManuelRaidCommand : ICommand<CreateManuelRaidRequest>
    {
    }

    public class CreateManuelRaidCommand : ICreateManuelRaidCommand
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
                    var raid = new PogoRaids { ChatId = null, Finished = (userraid.Start ?? DateTime.UtcNow).AddMinutes(45), Start = (userraid.Start ?? DateTime.UtcNow), GymId = userraid.GymId ?? 0, Level = userraid.Level ?? 0, PokeId = userraid.PokeId ?? 0, OwnerId = userraid.UserId, Title = userraid.Title };
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
