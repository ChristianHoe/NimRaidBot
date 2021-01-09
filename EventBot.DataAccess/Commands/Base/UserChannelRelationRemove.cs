using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Base
{
    public record UserChannelRelationRemoveRequest(
        long UserId,
        long ChatId
    );

    public interface IUserChannelRelationRemoveCommand : ICommand<UserChannelRelationRemoveRequest>
    {
    }

    public class UserChannelRelationRemove : IUserChannelRelationRemoveCommand
    {
        readonly DatabaseFactory databaseFactory;

        public UserChannelRelationRemove(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(UserChannelRelationRemoveRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.Memberships.SingleOrDefault(x => x.UserId == request.UserId && x.GroupId == request.ChatId);

                if (result != null)
                {
                    db.Memberships.Remove(result);
                    db.SaveChanges();
                }
            }
        }
    }
}
