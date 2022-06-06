using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries
{
    public sealed record QueryCurrentStateRequest(
        long ChatId
    );

    public interface IQueryCurrentState : IQuery<QueryCurrentStateRequest, State?>
    {
    }

    public sealed class QueryCurrentState : IQueryCurrentState
    {
        readonly DatabaseFactory databaseFactory;

        public QueryCurrentState(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public State? Execute(QueryCurrentStateRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.States.Where(x => x.UserId == request.ChatId).OrderByDescending(x => x.Level).FirstOrDefault();
            }
        }
    }
}
