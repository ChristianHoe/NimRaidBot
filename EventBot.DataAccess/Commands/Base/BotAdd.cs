using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Commands.Base
{
    public sealed record BotAddRequest(
        long ChatId,
        long? BotId
    );

    public interface IBotAddCommand : ICommand<BotAddRequest>
    {
    }

    public sealed class BotAdd : IBotAddCommand
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
                var result = db.RelChatBots.SingleOrDefault(x => x.ChatId == request.ChatId && x.BotId == request.BotId);

                if (result == null)
                {
                    db.RelChatBots.Add(new RelChatBot { ChatId = request.ChatId, BotId = request.BotId });
                    db.SaveChanges();
                }
            }
        }
    }
}
