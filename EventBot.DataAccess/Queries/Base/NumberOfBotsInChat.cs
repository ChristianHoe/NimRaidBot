using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Queries.Base
{
    public record NumberOfBotsInChatRequest(
        long ChatId
    );

    public interface INumberOfBotsInChatQuery : IQuery<NumberOfBotsInChatRequest, int>
    {
    }

    public class NumberOfBotsInChat : INumberOfBotsInChatQuery
    {
        readonly DatabaseFactory databaseFactory;

        public NumberOfBotsInChat(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public int Execute(NumberOfBotsInChatRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.RelChatBots.Count(x => x.ChatId == request.ChatId);
            }
        }
    }
}
