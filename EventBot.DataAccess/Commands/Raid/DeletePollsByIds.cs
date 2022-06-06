using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public sealed record DeletePollsByIdsRequest(
        int[] Ids
    );

    public interface IDeletePollsByIdsCommand : ICommand<DeletePollsByIdsRequest>
    {
    }

    public sealed class DeletePollsByIdsCommand : IDeletePollsByIdsCommand
    {
        readonly DatabaseFactory databaseFactory;

        public DeletePollsByIdsCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(DeletePollsByIdsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var toBeDeleted = db.ActivePolls.Where(x => request.Ids.Contains(x.Id));
                foreach(var record in toBeDeleted)
                {
                    record.Deleted = true;
                }
                db.SaveChanges();
            }
        }
    }
}
