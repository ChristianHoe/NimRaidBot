using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Commands.PoGo
{
    public class AnRequest
    {
        public long UserId;
        public long BotId;
    }

    public interface IAnCommand : ICommand<AnRequest>
    {
    }

    public class An : IAnCommand
    {
        readonly DatabaseFactory databaseFactory;

        public An(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(AnRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoUser.SingleOrDefault(x => x.UserId == request.UserId);

                if (result == null)
                {
                    result = _Helper.CreateNewUser(db, request.UserId);
                }

                if (result != null)
                {
                    result.Active = true;
                    db.SaveChanges();
                }

                var active = db.RelChatBot.SingleOrDefault(x => x.ChatId == request.UserId && x.BotId == request.BotId);
                if (active != null)
                {
                    active.AllowNotification = true;
                    db.SaveChanges();
                }
            }
        }
    }
}
