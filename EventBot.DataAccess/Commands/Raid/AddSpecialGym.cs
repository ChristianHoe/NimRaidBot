using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using EventBot.DataAccess.ModelsEx;
using System.Linq;


namespace EventBot.DataAccess.Commands.Raid
{
    public sealed record AddSpecialGymRequest(
        long ChatId,
        int GymId,
        GymType Type
    );

    public interface IAddSpecialGymCommand : ICommand<AddSpecialGymRequest>
    {
    }

    public sealed class AddSpecialGym : IAddSpecialGymCommand
    {
        readonly DatabaseFactory databaseFactory;

        public AddSpecialGym(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(AddSpecialGymRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoSpecialGyms.SingleOrDefault(x => x.ChatId == request.ChatId && x.GymId == request.GymId);

                if (result == null)
                {
                    db.PogoSpecialGyms.Add(new PogoSpecialGym { ChatId = request.ChatId, GymId = request.GymId, Type = (int)request.Type });
                    db.SaveChanges();
                }
            }
        }
    }
}
