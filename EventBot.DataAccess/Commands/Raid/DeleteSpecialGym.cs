using EventBot.DataAccess.Database;
using EventBot.DataAccess.ModelsEx;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public record DeleteSpecialGymRequest(
        long ChatId,
        int GymId,
        GymType Type
    );

    public interface IDeleteSpecialGymCommand : ICommand<DeleteSpecialGymRequest>
    {
    }

    public class DeleteSpecialGym : IDeleteSpecialGymCommand
    {
        readonly DatabaseFactory databaseFactory;

        public DeleteSpecialGym(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(DeleteSpecialGymRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoSpecialGyms.SingleOrDefault(x => x.ChatId == request.ChatId && x.GymId == request.GymId && x.Type == (int)request.Type);

                if (result != null)
                {
                    db.PogoSpecialGyms.Remove(result);
                    db.SaveChanges();
                }
            }
        }
    }
}
