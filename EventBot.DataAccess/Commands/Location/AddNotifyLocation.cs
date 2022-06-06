using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;

namespace EventBot.DataAccess.Commands.Location
{
    public sealed record AddNotifyLocationRequest(
        long ChatId,
        int LocationId
    );

    public interface IAddNotifyLocationCommand : ICommand<AddNotifyLocationRequest>
    {
    }

    public sealed class AddNotifyLocationCommand : IAddNotifyLocationCommand
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
                db.NotifyLocations.Add(notifyLocation);
                db.SaveChanges();
            }
        }
    }
}