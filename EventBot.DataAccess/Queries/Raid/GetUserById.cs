using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using EventBot.DataAccess.Queries.PoGo;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public class GetUserByIdRequest
    {
        public long UserId;
    }

    public interface IGetUserByIdQuery : IQuery<GetUserByIdRequest, PogoUser>
    {
    }

    public class GetUserById : IGetUserByIdQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetUserById(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public PogoUser Execute(GetUserByIdRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoUser.Where(x => x.UserId == request.UserId).SingleOrDefault();
            }
        }
    }
}
