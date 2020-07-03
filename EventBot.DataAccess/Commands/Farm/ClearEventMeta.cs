using EventBot.DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using System;

namespace EventBot.DataAccess.Commands.Farm
{
    public class ClearEventMetaRequest
    {
    }

    public interface IClearEventMetaCommand : ICommand<ClearEventMetaRequest>
    {
    }

    public class ClearEventMetaCommand : IClearEventMetaCommand
    {
        readonly DatabaseFactory databaseFactory;

        public ClearEventMetaCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(ClearEventMetaRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var date = DateTime.UtcNow.AddHours(-4);
                db.Database.ExecuteSqlInterpolated($"DELETE FROM INGR_EVENTS_META WHERE CREATED < @{date}");

                return;
            }
        }
    }
}
