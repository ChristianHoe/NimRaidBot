using System.Linq;
using EventBot.DataAccess.Database;

namespace EventBot.DataAccess.Commands.Raid
{

    public class SetRaidIdToUpdateRequest
    {
        public long UserId;
        public int RaidId;
    }

    public interface ISetRaidIdToUpdateCommand : ICommand<SetRaidIdToUpdateRequest>
    {
    }

    public class SetRaidIdToUpdateCommand : ISetRaidIdToUpdateCommand
    {
        readonly DatabaseFactory databaseFactory;

        public SetRaidIdToUpdateCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(SetRaidIdToUpdateRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoUserRaids.SingleOrDefault(x => x.UserId == request.UserId);

                if (result != null)
                {
                    result.UpdRaidId = request.RaidId;
                    db.SaveChanges();
                }
            }
        }
    }
}