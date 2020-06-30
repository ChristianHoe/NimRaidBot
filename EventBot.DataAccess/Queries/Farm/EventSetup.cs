using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries.Farm
{
    public class EventSetupRequest
    {
        public long ChatId;
        public long MessageId;
    }

    public interface IEventSetupQuery : IQuery<EventSetupRequest, EventSetups>
    {
    }

    public class EventSetup : IEventSetupQuery
    {
        readonly DatabaseFactory databaseFactory;

        public EventSetup(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public EventSetups Execute(EventSetupRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.EventSetups.SingleOrDefault(x => x.ChatId == request.ChatId && x.MessageId == request.MessageId);
            }
        }
    }
}
