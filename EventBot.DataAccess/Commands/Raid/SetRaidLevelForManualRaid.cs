using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public record SetRaidLevelForManualRaidRequest(
        long UserId,
        int Level
    );

    public interface ISetRaidLevelForManualRaidCommand : ICommand<SetRaidLevelForManualRaidRequest>
    {
    }

    public class SetRaidLevelForManualRaidCommand : ISetRaidLevelForManualRaidCommand
    {
        readonly DatabaseFactory databaseFactory;

        public SetRaidLevelForManualRaidCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(SetRaidLevelForManualRaidRequest request)
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
                    result.PokeId = null;
                    result.Level = request.Level;
                    db.SaveChanges();
                }
            }
        }
    }
}
