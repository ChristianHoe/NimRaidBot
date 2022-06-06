using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public sealed record SetEastRequest(
        long ChatId,
        decimal East
    );

    public interface ISetEastCommand : ICommand<SetEastRequest>
    {
    }

    public sealed class SetEastCommand : ISetEastCommand
    {
        readonly DatabaseFactory databaseFactory;

        public SetEastCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(SetEastRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoRaidUsers.SingleOrDefault(x => x.ChatId == request.ChatId);

                if (result != null)
                {
                    _Helper.SetEast(result, request.East);
                    db.SaveChanges();
                }
            }
        }
    }
}
