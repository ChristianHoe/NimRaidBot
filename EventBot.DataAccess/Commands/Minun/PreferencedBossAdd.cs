using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Minun
{
    public class PreferencedBossAddRequest
    {
        public long ChatId;
        public int PokeId;
    }

    public interface IPreferencedBossAddCommand : ICommand<PreferencedBossAddRequest>
    {
    }
    public class PreferencedBossAddCommand : IPreferencedBossAddCommand
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
                var preferences = db.PogoRaidPreference.FirstOrDefault(x => x.ChatId == request.ChatId && x.PokeId == request.PokeId);
                if (preferences == null)
                {
                    db.PogoRaidPreference.Add(new Models.PogoRaidPreference { ChatId = request.ChatId, PokeId = request.PokeId });
                    db.SaveChanges();
                }
            }
        }
    }
}
