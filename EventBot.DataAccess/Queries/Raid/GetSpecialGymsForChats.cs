using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public sealed record GetSpecialGymsForChatsRequest(
        long[] ChatIds
    );

    public interface IGetSpecialGymsForChatsQuery : IQuery<GetSpecialGymsForChatsRequest, IEnumerable<PogoSpecialGym>>
    {
    }

    public sealed class GetSpecialGymsForChats : IGetSpecialGymsForChatsQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetSpecialGymsForChats(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoSpecialGym> Execute(GetSpecialGymsForChatsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoSpecialGyms.Where(x => request.ChatIds.Contains(x.ChatId)).ToList();
            }
        }
    }
}
