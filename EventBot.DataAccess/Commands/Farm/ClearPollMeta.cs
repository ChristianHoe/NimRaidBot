using EventBot.DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using System;

namespace EventBot.DataAccess.Commands.Farm
{
    public sealed record ClearPollMetaRequest();

    public interface IClearPollMetaCommand : ICommand<ClearPollMetaRequest>
    {
    }

    public sealed class ClearPollMetaCommand : IClearPollMetaCommand
    {
        readonly DatabaseFactory databaseFactory;

        public ClearPollMetaCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(ClearPollMetaRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var date = DateTime.UtcNow.AddHours(-4);
                db.Database.ExecuteSqlInterpolated($"DELETE FROM ACTIVE_POLLS_META WHERE CREATED < {date}");

                return;
            }
        }
    }
}
