using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries
{
    public record QueryCurrentStateRequest(
        long ChatId
    );

    public interface IQueryCurrentState : IQuery<QueryCurrentStateRequest, States>
    {
    }

    public class QueryCurrentState : IQueryCurrentState
    {
        readonly DatabaseFactory databaseFactory;

        public QueryCurrentState(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public States Execute(QueryCurrentStateRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.States.Where(x => x.UserId == request.ChatId).OrderByDescending(x => x.Level).FirstOrDefault();
            }
        }
    }
}
