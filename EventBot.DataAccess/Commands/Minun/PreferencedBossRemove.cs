using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Minun
{
    public record PreferencedBossRemoveRequest(
        long ChatId,
        int PokeId
    );

    public interface IPreferencedBossRemoveCommand : ICommand<PreferencedBossRemoveRequest>
    {
    }
    public class PreferencedBossRemoveCommand : IPreferencedBossRemoveCommand
    {
        readonly DatabaseFactory databaseFactory;

        public PreferencedBossRemoveCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(PreferencedBossRemoveRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var preference = db.PogoRaidPreferences.SingleOrDefault(x => x.ChatId == request.ChatId && x.PokeId == request.PokeId);

                if (preference != null)
                {
                    db.PogoRaidPreferences.Remove(preference);
                    db.SaveChanges();
                }
            }
        }
    }
}
