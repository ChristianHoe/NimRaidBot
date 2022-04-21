using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public record GetActivePollByMessageIdRequest(
        int MessageId,
        long ChatId
    );

    public interface IGetActivePollByMessageId : IQuery<GetActivePollByMessageIdRequest, ActivePoll?>
    {
    }

    public class GetActivePollByMessageId : IGetActivePollByMessageId
    {
        readonly DatabaseFactory databaseFactory;

        public GetActivePollByMessageId(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public ActivePoll? Execute(GetActivePollByMessageIdRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.ActivePolls.SingleOrDefault(x => x.MessageId == request.MessageId && x.ChatId == request.ChatId);
            }
        }
    }
}
