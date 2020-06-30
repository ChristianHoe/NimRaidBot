﻿using EventBot.DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace EventBot.DataAccess.Queries.Farm
{
    public class MarkPollAsProcessingRequest
    {
        public int Id;
    }

    public interface IMarkPollAsProcessingQuery : IQuery<MarkPollAsProcessingRequest, bool>
    {
    }

    public class MarkPollAsProcessing : IMarkPollAsProcessingQuery
    {
        readonly DatabaseFactory databaseFactory;

        public MarkPollAsProcessing(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public bool Execute(MarkPollAsProcessingRequest request)
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
SET FARM = 0
WHERE POLL_ID = @p0 AND FARM is NULL";

                        command.CommandText = query;
                        command.Parameters.Add(new MySqlParameter("@p0", request.Id));
                        return command.ExecuteNonQuery() == 1;
                    }
                }
            }

        }
    }
}
