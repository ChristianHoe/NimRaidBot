using System;
using System.Collections.Generic;
using System.Linq;
using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;

namespace EventBot.DataAccess.Queries.Raid
{
    public class GetInactiveUsersRequest
    {
        public long GroupId;
        public DateTime Threshold;
    }

    public interface IGetInactiveUsersQuery : IQuery<GetInactiveUsersRequest, IEnumerable<PogoUser>>
    {
    }

    public class GetInactiveUsers : IGetInactiveUsersQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetInactiveUsers(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoUser> Execute(GetInactiveUsersRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.Memberships
                .Where(x => x.GroupId == request.GroupId && x.LastAccess < request.Threshold)
                .Join(db.PogoUser, 
                m => m.UserId, 
                u => u.UserId, 
                (m, u) => u).ToList();
            }
        }
    }
}