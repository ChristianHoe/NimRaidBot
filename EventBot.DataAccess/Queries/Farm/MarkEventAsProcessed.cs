using EventBot.DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace EventBot.DataAccess.Queries.Farm
{
    public class MarkEventAsProcessedRequest
    {
        public int Id;
    }

    public interface IMarkEventAsProcessedQuery : IQuery<MarkEventAsProcessedRequest, bool>
    {
    }

    public class MarkEventAsProcessed : IMarkEventAsProcessedQuery
    {
        readonly DatabaseFactory databaseFactory;

        public MarkEventAsProcessed(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public bool Execute(MarkEventAsProcessedRequest request)
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

@"UPDATE INGR_EVENTS_META
SET FARM = 1
WHERE EVENT_ID = @p0 AND FARM = 0";

                        command.CommandText = query;
                        command.Parameters.Add(new MySqlParameter("@p0", request.Id));

                        return command.ExecuteNonQuery() == 1;
                    }
                }
            }

        }
    }
}
