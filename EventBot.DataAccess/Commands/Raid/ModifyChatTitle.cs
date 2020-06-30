using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Commands.Raid
{
    public class ModifyChatTitleRequest
    {
        public long ChatId;
        public string Name;
    }

    public interface IModifyChatTitleCommand : ICommand<ModifyChatTitleRequest>
    {
    }

    public class ModifyChatTitle : IModifyChatTitleCommand
    {
        readonly DatabaseFactory databaseFactory;

        public ModifyChatTitle(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(ModifyChatTitleRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var result = db.PogoRaidUsers.SingleOrDefault(x => x.ChatId == request.ChatId);

                if (result != null)
                {
                    result.Name = request.Name.Length > 100 ? request.Name.Substring(0, 100) : request.Name;
                    db.SaveChanges();
                }
            }
        }
    }
}
