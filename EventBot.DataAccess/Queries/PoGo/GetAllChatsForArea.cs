using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.PoGo
{
    public sealed record GetAllChatsForAreaRequest();

    public interface IGetAllChatsForArea : IQuery<GetAllChatsForAreaRequest, IEnumerable<PogoRelScanChat>>
    {
    }

    public sealed class GetAllChatsForArea : IGetAllChatsForArea
    {
        readonly DatabaseFactory databaseFactory;

        public GetAllChatsForArea(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoRelScanChat> Execute(GetAllChatsForAreaRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoRelScanChats.ToList();
            }
        }
    }
}
