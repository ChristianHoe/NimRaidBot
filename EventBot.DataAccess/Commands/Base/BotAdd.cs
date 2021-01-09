using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Commands.Base
{
    public record BotAddRequest(
        long ChatId,
        long BotId
    );

    public interface IBotAddCommand : ICommand<BotAddRequest>
    {
    }

    public class BotAdd : IBotAddCommand
    {
        readonly DatabaseFactory databaseFactory;

        public BotAdd(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(BotAddRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.RelChatBot.SingleOrDefault(x => x.ChatId == request.ChatId && x.BotId == request.BotId);

                if (result == null)
                {
                    db.RelChatBot.Add(new RelChatBot { ChatId = request.ChatId, BotId = request.BotId });
                    db.SaveChanges();
                }
            }
        }
    }
}
