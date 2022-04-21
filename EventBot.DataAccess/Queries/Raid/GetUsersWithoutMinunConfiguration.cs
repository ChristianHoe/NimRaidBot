using System;
using System.Collections.Generic;
using System.Linq;
using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;

namespace EventBot.DataAccess.Queries.Raid
{
    public record GetUsersWithoutMinunConfigurationRequest(
        long GroupId,
        DateTime Threshold
    );

    public interface IGetUsersWithoutMinunConfigurationQuery : IQuery<GetUsersWithoutMinunConfigurationRequest, IEnumerable<PogoUser>>
    {
    }

    public class GetUsersWithoutMinunConfiguration : IGetUsersWithoutMinunConfigurationQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetUsersWithoutMinunConfiguration(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoUser> Execute(GetUsersWithoutMinunConfigurationRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.Memberships
                .Where(x => x.GroupId == request.GroupId && x.Created < request.Threshold)
                .Join(db.PogoUsers.Where(y => y.IngameName == null && y.Team == null && y.Level == null), 
                m => m.UserId, 
                u => u.UserId, 
                (m, u) => u).ToList();
            }
        }
    }
}