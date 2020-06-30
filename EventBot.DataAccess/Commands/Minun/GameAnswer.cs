using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;

namespace EventBot.DataAccess.Commands.Minun
{
    public class GameAnswerRequest
    {
        public long ChatId;
        public int MessageId;

        public long UserId;

        public string UserName;

        public int Choice;
    }

    public interface IGameAnswerCommand : ICommand<GameAnswerRequest>
    {
    }

    public class GameAnswerCommand : IGameAnswerCommand
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
                var answers = new PogoGamePokesAnswers {
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
