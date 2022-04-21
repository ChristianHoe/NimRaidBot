using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Collections.Generic;

namespace EventBot.DataAccess.Commands.Raid
{
    public record AddQuestsRequest(
        IEnumerable<PogoQuest> Quests
    );

    public interface IAddQuestsCommand : ICommand<AddQuestsRequest>
    {
    }
    public class AddQuests : IAddQuestsCommand
    {
        readonly DatabaseFactory databaseFactory;

        public AddQuests(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(AddQuestsRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                db.PogoQuests.AddRange(request.Quests);
                db.SaveChanges();
            }
        }
    }
}
