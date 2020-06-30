using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Minun
{
    public class GetChatAreasRequest
    {
        public long[] ChatIds;
    }

    public interface IGetChatAreas : IQuery<GetChatAreasRequest, IEnumerable<PogoRaidUsers>>    {
    }

    public class GetChatAreas : IGetChatAreas
    {
        readonly DatabaseFactory databaseFactory;

        public GetChatAreas(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoRaidUsers> Execute(GetChatAreasRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoRaidUsers.Where(x => request.ChatIds.Contains(x.ChatId)).ToList();
            }
        }
    }
}
