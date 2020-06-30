using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Commands.Pokes
{
    public class RemoveNoficationsByIdsRequest
    {
        public int[] Ids;
    }

    public interface IRemoveNotificationsByIdsCommand : ICommand<RemoveNoficationsByIdsRequest>
    {
    }
    public class RemoveNotificationsByIdsCommand : IRemoveNotificationsByIdsCommand
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
