using System.Linq;
using EventBot.DataAccess.Database;

namespace EventBot.DataAccess.Commands.Location
{
    public record RemoveNotifyLocationRequest(
        long ChatId,
        int LocationId
    );

    public interface IRemoveNotifyLocationCommand : ICommand<RemoveNotifyLocationRequest>
    {
    }

    public class RemoveNotifyLocationCommand : IRemoveNotifyLocationCommand
    {
        readonly DatabaseFactory databaseFactory;

        public RemoveNotifyLocationCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(RemoveNotifyLocationRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var remove = db.NotifyLocation.SingleOrDefault(x => x.ChatId == request.ChatId && x.LocationId == request.LocationId);
                if (remove != null)
                {
                    db.NotifyLocation.Remove(remove);
                    db.SaveChanges();
                }
            }
        }
    }
}