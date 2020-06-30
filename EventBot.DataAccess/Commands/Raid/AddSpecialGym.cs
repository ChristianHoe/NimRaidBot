using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using EventBot.DataAccess.ModelsEx;
using System.Linq;


namespace EventBot.DataAccess.Commands.Raid
{
    public class AddSpecialGymRequest
    {
        public long ChatId;
        public int GymId;
        public GymType Type;
    }

    public interface IAddSpecialGymCommand : ICommand<AddSpecialGymRequest>
    {
    }

    public class AddSpecialGym : IAddSpecialGymCommand
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
                    db.PogoSpecialGyms.Add(new PogoSpecialGyms { ChatId = request.ChatId, GymId = request.GymId, Type = (int)request.Type });
                    db.SaveChanges();
                }
            }
        }
    }
}
