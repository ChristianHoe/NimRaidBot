using EventBot.DataAccess.Database;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public class SetNordRequest
    {
        public long ChatId;
        public decimal Nord;
    }

    public interface ISetNordCommand : ICommand<SetNordRequest>
    {
    }

    public class SetNordCommand : ISetNordCommand
    {
        readonly DatabaseFactory databaseFactory;

        public SetNordCommand(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(SetNordRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoRaidUsers.SingleOrDefault(x => x.ChatId == request.ChatId);

                if (result != null)
                {
                    _Helper.SetNorth(result, request.Nord);
                    db.SaveChanges();
                }
            }
        }
    }
}
