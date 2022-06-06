using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Pokes
{
    public sealed record RemoveNoficationsByIdsRequest(
        int[] Ids
    );

    public interface IRemoveNotificationsByIdsCommand : ICommand<RemoveNoficationsByIdsRequest>
    {
    }

    public sealed class RemoveNotificationsByIdsCommand : IRemoveNotificationsByIdsCommand
    {
        readonly DatabaseFactory databaseFactory;

        public RemoveNotificationsByIdsCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(RemoveNoficationsByIdsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var toBeDeleted = db.PogoRelPokesChats.Where(x => request.Ids.Contains(x.Id));
                foreach (var record in toBeDeleted)
                {
                    record.Deleted = true;
                }
                db.SaveChanges();
            }
        }
    }
}
