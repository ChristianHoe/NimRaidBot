using System.Linq;
using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;

namespace EventBot.DataAccess.Commands {
    public class State
    {
        public State (long chatId, string command) : this(chatId, command, 0)
        {          
        }

        public State (long chatId, string command, int step)
        {
            this.ChatId = chatId;
            this.Command = command;
            this.Step = step;
        }

        public long ChatId { get; }
        public string Command { get; }
        public int Step { get; }
    }

    public class StatePushRequest {
        public State State { get; }

        public StatePushRequest (State state) {
            State = state;
        }
    }

    public interface IStatePushCommand : ICommand<StatePushRequest> { }

    public class StatePush : IStatePushCommand {
        readonly DatabaseFactory databaseFactory;

        public StatePush (DatabaseFactory databaseFactory) {
            this.databaseFactory = databaseFactory;
        }

        public void Execute (StatePushRequest request) {
            using (var db = databaseFactory.CreateNew ()) {
                var currentLevel = db.States.Where (x => x.UserId == request.State.ChatId).OrderByDescending (x => x.Level).Select (x => x.Level).FirstOrDefault ();
                currentLevel++;

                db.States.Add (new States { UserId = request.State.ChatId, Command = request.State.Command, Level = currentLevel, Step = request.State.Step });

                db.SaveChanges ();
            }
        }
    }
}