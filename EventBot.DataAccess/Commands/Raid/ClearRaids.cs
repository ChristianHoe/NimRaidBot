using EventBot.DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using System;

namespace EventBot.DataAccess.Commands.Raid
{
    public class ClearRaidsRequest
    {
        public long ChatId;
    }

    public interface IClearRaidsCommand : ICommand<ClearRaidsRequest>
    {
    }

    public class ClearRaidsCommand : IClearRaidsCommand
    {
        readonly DatabaseFactory databaseFactory;

        public ClearRaidsCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(ClearRaidsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var date = DateTime.UtcNow.AddDays(-7);
                db.Database.ExecuteSqlInterpolated($"DELETE FROM POGO_RAIDS WHERE FINISHED < {date}");

                return;
            }
        }
    }
}
