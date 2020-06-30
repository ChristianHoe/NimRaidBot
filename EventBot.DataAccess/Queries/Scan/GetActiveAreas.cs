﻿using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Scan
{
    public class GetActiveAreasRequest
    {
        public long UserId;
        public int MapId;
    }

    public interface IGetActiveAreas : IQuery<GetActiveAreasRequest, IEnumerable<PogoScanArea>>
    {
    }

    public class GetActiveAreas : IGetActiveAreas
    {
        readonly DatabaseFactory databaseFactory;

        public GetActiveAreas(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoScanArea> Execute(GetActiveAreasRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoScanArea.Where(x => x.Active == true && x.MapId == request.MapId).ToArray();
            }
        }
    }
}
