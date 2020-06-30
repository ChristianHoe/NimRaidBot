using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using EventBot.DataAccess.ModelsEx;
using EventBot.DataAccess.Queries.PoGo;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public class GetUserVoteRequest
    {
        public long ChatId;
        public long MessageId;
        public long UserId;
        public ActivePolls Poll;
    }

    public interface IGetUserVoteQuery : IQuery<GetUserVoteRequest, PollVoteResponse>
    {
    }

    public class GetUserVote : IGetUserVoteQuery
    {
        readonly DatabaseFactory databaseFactory;

        public GetUserVote(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public PollVoteResponse Execute(GetUserVoteRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                if (request.Poll != null)
                {
                    return db.UserVotes.Where(x => x.PollId == request.Poll.Id && x.UserId == request.UserId).Join(
                        db.PogoUser,
                        v => v.UserId,
                        u => u.UserId,
                        (v, u) => new PollVoteResponse { FirstName = u.FirstName, Attendee = v.Attendee, Time = v.Time, Comment = (PogoUserVoteComments?) v.Comment }
                        ).SingleOrDefault();
                }

                return null;
            }
        }
    }
}
