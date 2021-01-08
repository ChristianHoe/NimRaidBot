using System.Linq;
using EventBot.DataAccess.Database;

namespace EventBot.DataAccess.Commands.Raid
{
    public record RemoveMembershipByUserIdsRequest(
        long GroupId,
        long[] UserIds
    );

    public interface IRemoveMembershipByUserIdsCommand : ICommand<RemoveMembershipByUserIdsRequest>
    {
    }

    public class RemoveMembershipByUserIdsCommand : IRemoveMembershipByUserIdsCommand
    {
        readonly DatabaseFactory databaseFactory;

        public RemoveMembershipByUserIdsCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(RemoveMembershipByUserIdsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var toBeDeleted = db.Memberships.Where(x => request.UserIds.Contains(x.UserId) && request.GroupId == x.GroupId);
                db.Memberships.RemoveRange(toBeDeleted);

                db.SaveChanges();
            }
        }
    }
}