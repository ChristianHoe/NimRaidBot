using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Base
{
    public record BotRemoveRequest(
        long ChatId,
        long BotId
    );

    public interface IBotRemoveCommand : ICommand<BotRemoveRequest>
    {
    }

    public class BotRemove : IBotRemoveCommand
    {
        readonly DatabaseFactory databaseFactory;

        public BotRemove(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(BotRemoveRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.RelChatBot.SingleOrDefault(x => x.ChatId == request.ChatId && x.BotId == request.BotId);

                if (result != null)
                {
                    db.RelChatBot.Remove(result);
                    db.SaveChanges();
                }
            }
        }
    }
}
