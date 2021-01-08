using EventBot.DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace EventBot.DataAccess.Queries.Raid
{
    public record MarkQuestAsProcessedRequest(
        int Id
    );

    public interface IMarkQuestAsProcessedQuery : IQuery<MarkQuestAsProcessedRequest, bool>
    {
    }

    public class MarkQuestAsProcessed : IMarkQuestAsProcessedQuery
    {
        readonly DatabaseFactory databaseFactory;

        public MarkQuestAsProcessed(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public bool Execute(MarkQuestAsProcessedRequest request)
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

@"UPDATE POGO_QUESTS_META
SET PROCESSED = 1
WHERE STOP_ID = @p0 AND PROCESSED = 0";

                        command.CommandText = query;
                        command.Parameters.Add(new MySqlParameter("@p0", request.Id));

                        return command.ExecuteNonQuery() == 1;
                    }
                }
            }

        }
    }
}