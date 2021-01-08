using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;

namespace EventBot.DataAccess.Commands.Raid
{
    public record AddRaidsRequest(
        IEnumerable<PogoRaids> Raids
    );

    public interface IAddRaidsCommand : ICommand<AddRaidsRequest>
    {
    }
    public class AddRaidsCommand : IAddRaidsCommand
    {
        readonly DatabaseFactory databaseFactory;

        public AddRaidsCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(AddRaidsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                //var raids = request.Raids.Select(x => new POGO_RAIDS { GYM_ID = gyms[x.gym_id].ID, START = EventBot.Models.GoMap.Helper.GoMapTimeToUtc(x.rs), FINISHED = EventBot.Models.GoMap.Helper.GoMapTimeToUtc(x.re), LEVEL = x.lvl, POKE_ID = x.rpid }).ToList();

                db.PogoRaids.AddRange(request.Raids);
                db.SaveChanges();
            }
        }
    }
}
