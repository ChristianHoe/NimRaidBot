﻿using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.PoGo
{
    public sealed record AusRequest(
        long UserId
    );

    public interface IAusCommand : ICommand<AusRequest>
    {
    }

    public sealed class Aus : IAusCommand
    {
        readonly DatabaseFactory databaseFactory;

        public Aus(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(AusRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoUsers.SingleOrDefault(x => x.UserId == request.UserId);

                if (result != null)
                {
                    result.Active = false;
                    db.SaveChanges();
                }
            }
        }
    }
}
