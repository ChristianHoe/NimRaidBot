using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public sealed record UpdateMembershipAccessRequest(
        long GroupId,
        long UserId
    );

    public interface IUpdateMembershipAccessCommand : ICommand<UpdateMembershipAccessRequest>
    {
    }

    public sealed class UpdateMembershipAccess : IUpdateMembershipAccessCommand
    {
        readonly DatabaseFactory databaseFactory;

        public UpdateMembershipAccess(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(UpdateMembershipAccessRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.Memberships.SingleOrDefault(x => x.GroupId == request.GroupId && x.UserId == request.UserId);

                if (result != null)
                {
                    result.LastAccess = DateTime.UtcNow;
                }
                else
                {
                    db.Memberships.Add(new Membership { GroupId = request.GroupId, UserId = request.UserId, LastAccess = DateTime.UtcNow});
                }

                db.SaveChanges();
            }
        }
    }
}