using EventBot.DataAccess.Database;
using EventBot.DataAccess.ModelsEx;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.PoGo
{
    public class PollVotesRequest
    {
        public long ChatId;
        public long MessageId;
    }

    public class PollVoteResponse
    {
        public long UserId;
        public string? FirstName;
        public string? IngameName;
        public string? IngressName;
        public int? Level;
        public Commands.Raid.TeamType? Team;
        public int Attendee;
        public string? Time;
        public PogoUserVoteComments? Comment;
    }


    public interface IPollVotesUsers : IQuery<PollVotesRequest, IEnumerable<PollVoteResponse>>
    {
    }



    public class PollVotes : IPollVotesUsers
    {
        readonly DatabaseFactory databaseFactory;

        public PollVotes(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public IEnumerable<PollVoteResponse> Execute(PollVotesRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var z = db.ActivePolls.SingleOrDefault(x => x.ChatId == request.ChatId && x.MessageId == request.MessageId);

                if (z != null)
                {
                    return db.UserVotes.Where(x => x.PollId == z.Id).Join(
                        db.PogoUser,
                        v => v.UserId,
                        u => u.UserId,
                        (v, u) => new PollVoteResponse { UserId = u.UserId, FirstName = u.FirstName, IngameName = u.IngameName, IngressName = u.IngressName, Level = u.Level, Team = (Commands.Raid.TeamType?)u.Team, Attendee = v.Attendee, Time = v.Time, Comment = (PogoUserVoteComments?)v.Comment }
                        ).ToList();
                }

                return Enumerable.Empty<PollVoteResponse>();
            }
        }
    }
}
