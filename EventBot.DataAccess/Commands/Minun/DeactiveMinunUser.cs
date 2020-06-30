using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventBot.DataAccess.Commands.Minun
{
    public class DeactiveMinunUserRequest
    {
        public long UserId;
        public long BotId;
    }

    public interface IDeactiveMinunUserCommand : ICommand<DeactiveMinunUserRequest>
    {
    }

    public class DeactiveMinunUser : IDeactiveMinunUserCommand
    {
        readonly DatabaseFactory databaseFactory;

        public DeactiveMinunUser(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(DeactiveMinunUserRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.RelChatBot.SingleOrDefault(x => x.ChatId == request.UserId && x.BotId == request.BotId);

                if (result != null)
                {
                    db.Remove(result);
                    db.SaveChanges();
                }
            }
        }
    }
}
