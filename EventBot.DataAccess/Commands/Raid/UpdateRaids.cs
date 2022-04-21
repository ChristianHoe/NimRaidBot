using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public record UpdateRaidsRequest(
        IEnumerable<PogoRaid> Raids
    );

    public interface IUpdateRaidsCommand : ICommand<UpdateRaidsRequest>
    {
    }
    public class UpdateRaidsCommand : IUpdateRaidsCommand
    {
        readonly DatabaseFactory databaseFactory;

        public UpdateRaidsCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(UpdateRaidsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                foreach (var raid in request.Raids)
                {
                    var current = db.PogoRaids.SingleOrDefault(x => x.Id == raid.Id);
                    if (current != null)
                    {
                        current.PokeId = raid.PokeId;
                        current.Move2 = raid.Move2;
                        current.PokeForm = raid.PokeForm;
                    }
                }

                db.SaveChanges();
            }
        }
    }
}
