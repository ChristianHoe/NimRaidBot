using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;

namespace EventBot.DataAccess.Commands.PoGo
{
    public record NewPollRequest(
        long ChatId,
        long MessageId,
        int? RaidId = null,
        int? EventId = null,
        int? TimeOffsetId = null
    );

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
                var poll = new ActivePoll { ChatId = request.ChatId, MessageId = request.MessageId, Deleted = false, TimeOffsetId = request.TimeOffsetId };
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
