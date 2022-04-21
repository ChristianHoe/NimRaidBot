using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Commands.Minun
{
    public record AddPokeToNotificationListRequest(
        long ChatId,
        int PokeId,
        char? Gender,
        int? IV
    );

    public interface IAddPokeToNotificationListCommand : ICommand<AddPokeToNotificationListRequest>
    {
    }

    public class AddPokeToNotificationList : IAddPokeToNotificationListCommand
    {
        readonly DatabaseFactory databaseFactory;

        public AddPokeToNotificationList(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(AddPokeToNotificationListRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoChatPokes.SingleOrDefault(x => x.ChatId == request.ChatId && x.PokeId == request.PokeId);

                if (result == null)
                {
                    var add = new PogoChatPoke
                    {
                        PokeId = request.PokeId,
                        ChatId = request.ChatId,
                        Gender = request.Gender,
                        Iv = request.IV,
                    };

                    db.PogoChatPokes.Add(add);
                    db.SaveChanges();
                }
            }
        }
    }
}
