using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public record SetPokeIdForManualRaidRequest(
        long UserId,
        int PokeId,
        char? PokeForm
    );

    public interface ISetPokeIdForManualRaidCommand : ICommand<SetPokeIdForManualRaidRequest>
    {
    }

    public class SetPokeIdForManualRaidCommand : ISetPokeIdForManualRaidCommand
    {
        readonly DatabaseFactory databaseFactory;

        public SetPokeIdForManualRaidCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(SetPokeIdForManualRaidRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoUserRaids.SingleOrDefault(x => x.UserId == request.UserId);

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
