using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public class SetChatForManualRaidAndInitializeRequest
    {
        public long UserId;
        public long ChatId;
    }

    public interface ISetChatForManualRaidAndInitializeCommand : ICommand<SetChatForManualRaidAndInitializeRequest>
    {
    }

    public class SetChatForManualRaidAndInitializeCommand : ISetChatForManualRaidAndInitializeCommand
    {
        readonly DatabaseFactory databaseFactory;

        public SetChatForManualRaidAndInitializeCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(SetChatForManualRaidAndInitializeRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoUserRaids.SingleOrDefault(x => x.UserId == request.UserId);

                if (result == null)
                {
                    db.PogoUserRaids.Add(new PogoUserRaids { UserId = request.UserId, ChatId = request.ChatId });
                }
                else
                {
                    result.ChatId = request.ChatId;
                    result.GymId = null;
                    result.Level = null;
                    result.PokeId = null;
                    result.RaidId = null;
                    result.Start = null;
                    result.TimeMode = null;
                    result.Title = string.Empty;
                    result.UpdRaidId = 0;
                }

                db.SaveChanges();
            }
        }
    }
}
