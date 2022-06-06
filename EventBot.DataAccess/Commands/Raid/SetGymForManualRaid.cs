using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public sealed record SetGymForManualRaidRequest(
        long UserId,
        int GymId
    );

    public interface ISetGymForManualRaidCommand : ICommand<SetGymForManualRaidRequest>
    {
    }

    public sealed class SetGymForManualRaidCommand : ISetGymForManualRaidCommand
    {
        readonly DatabaseFactory databaseFactory;

        public SetGymForManualRaidCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(SetGymForManualRaidRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoUserRaids.SingleOrDefault(x => x.UserId == request.UserId);

                //if (result == null)
                //{
                //    db.POGO_USER_RAIDS.Add(new POGO_USER_RAIDS { USER_ID = request.UserId, CHAT_ID = request.ChatId });
                //}

                if (result != null)
                {
                    result.GymId = request.GymId;
                    db.SaveChanges();
                }
            }
        }
    }
}
