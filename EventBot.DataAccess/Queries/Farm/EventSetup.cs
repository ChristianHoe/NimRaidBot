using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries.Farm
{
    public record EventSetupRequest(
        long ChatId,
        long MessageId
    );

    public interface IEventSetupQuery : IQuery<EventSetupRequest, DataAccess.Models.EventSetup?>
    {
    }

    public class EventSetup : IEventSetupQuery
    {
        readonly DatabaseFactory databaseFactory;

        public EventSetup(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public DataAccess.Models.EventSetup? Execute(EventSetupRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.EventSetups.SingleOrDefault(x => x.ChatId == request.ChatId && x.MessageId == request.MessageId);
            }
        }
    }
}
