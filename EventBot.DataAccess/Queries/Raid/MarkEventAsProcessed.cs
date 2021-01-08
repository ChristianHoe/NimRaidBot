using EventBot.DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace EventBot.DataAccess.Queries.Raid
{
    public record MarkEventAsProcessedRequest(
        int Id
    );

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

@"UPDATE POGO_RAIDS_META
SET RAID = 1
WHERE RAID_ID = @p0 AND RAID = 0";

                        command.CommandText = query;
                        command.Parameters.Add(new MySqlParameter("@p0", request.Id));

                        return command.ExecuteNonQuery() == 1;
                    }
                }
            }

        }
    }
}
