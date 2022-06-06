using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Minun
{
    public sealed record PreferencedBossAddRequest(
        long ChatId,
        int PokeId
    );

    public interface IPreferencedBossAddCommand : ICommand<PreferencedBossAddRequest>
    {
    }
    public sealed class PreferencedBossAddCommand : IPreferencedBossAddCommand
    {
        readonly DatabaseFactory databaseFactory;

        public PreferencedBossAddCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(PreferencedBossAddRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var preferences = db.PogoRaidPreferences.FirstOrDefault(x => x.ChatId == request.ChatId && x.PokeId == request.PokeId);
                if (preferences == null)
                {
                    db.PogoRaidPreferences.Add(new Models.PogoRaidPreference { ChatId = request.ChatId, PokeId = request.PokeId });
                    db.SaveChanges();
                }
            }
        }
    }
}
