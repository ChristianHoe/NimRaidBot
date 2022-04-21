using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Commands.Farm
{
    public record UpdateEventSetupRequest(
        EventSetup EventSetup
    );

    public interface IUpdateEventSetupCommand : ICommand<UpdateEventSetupRequest>
    {
    }

    public class UpdateEventSetup : IUpdateEventSetupCommand
    {
        readonly DatabaseFactory databaseFactory;

        public UpdateEventSetup(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(UpdateEventSetupRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var current = db.EventSetups.SingleOrDefault(x => x.ChatId == request.EventSetup.ChatId && x.MessageId == request.EventSetup.MessageId);
                if (current == null)
                {
                    db.EventSetups.Add(request.EventSetup);
                }
                else
                {
                    current.ChatId = request.EventSetup.ChatId;
                    current.LocationId = request.EventSetup.LocationId;
                    current.MessageId = request.EventSetup.MessageId;
                    current.Start = request.EventSetup.Start;
                    current.TargetChatId = request.EventSetup.TargetChatId;
                    current.Type = request.EventSetup.Type;
                    current.Modified = request.EventSetup.Modified;
                }

                db.SaveChanges();
            }
        }
    }
}

