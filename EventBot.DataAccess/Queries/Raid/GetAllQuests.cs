using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public record GetAllQuestsRequest
    {
        //public DateTime Date;
    }

    public interface IGetAllQuestsQuery : IQuery<GetAllQuestsRequest, IEnumerable<PogoQuest>>
    {
    }

    public class GetAllQuests : IGetAllQuestsQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetAllQuests(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoQuest> Execute(GetAllQuestsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoQuests.ToList();
            }
        }
    }
}