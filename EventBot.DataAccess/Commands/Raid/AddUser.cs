using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public sealed record AddUserRequest(
        long UserId,
        string FirstName
    );

    public interface IAddUserCommand : ICommand<AddUserRequest>
    {
    }

    public sealed class AddUser : IAddUserCommand
    {
        readonly DatabaseFactory databaseFactory;

        public AddUser(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(AddUserRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoUsers.SingleOrDefault(x => x.UserId == request.UserId);

                if (result == null)
                {
                    db.PogoUsers.Add(new PogoUser { UserId = request.UserId, Active = false, FirstName = request.FirstName });
                    db.SaveChanges();
                }
            }
        }
    }
}
