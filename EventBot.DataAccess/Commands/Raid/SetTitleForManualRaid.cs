using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public sealed record SetTitleForManualRaidRequest(
        long UserId,
        string Title
    );

    public interface ISetTitleForManualRaidCommand : ICommand<SetTitleForManualRaidRequest>
    {
    }

    public sealed class SetTitleForManualRaidCommand : ISetTitleForManualRaidCommand
    {
        readonly DatabaseFactory databaseFactory;

        public SetTitleForManualRaidCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }

        public void Execute(SetTitleForManualRaidRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoUserRaids.SingleOrDefault(x => x.UserId == request.UserId);

                if (result != null)
                {
                    result.Title = request.Title;
                    db.SaveChanges();
                }
            }
        }
    }
}
