using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public record AddChatRequest(
        long ChatId,
        string Name
    );

    public interface IAddChatCommand : ICommand<AddChatRequest>
    {
    }

    public class AddChat : IAddChatCommand
    {
        readonly DatabaseFactory databaseFactory;

        public AddChat(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(AddChatRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoRaidUsers.SingleOrDefault(x => x.ChatId == request.ChatId);

                if (result == null)
                {
                    db.PogoRaidUsers.Add(new PogoRaidUser { ChatId = request.ChatId, Ingress = false, RaidLevel = 1, Active = true, Name = request.Name.Length > 100 ? request.Name.Substring(0, 100) : request.Name });
                    db.SaveChanges();
                }
            }
        }
    }
}
