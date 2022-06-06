using EventBot.DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace EventBot.DataAccess.Queries.Pokes
{
    public sealed record MarkAsProcessingRequest(
        int Id
    );

    public interface IMarkAsProcessingQuery : IQuery<MarkAsProcessingRequest, bool>
    {
    }

    public sealed class MarkAsProcessing : IMarkAsProcessingQuery
    {
        readonly DatabaseFactory databaseFactory;

        public MarkAsProcessing(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public bool Execute(MarkAsProcessingRequest request)
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
SET POKE = 0
WHERE POGO_POKE_ID = @p0 AND POKE is NULL";

                        command.CommandText = query;
                        command.Parameters.Add(new MySqlParameter("@p0", request.Id));
                        return command.ExecuteNonQuery() == 1;
                    }
                }
            }

        }
    }
}
