using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public class SetMinRaidLevelRequest
    {
        public long ChatId;
        public int? RaidLevel;
    }

    public interface ISetMinRaidLevelCommand : ICommand<SetMinRaidLevelRequest>
    {
    }

    public class SetMinRaidLevelCommand : ISetMinRaidLevelCommand
    {
        readonly DatabaseFactory databaseFactory;

        public SetMinRaidLevelCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(SetMinRaidLevelRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoRaidUsers.SingleOrDefault(x => x.ChatId == request.ChatId);

                if (result != null)
                {
                    result.RaidLevel = request.RaidLevel;
                    db.SaveChanges();
                }
            }
        }
    }
}
