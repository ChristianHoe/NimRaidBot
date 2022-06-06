using System;
using System.Collections.Generic;
using System.Linq;
using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;

namespace EventBot.DataAccess.Queries.Raid
{
    public sealed record GetInactiveUsersRequest(
        long GroupId,
        DateTime Threshold
    );

    public interface IGetInactiveUsersQuery : IQuery<GetInactiveUsersRequest, IEnumerable<PogoUser>>
    {
    }

    public sealed class GetInactiveUsers : IGetInactiveUsersQuery
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
                .Join(db.PogoUsers, 
                m => m.UserId, 
                u => u.UserId, 
                (m, u) => u).ToList();
            }
        }
    }
}