using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Minun
{
    public record DisableNotificationsRequest(
        long UserId,
        long BotId
    );

    public interface IDisableNotificationsCommand : ICommand<DisableNotificationsRequest>
    {
    }

    public class DisableNotifications : IDisableNotificationsCommand
    {
        readonly DatabaseFactory databaseFactory;

        public DisableNotifications(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(DisableNotificationsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoUser.SingleOrDefault(x => x.UserId == request.UserId);

                if (result != null)
                {
                    result.Active = false;
                    db.SaveChanges();
                }

                var active = db.RelChatBot.SingleOrDefault(x => x.ChatId == request.UserId && x.BotId == request.BotId);
                if (active != null)
                {
                    active.AllowNotification = false;
                    db.SaveChanges();
                }
            }
        }
    }
}
