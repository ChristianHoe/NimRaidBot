using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Minun
{
    public class GetGameAnswersRequest
    {
        public long ChatId;
        public int MessageId;
    }

    public interface IGetGameAnswersQuery : IQuery<GetGameAnswersRequest, IEnumerable<PogoGamePokesAnswers>>
    {
    }

    public class GetGameAnswers : IGetGameAnswersQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetGameAnswers(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoGamePokesAnswers> Execute(GetGameAnswersRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoGamePokesAnswers.Where(x => request.ChatId == x.ChatId && request.MessageId == x.MessageId).ToList();
            }
        }
    }
}