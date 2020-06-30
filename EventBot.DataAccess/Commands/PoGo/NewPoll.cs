using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;

namespace EventBot.DataAccess.Commands.PoGo
{
    public class NewPollRequest
    {
        public long ChatId;
        public long MessageId;
        public int? RaidId;
        public int? EventId;
        public int? TimeOffsetId;
    }

    public interface INewPollCommand : ICommand<NewPollRequest>
    {
    }

    public class NewPoll : INewPollCommand
    {
        readonly DatabaseFactory databaseFactory;

        public NewPoll(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }

        public void Execute(NewPollRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var poll = new ActivePolls { ChatId = request.ChatId, MessageId = request.MessageId, Deleted = false, TimeOffsetId = request.TimeOffsetId };
                if (request.EventId != null)
                    poll.EventId = request.EventId;
                else
                    poll.RaidId = request.RaidId;

                db.ActivePolls.Add(poll);
                db.SaveChanges();
            }
        }
    }
}
