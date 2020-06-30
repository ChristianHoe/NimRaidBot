using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries.PoGo
{
    public class IgnoredMonsterByUserRequest
    {
        public long[] UserIds;
    }

    public interface IIgnoredMonsterByUser : IQuery<IgnoredMonsterByUserRequest, ILookup<int, PogoIgnore>>
    {
    }

    public class IgnoredMonsterByUser : IIgnoredMonsterByUser
    {
        readonly DatabaseFactory databaseFactory;

        public IgnoredMonsterByUser(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public ILookup<int, PogoIgnore> Execute(IgnoredMonsterByUserRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoIgnore.Where(x => request.UserIds.Contains(x.UserId)).ToLookup(x => x.MonsterId);
            }
        }
    }
}
