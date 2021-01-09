using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands
{
    public record StatePopRequest(
        State State
    );

    public interface IStatePopCommand : ICommand<StatePopRequest>
    {
    }

    public class StatePop : IStatePopCommand
    {
        readonly DatabaseFactory databaseFactory;

        public StatePop(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(StatePopRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.States.Where(x => x.UserId == request.State.ChatId && x.Command == request.State.Command).OrderByDescending(x => x.Level).FirstOrDefault();

                if (result != null)
                {
                    db.States.Remove(result);
                    db.SaveChanges();
                }
            }
        }
    }
}
