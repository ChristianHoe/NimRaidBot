using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Minun
{
    public record DeactiveMinunUserRequest(
        long UserId,
        long BotId
    );

    public interface IDeactiveMinunUserCommand : ICommand<DeactiveMinunUserRequest>
    {
    }

    public class DeactiveMinunUser : IDeactiveMinunUserCommand
    {
        readonly DatabaseFactory databaseFactory;

        public DeactiveMinunUser(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(DeactiveMinunUserRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.RelChatBots.SingleOrDefault(x => x.ChatId == request.UserId && x.BotId == request.BotId);

                if (result != null)
                {
                    db.Remove(result);
                    db.SaveChanges();
                }
            }
        }
    }
}
