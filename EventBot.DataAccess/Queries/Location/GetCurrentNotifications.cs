using System;
using System.Collections.Generic;
using System.Linq;
using EventBot.DataAccess.Database;
using EventBot.DataAccess.ModelsEx;

namespace EventBot.DataAccess.Queries.Location
{
    public record GetCurrentNotificationsRequest(
        int LocationId,
        DateTime Threshold
    );

    public interface IGetCurrentNotificationsQuery : IQuery<GetCurrentNotificationsRequest, IEnumerable<NotifyLocationExtended>>
    {
    }

    public class GetCurrentNotifications : IGetCurrentNotificationsQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetCurrentNotifications(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<NotifyLocationExtended> Execute(GetCurrentNotificationsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoRaidUsers
                .Join(
                    db.Memberships,
                    g => g.ChatId,
                    m => m.GroupId,
                    (g, m) => m
                ).Join(
                    db.NotifyLocations,
                    m => m.GroupId,
                    n1 => n1.ChatId,
                    (m, n1) => new { m, n1 }
                ).Join(
                    db.NotifyLocations,
                    x => x.m.UserId,
                    n2 => n2.ChatId,
                    (x,  n2) => new { x.m,  x.n1, n2}
                ).Join(
                    db.RelChatBots,
                    x => x.n2.ChatId,
                    b => b.ChatId,
                    (x, b) => new {x.m, x.n1, x.n2, b}
                ).Where(
                    x => x.n1.LocationId == x.n2.LocationId && x.n2.LocationId == request.LocationId && x.m.LastAccess >= request.Threshold && x.b.AllowNotification
                ).Select(
                    x => new NotifyLocationExtended { ChatId = x.n2.ChatId, LocationId = x.n2.LocationId, BotId = x.b.BotId }
                ).Concat(
                    db.PogoRaidUsers
                    .Join(
                        db.NotifyLocations,
                        g => g.ChatId,
                        n2 => n2.ChatId,
                        (g, n2) => n2
                    ).Join(
                        db.RelChatBots,
                        n2 => n2.ChatId,
                        b => b.ChatId,
                        (n2, b) => new { n2, b }
                    ).Where(
                        x => x.b.AllowNotification && x.n2.LocationId == request.LocationId
                    ).Select(
                        x => new NotifyLocationExtended { ChatId = x.n2.ChatId, LocationId = x.n2.LocationId, BotId = x.b.BotId }
                    )
                ).Distinct()
                .ToList();
            }
        }
    }
}