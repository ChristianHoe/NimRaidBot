using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Minun
{
    public sealed record GetGameAnswersRequest(
        long ChatId,
        int MessageId
    );

    public interface IGetGameAnswersQuery : IQuery<GetGameAnswersRequest, IEnumerable<PogoGamePokesAnswer>>
    {
    }

    public sealed class GetGameAnswers : IGetGameAnswersQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetGameAnswers(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoGamePokesAnswer> Execute(GetGameAnswersRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoGamePokesAnswers.Where(x => request.ChatId == x.ChatId && request.MessageId == x.MessageId).ToList();
            }
        }
    }
}