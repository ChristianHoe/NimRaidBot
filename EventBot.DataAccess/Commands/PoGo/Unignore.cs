using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Commands.PoGo
{
    public class UnignoreRequest
    {
        public long UserId;
        public int MonsterId;
    }

    public interface IUnignoreCommand : ICommand<UnignoreRequest>
    {
    }

    public class Unignore : IUnignoreCommand
    {
        readonly DatabaseFactory databaseFactory;

        public Unignore(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(UnignoreRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoIgnore.SingleOrDefault(x => x.UserId == request.UserId && x.MonsterId == request.MonsterId);

                if (result != null)
                {
                    db.PogoIgnore.Remove(result);
                    db.SaveChanges();
                }
            }
        }
    }
}
