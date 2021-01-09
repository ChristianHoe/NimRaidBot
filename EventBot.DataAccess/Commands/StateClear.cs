using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands
{
    public record StateClearRequest(
        long ChatId
    );

    public interface IStateClearCommand : ICommand<StateClearRequest>
    {
    }

    public class StateClear : IStateClearCommand
    {
        readonly DatabaseFactory databaseFactory;

        public StateClear(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(StateClearRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var results = db.States .Where(x => x.UserId == request.ChatId);

                db.States.RemoveRange(results);
                db.SaveChanges();
            }
        }
    }
}
