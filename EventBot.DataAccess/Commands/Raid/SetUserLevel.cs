using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public sealed record SetUserLevelRequest(
        long UserId,
        int Level
    );

    public interface ISetUserLevelCommand : ICommand<SetUserLevelRequest>
    {
    }

    public sealed class SetUserLevelCommand : ISetUserLevelCommand
    {
        readonly DatabaseFactory databaseFactory;

        public SetUserLevelCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(SetUserLevelRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoUsers.SingleOrDefault(x => x.UserId == request.UserId);

                if (result != null)
                {
                    result.Level = request.Level;
                    db.SaveChanges();
                }
            }
        }
    }
}
