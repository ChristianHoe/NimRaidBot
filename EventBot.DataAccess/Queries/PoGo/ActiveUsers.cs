﻿using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.PoGo
{
    public sealed record ActiveUsersRequest();

    public interface IActiveUsers : IQuery<ActiveUsersRequest, IEnumerable<PogoUser>>
    {
    }

    public sealed class ActiveUsers : IActiveUsers
    {
        readonly DatabaseFactory databaseFactory;

        public ActiveUsers(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoUser> Execute(ActiveUsersRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoUsers.Where(x => x.Active).ToList();
            }
        }
    }
}
