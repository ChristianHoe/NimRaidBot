using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries.Farm
{
    public class GetActivePollByEventIdRequest
    {
        public int EventId;
        public long ChatId;
    }

    public interface IGetActivePollByEventId : IQuery<GetActivePollByEventIdRequest, ActivePolls>
    {
    }

    public class GetActivePollByEventId : IGetActivePollByEventId
    {
        readonly DatabaseFactory databaseFactory;

        public GetActivePollByEventId(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public ActivePolls Execute(GetActivePollByEventIdRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.ActivePolls.SingleOrDefault(x => x.EventId == request.EventId && x.ChatId == request.ChatId);
            }
        }
    }
}
