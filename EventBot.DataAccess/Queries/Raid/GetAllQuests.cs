using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public class GetAllQuestsRequest
    {
        //public DateTime Date;
    }

    public interface IGetAllQuestsQuery : IQuery<GetAllQuestsRequest, IEnumerable<PogoQuests>>
    {
    }

    public class GetAllQuests : IGetAllQuestsQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetAllQuests(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoQuests> Execute(GetAllQuestsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoQuests.ToList();
            }
        }
    }
}