using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;

namespace EventBot.DataAccess.Commands.Minun
{
    public sealed record GameAnswerRequest(
        long ChatId,
        int MessageId,
        long UserId,
        string UserName,
        int Choice
    );

    public interface IGameAnswerCommand : ICommand<GameAnswerRequest>
    {
    }

    public sealed class GameAnswerCommand : IGameAnswerCommand
    {
        readonly DatabaseFactory databaseFactory;

        public GameAnswerCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(GameAnswerRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var answers = new PogoGamePokesAnswer {
                    ChatId = request.ChatId,
                    MessageId = request.MessageId,
                    UserId = request.UserId,
                    UserName = request.UserName,
                    Choice = request.Choice
                };

                db.PogoGamePokesAnswers.Add(answers);
                db.SaveChanges();
            }
        }
    }
}
