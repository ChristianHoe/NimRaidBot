using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Minun
{
    public record EnableNotificationsRequest(
        long UserId
    );

    public interface IEnableNotificationsCommand : ICommand<EnableNotificationsRequest>
    {
    }

    public class EnableNotifications : IEnableNotificationsCommand
    {
        readonly DatabaseFactory databaseFactory;

        public EnableNotifications(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(EnableNotificationsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoUsers.SingleOrDefault(x => x.UserId == request.UserId);

                if (result != null)
                {
                    result.Active = true;
                    db.SaveChanges();
                }
            }
        }
    }
}