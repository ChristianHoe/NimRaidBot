using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public class SetPokeIdForRaidRequest
    {
        public long RaidId;
        public int PokeId;
        public char? PokeForm;
    }

    public interface ISetPokeIdForRaidCommand : ICommand<SetPokeIdForRaidRequest>
    {
    }

    public class SetPokeIdForRaidCommand : ISetPokeIdForRaidCommand
    {
        readonly DatabaseFactory databaseFactory;

        public SetPokeIdForRaidCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(SetPokeIdForRaidRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoRaids.SingleOrDefault(x => x.Id == request.RaidId);

                if (result != null)
                {
                    result.PokeId = request.PokeId;
                    result.PokeForm = request.PokeForm;
                    db.SaveChanges();
                }
            }
        }
    }
}
