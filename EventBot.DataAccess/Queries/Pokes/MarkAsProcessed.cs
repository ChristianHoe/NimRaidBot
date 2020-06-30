using EventBot.DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace EventBot.DataAccess.Queries.Pokes
{
    public class MarkAsProcessedRequest
    {
        public int Id;
    }

    public interface IMarkAsProcessedQuery : IQuery<MarkAsProcessedRequest, bool>
    {
    }

    public class MarkAsProcessed : IMarkAsProcessedQuery
    {
        readonly DatabaseFactory databaseFactory;

        public MarkAsProcessed(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public bool Execute(MarkAsProcessedRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                using (var conn = db.Database.GetDbConnection())
                {
                    //await conn.OpenAsync();
                    conn.Open();
                    using (var command = conn.CreateCommand())
                    {
                        string query =

@"UPDATE POGO_POKES_META
SET POKE = 1
WHERE POGO_POKE_ID = @p0 AND POKE is null";

                        command.CommandText = query;
                        command.Parameters.Add(new MySqlParameter("@p0", request.Id));

                        return command.ExecuteNonQuery() == 1;
                    }
                }
            }

        }
    }
}
