using EventBot.DataAccess.Database;
using EventBot.DataAccess.ModelsEx;
using System.Collections.Generic;
using System.Linq;

namespace EventBot.DataAccess.Queries.PoGo
{
    public sealed record PollVotesRequest(
        long ChatId,
        long MessageId
    );

    public sealed record PollVoteResponse(
        long UserId,
        string? FirstName,
        string? IngameName,
        string? IngressName,
        int? Level,
        Commands.Raid.TeamType? Team,
        int Attendee,
        string? Time,
        PogoUserVoteComments? Comment
    );


    public interface IPollVotesUsers : IQuery<PollVotesRequest, IEnumerable<PollVoteResponse>>
    {
    }



    public sealed class PollVotes : IPollVotesUsers
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
                        db.PogoUsers,
                        v => v.UserId,
                        u => u.UserId,
                        (v, u) => new PollVoteResponse(u.UserId, u.FirstName, u.IngameName, u.IngressName, u.Level, (Commands.Raid.TeamType?)u.Team, v.Attendee, v.Time, (PogoUserVoteComments?)v.Comment )
                        ).ToList();
                }

                return Enumerable.Empty<PollVoteResponse>();
            }
        }
    }
}
