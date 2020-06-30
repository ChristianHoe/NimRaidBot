using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public enum TeamType
    {
        Blau = 1,
        Rot = 2,
        Gelb = 3
    }

    public class SetUserTeamRequest
    {
        public long UserId;
        public TeamType Team;
    }

    public interface ISetUserTeamCommand : ICommand<SetUserTeamRequest>
    {
    }

    public class SetUserTeamCommand : ISetUserTeamCommand
    {
        readonly DatabaseFactory databaseFactory;

        public SetUserTeamCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(SetUserTeamRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoUser.SingleOrDefault(x => x.UserId == request.UserId);

                if (result != null)
                {
                    result.Team = (int)request.Team;
                    db.SaveChanges();
                }
            }
        }
    }
}
