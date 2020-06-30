using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using EventBot.DataAccess.ModelsEx;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public class GetSpecialGymsForChatsRequest
    {
        public long[] ChatIds;
        public GymType? Type;
    }

    public interface IGetSpecialGymsForChatsQuery : IQuery<GetSpecialGymsForChatsRequest, IEnumerable<PogoSpecialGyms>>
    {
    }

    public class GetSpecialGymsForChats : IGetSpecialGymsForChatsQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetSpecialGymsForChats(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoSpecialGyms> Execute(GetSpecialGymsForChatsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                if (request.Type.HasValue)
                    return db.PogoSpecialGyms.Where(x => request.ChatIds.Contains(x.ChatId) && x.Type == (int)request.Type).ToList();

                return db.PogoSpecialGyms.Where(x => request.ChatIds.Contains(x.ChatId)).ToList();
            }
        }
    }
}
