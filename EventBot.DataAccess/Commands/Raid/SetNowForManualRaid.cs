using EventBot.DataAccess.Database;
using System;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public sealed record SetNowForManualRaidRequest(
        long UserId,
        DateTime Start
    );

    public interface ISetNowForManualRaidCommand : ICommand<SetNowForManualRaidRequest>
    {
    }

    public sealed class SetNowForManualRaidCommand : ISetNowForManualRaidCommand
    {
        readonly DatabaseFactory databaseFactory;

        public SetNowForManualRaidCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(SetNowForManualRaidRequest request)
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
                    result.Start = request.Start;
                    db.SaveChanges();
                }
            }
        }
    }
}
