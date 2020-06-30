using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Commands.PoGo
{
    public class IgnoreRequest
    {
        public long UserId;
        public int MonsterId;
    }

    public interface IIgnoreCommand : ICommand<IgnoreRequest>
    {
    }

    public class Ignore : IIgnoreCommand
    {
        readonly DatabaseFactory databaseFactory;

        public Ignore(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(IgnoreRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoIgnore.SingleOrDefault(x => x.UserId == request.UserId && x.MonsterId == request.MonsterId);

                if (result == null)
                {
                    var ignore = new PogoIgnore
                    {
                        MonsterId = request.MonsterId,
                        UserId = request.UserId
                    };

                    db.PogoIgnore.Add(ignore);
                    db.SaveChanges();
                }
            }
        }
    }
}
