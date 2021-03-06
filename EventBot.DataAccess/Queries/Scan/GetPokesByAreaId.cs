﻿using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Scan
{
    public record GetPokesByAreaIdRequest(
        long ScanAreaId
    );

    public interface IGetPokesByAreaId : IQuery<GetPokesByAreaIdRequest, IEnumerable<PogoChatPoke>>
    {
    }

    public class GetPokesByAreaId : IGetPokesByAreaId
    {
        readonly DatabaseFactory databaseFactory;

        public GetPokesByAreaId(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoChatPoke> Execute(GetPokesByAreaIdRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoRelScanChat
                    .Where(x => x.ScanAreaId == request.ScanAreaId)
                    .Join(db.PogoRaidUsers,
                        s => s.ChatId,
                        c => c.ChatId,
                        (s, c) => c)
                    .Join(db.PogoChatPoke,
                        c => c.ChatId,
                        p => p.ChatId,
                        (c, p) => p)
                    .Distinct()
                    .ToArray();
            }
        }
    }
}
