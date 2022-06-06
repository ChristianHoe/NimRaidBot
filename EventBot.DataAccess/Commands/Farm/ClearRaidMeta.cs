using EventBot.DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using System;

namespace EventBot.DataAccess.Commands.Farm
{
    public sealed record ClearRaidMetaRequest();

    public interface IClearRaidMetaCommand : ICommand<ClearRaidMetaRequest>
    {
    }

    public sealed class ClearRaidMetaCommand : IClearRaidMetaCommand
    {
        readonly DatabaseFactory databaseFactory;

        public ClearRaidMetaCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(ClearRaidMetaRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var date = DateTime.UtcNow.AddHours(-4);
                db.Database.ExecuteSqlInterpolated($"DELETE FROM POGO_RAIDS_META WHERE CREATED < {date}");

                return;
            }
        }
    }
}
