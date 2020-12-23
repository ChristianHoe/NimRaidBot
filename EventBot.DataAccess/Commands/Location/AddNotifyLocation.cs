using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;

namespace EventBot.DataAccess.Commands.Location
{
    public class AddNotifyLocationRequest
    {
        public long ChatId;
        public int LocationId;
    }

    public interface IAddNotifyLocationCommand : ICommand<AddNotifyLocationRequest>
    {
    }

    public class AddNotifyLocationCommand : IAddNotifyLocationCommand
    {
        readonly DatabaseFactory databaseFactory;

        public AddNotifyLocationCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(AddNotifyLocationRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var notifyLocation = new NotifyLocation { ChatId = request.ChatId, LocationId = request.LocationId };
                db.NotifyLocation.Add(notifyLocation);
                db.SaveChanges();
            }
        }
    }
}