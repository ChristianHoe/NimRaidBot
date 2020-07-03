using System;
using EventBot.DataAccess.Database;
using Microsoft.EntityFrameworkCore;

namespace EventBot.DataAccess.Commands.Farm
{
    public class ClearQuestMetaRequest
    {
    }

    public interface IClearQuestMetaCommand : ICommand<ClearQuestMetaRequest>
    {
    }

    public class ClearQuestMetaCommand : IClearQuestMetaCommand
    {
        readonly DatabaseFactory databaseFactory;

        public ClearQuestMetaCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(ClearQuestMetaRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var date = DateTime.UtcNow.AddHours(-4);
                db.Database.ExecuteSqlInterpolated($"DELETE FROM POGO_QUESTS_META WHERE CREATED < {date}");

                return;
            }
        }
    }
}