using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public enum TeamType
    {
        Blau = 1,
        Rot = 2,
        Gelb = 3
    }

    public sealed record SetUserTeamRequest(
        long UserId,
        TeamType Team
    );

    public interface ISetUserTeamCommand : ICommand<SetUserTeamRequest>
    {
    }

    public sealed class SetUserTeamCommand : ISetUserTeamCommand
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
                var result = db.PogoUsers.SingleOrDefault(x => x.UserId == request.UserId);

                if (result != null)
                {
                    result.Team = (int)request.Team;
                    db.SaveChanges();
                }
            }
        }
    }
}
