using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public sealed record GetUserByIdRequest(
        long UserId
    );

    public interface IGetUserByIdQuery : IQuery<GetUserByIdRequest, PogoUser?>
    {
    }

    public sealed class GetUserById : IGetUserByIdQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetUserById(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public PogoUser? Execute(GetUserByIdRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoUsers.Where(x => x.UserId == request.UserId).SingleOrDefault();
            }
        }
    }
}
