using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands
{
    public class StateUpdateRequest
    {
        public State State { get; }

        public StateUpdateRequest(State state)
        {
            State = state;
        }
    }

    public interface IStateUpdateCommand : ICommand<StateUpdateRequest>
    {
    }

    public sealed class StateUpdate : IStateUpdateCommand
    {
        readonly DatabaseFactory databaseFactory;

        public StateUpdate(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(StateUpdateRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.States
                    .Where(x => x.UserId == request.State.ChatId && x.Command == request.State.Command)
                    .OrderByDescending(x => x.Level)
                    .First();

                if (result != null)
                {
                    result.Step = request.State.Step;
                    db.SaveChanges();
                }
            }
        }
    }
}
