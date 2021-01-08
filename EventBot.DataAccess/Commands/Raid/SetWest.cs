using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public record SetWestRequest(
        long ChatId,
        decimal West
    );

    public interface ISetWestCommand : ICommand<SetWestRequest>
    {
    }

    public class SetWestCommand : ISetWestCommand
    {
        readonly DatabaseFactory databaseFactory;

        public SetWestCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(SetWestRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoRaidUsers.SingleOrDefault(x => x.ChatId == request.ChatId);

                if (result != null)
                {
                    _Helper.SetWest(result, request.West);
                    db.SaveChanges();
                }
            }
        }
    }
}
