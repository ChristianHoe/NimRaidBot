﻿using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Minun
{
    public sealed record GetRaidBossPreferencesAllRequest
    {
    }

    public interface IGetRaidBossPreferencesAllQuery : IQuery<GetRaidBossPreferencesAllRequest, IEnumerable<PogoRaidPreference>>
    {
    }

    public sealed class GetRaidBossPreferencesAll : IGetRaidBossPreferencesAllQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetRaidBossPreferencesAll(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoRaidPreference> Execute(GetRaidBossPreferencesAllRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoRaidPreferences.ToList();
            }
        }
    }
}
