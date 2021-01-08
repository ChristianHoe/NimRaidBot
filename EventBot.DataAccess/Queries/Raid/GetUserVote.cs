using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using EventBot.DataAccess.ModelsEx;
using EventBot.DataAccess.Queries.PoGo;
using System.Linq;

namespace EventBot.DataAccess.Queries.Raid
{
    public record GetUserVoteRequest(
        long ChatId,
        long MessageId,
        long UserId,
        ActivePolls Poll
    );

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
                        (v, u) => new PollVoteResponse(u.UserId, u.FirstName, u.IngameName, u.IngressName, u.Level, (Commands.Raid.TeamType?)u.Team, v.Attendee, v.Time, (PogoUserVoteComments?) v.Comment)
                        ).SingleOrDefault();
                }

                return null;
            }
        }
    }
}
