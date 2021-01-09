using EventBot.DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using System;

namespace EventBot.DataAccess.Commands.Farm
{
    public record ClearPokeMetaRequest();

    public interface IClearPokeMetaCommand : ICommand<ClearPokeMetaRequest>
    {
    }

    public class ClearPokeMetaCommand : IClearPokeMetaCommand
    {
        readonly DatabaseFactory databaseFactory;

        public ClearPokeMetaCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(ClearPokeMetaRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var date = DateTime.UtcNow.AddHours(-4);
                db.Database.ExecuteSqlInterpolated($"DELETE FROM POGO_POKES_META WHERE CREATED < {date}");

                return;
            }
        }
    }
}
