using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public class ActivateChatRequest
    {
        public long ChatId;
    }

    public interface IActivateChatCommand : ICommand<ActivateChatRequest>
    {
    }

    public class ActivateChat : IActivateChatCommand
    {
        readonly DatabaseFactory databaseFactory;

        public ActivateChat(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(ActivateChatRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoRaidUsers.SingleOrDefault(x => x.ChatId == request.ChatId);

                if (result != null)
                {
                    result.Active = true;
                    db.SaveChanges();
                }
            }
        }
    }
}
