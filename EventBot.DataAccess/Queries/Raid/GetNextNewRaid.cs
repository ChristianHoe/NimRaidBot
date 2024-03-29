﻿using EventBot.DataAccess.Database;
using System;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public sealed record GetNextNewRaidRequest
    {
    }

    public interface IGetNextNewRaidQuery : IQuery<GetNextNewRaidRequest, Raid?>
    {
    }

    public sealed class GetNextNewRaid : IGetNextNewRaidQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetNextNewRaid(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public Raid? Execute(GetNextNewRaidRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoRaids.Join(
                    db.PogoRaidsMeta.Where(x => x.Raid == null),
                    r => r.Id,
                    m => m.RaidId,
                    (r, m) => r
                    ).Join(
                    db.PogoGyms,
                    r => r.GymId,
                    g => g.Id,
                    (r, g) => new Raid { GymId = g.Id, GymName = g.Name, Latitude = g.Latitude, Longitude = g.Longitude, PokeId = r.PokeId, PokeForm = r.PokeForm, Start = r.Start, Until = r.Finished, Level = r.Level, Id = r.Id, ChatId = r.ChatId, MoveId = r.Move2, Owner = r.OwnerId, Title = r.Title }
                    ).FirstOrDefault();
            }
        }
    }
}
