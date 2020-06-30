using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries.PoGo
{
    public class IsActivePollRequest
    {
        public long ChatId;
        public long MessageId;
    }

    public interface IIsActivePoll : IQuery<IsActivePollRequest, bool>
    {
    }

    public class IsActivePoll : IIsActivePoll
    {
        readonly DatabaseFactory databaseFactory;

        public IsActivePoll(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public bool Execute(IsActivePollRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.ActivePolls.Any(x => x.ChatId == request.ChatId && x.MessageId == request.MessageId);
            }
        }
    }
}
