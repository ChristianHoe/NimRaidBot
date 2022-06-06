using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Minun
{
    public sealed record RemovePokeFromNotificationListRequest(
        long ChatId,
        int PokeId
    );

    public interface IRemovePokeFromNotificationListCommand : ICommand<RemovePokeFromNotificationListRequest>
    {
    }

    public sealed class RemovePokeFromNotificationList : IRemovePokeFromNotificationListCommand
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
                var result = db.PogoChatPokes.SingleOrDefault(x => x.ChatId == request.ChatId && x.PokeId == request.PokeId);

                if (result != null)
                {
                    db.PogoChatPokes.Remove(result);
                    db.SaveChanges();
                }
            }
        }
    }
}
