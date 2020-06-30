using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public class SetTimeModeForManualRaidRequest
    {
        public long UserId;
        public int TimeMode;
    }

    public interface ISetTimeModeForManualRaidCommand : ICommand<SetTimeModeForManualRaidRequest>
    {
    }

    public class SetTimeModeForManualRaidCommand : ISetTimeModeForManualRaidCommand
    {
        readonly DatabaseFactory databaseFactory;

        public SetTimeModeForManualRaidCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(SetTimeModeForManualRaidRequest request)
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
                    result.TimeMode = request.TimeMode;
                    db.SaveChanges();
                }
            }
        }
    }
}
