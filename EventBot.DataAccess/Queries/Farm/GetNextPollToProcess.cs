﻿using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Queries.Farm
{
    public record GetNextPollToProcessRequest();

    public interface IGetNextPollToProcessQuery : IQuery<GetNextPollToProcessRequest, ActivePoll?>
    {
    }

    public class GetNextPollToProcess : IGetNextPollToProcessQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetNextPollToProcess(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public ActivePoll? Execute(GetNextPollToProcessRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.ActivePolls.Where(x => x.EventId != null)
                    .Join(db.ActivePollsMeta.Where(x => x.Farm == null),
                    p => p.Id,
                    m => m.PollId,
                    (p, m) => p
                    ).FirstOrDefault();
            }
        }
    }
}
