using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.PoGo
{
    public sealed record AnRequest(
        long UserId,
        long? BotId
    );

    public interface IAnCommand : ICommand<AnRequest>
    {
    }

    public sealed class An : IAnCommand
    {
        readonly DatabaseFactory databaseFactory;

        public An(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(AnRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoUsers.SingleOrDefault(x => x.UserId == request.UserId);

                if (result == null)
                {
                    result = _Helper.CreateNewUser(db, request.UserId);
                }

                if (result != null)
                {
                    result.Active = true;
                    db.SaveChanges();
                }

                var active = db.RelChatBots.SingleOrDefault(x => x.ChatId == request.UserId && x.BotId == request.BotId);
                if (active != null)
                {
                    active.AllowNotification = true;
                    db.SaveChanges();
                }
            }
        }
    }
}
