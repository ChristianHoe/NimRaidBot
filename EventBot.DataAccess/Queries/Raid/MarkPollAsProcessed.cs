using EventBot.DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace EventBot.DataAccess.Queries.Raid
{
    public class MarkPollAsProcessedRequest
    {
        public int Id;
    }

    public interface IMarkPollAsProcessedQuery : IQuery<MarkPollAsProcessedRequest, bool>
    {
    }

    public class MarkPollAsProcessed : IMarkPollAsProcessedQuery
    {
        readonly DatabaseFactory databaseFactory;

        public MarkPollAsProcessed(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public bool Execute(MarkPollAsProcessedRequest request)
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

@"UPDATE ACTIVE_POLLS_META
SET POKE = 1
WHERE POLL_ID = @p0 AND POKE = 0";

                        command.CommandText = query;
                        command.Parameters.Add(new MySqlParameter("@p0", request.Id));

                        return command.ExecuteNonQuery() == 1;
                    }
                }
            }

        }
    }
}
