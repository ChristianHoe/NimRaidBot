using EventBot.DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace EventBot.DataAccess.Queries.Raid
{
    public class MarkQuestAsProcessingRequest
    {
        public int Id;
    }

    public interface IMarkQuestAsProcessingQuery : IQuery<MarkQuestAsProcessingRequest, bool>
    {
    }

    public class MarkQuestAsProcessing : IMarkQuestAsProcessingQuery
    {
        readonly DatabaseFactory databaseFactory;

        public MarkQuestAsProcessing(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public bool Execute(MarkQuestAsProcessingRequest request)
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
SET PROCESSED = 0
WHERE STOP_ID = @p0 AND PROCESSED is NULL";

                        command.CommandText = query;
                        command.Parameters.Add(new MySqlParameter("@p0", request.Id));
                        return command.ExecuteNonQuery() == 1;
                    }
                }
            }

        }
    }
}