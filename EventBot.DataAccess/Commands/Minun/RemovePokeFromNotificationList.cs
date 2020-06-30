using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Minun
{
    public class RemovePokeFromNotificationListRequest
    {
        public long ChatId;
        public int PokeId;
    }

    public interface IRemovePokeFromNotificationListCommand : ICommand<RemovePokeFromNotificationListRequest>
    {
    }

    public class RemovePokeFromNotificationList : IRemovePokeFromNotificationListCommand
    {
        readonly DatabaseFactory databaseFactory;

        public RemovePokeFromNotificationList(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(RemovePokeFromNotificationListRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoChatPoke.SingleOrDefault(x => x.ChatId == request.ChatId && x.PokeId == request.PokeId);

                if (result != null)
                {
                    db.PogoChatPoke.Remove(result);
                    db.SaveChanges();
                }
            }
        }
    }
}
