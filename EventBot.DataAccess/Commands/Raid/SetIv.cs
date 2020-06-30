using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public class SetIvRequest
    {
        public long ChatId;
        public int Iv;
    }

    public interface ISetIvCommand : ICommand<SetIvRequest>
    {
    }

    public class SetIv : ISetIvCommand
    {
        readonly DatabaseFactory databaseFactory;

        public SetIv(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(SetIvRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoRaidUsers.SingleOrDefault(x => x.ChatId == request.ChatId);

                if (result != null)
                {
                    result.MinPokeLevel = (100 >= request.Iv && request.Iv >= 0) ? request.Iv : (int?)null;
                    db.SaveChanges();
                }
            }
        }
    }
}
