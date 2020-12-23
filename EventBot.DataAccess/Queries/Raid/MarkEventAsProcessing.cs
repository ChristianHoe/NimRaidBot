using EventBot.DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace EventBot.DataAccess.Queries.Raid
{
    public class MarkEventAsProcessingRequest
    {
        public int Id;
    }

    public interface IMarkEventAsProcessingQuery : IQuery<MarkEventAsProcessingRequest, bool>
    {
    }

    public class MarkEventAsProcessing : IMarkEventAsProcessingQuery
    {
        readonly DatabaseFactory databaseFactory;

        public MarkEventAsProcessing(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public bool Execute(MarkEventAsProcessingRequest request)
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

@"UPDATE POGO_RAIDS_META
SET RAID = 0
WHERE RAID_ID = @p0 AND RAID is NULL";

                        command.CommandText = query;
                        command.Parameters.Add(new MySqlParameter("@p0", request.Id));
                        return command.ExecuteNonQuery() == 1;
                    }
                }
            }
        }
    }
}
