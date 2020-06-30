using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public class GetActivePollByMessageIdRequest
    {
        public int MessageId;
        public long ChatId;
    }

    public interface IGetActivePollByMessageId : IQuery<GetActivePollByMessageIdRequest, ActivePolls>
    {
    }

    public class GetActivePollByMessageId : IGetActivePollByMessageId
    {
        readonly DatabaseFactory databaseFactory;

        public GetActivePollByMessageId(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public ActivePolls Execute(GetActivePollByMessageIdRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.ActivePolls.SingleOrDefault(x => x.MessageId == request.MessageId && x.ChatId == request.ChatId);
            }
        }
    }
}
