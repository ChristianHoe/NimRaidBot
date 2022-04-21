using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public record GetAllRaidsRequest(
        DateTime Date
    );

    public interface IGetAllRaidsQuery : IQuery<GetAllRaidsRequest, IEnumerable<PogoRaid>>
    {
    }

    public class GetAllRaids : IGetAllRaidsQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetAllRaids(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PogoRaid> Execute(GetAllRaidsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                return db.PogoRaids.Where(x => x.Finished >= request.Date).ToList();
                    
                    //.Join(
                    //db.POGO_GYMS,
                    //r => r.GYM_ID,
                    //g => g.ID,
                    //(r, g) => new { GoMapId = g.GOMAP_ID ?? 0, Until = r.FINISHED }
                    //)
                    //.Where(x => x.Until >= request.Date)
                    //.Select(x => x.GoMapId)
                    //.ToList();
            }
        }
    }
}
