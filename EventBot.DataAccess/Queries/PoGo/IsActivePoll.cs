using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Queries.PoGo
{
    public sealed record IsActivePollRequest(
        long ChatId,
        long MessageId
    );

    public interface IIsActivePoll : IQuery<IsActivePollRequest, bool>
    {
    }

    public sealed class IsActivePoll : IIsActivePoll
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
