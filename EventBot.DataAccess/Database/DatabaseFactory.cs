using EventBot.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace EventBot.DataAccess.Database
{
    public class DatabaseFactory
    {
        private DbContextOptions<telegramEntities> dbContextOptions;
        //private string connectionString;

        public DatabaseFactory(string connectionString)
        {
            //this.connectionString = connectionString;

            DbContextOptionsBuilder<telegramEntities> options = new DbContextOptionsBuilder<telegramEntities>();
            options.UseMySql(connectionString, new MariaDbServerVersion(new System.Version(5,5, 60)));

            this.dbContextOptions = options.Options;
        }


        public telegramEntities CreateNew()
        {
            return new telegramEntities(this.dbContextOptions);
        }
    }
}
