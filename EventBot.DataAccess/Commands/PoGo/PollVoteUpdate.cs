﻿using EventBot.DataAccess.Database;
using EventBot.DataAccess.Models;
using System.Linq;

namespace EventBot.DataAccess.Commands.PoGo
{
    public record PollVoteUpdateRequest(
        long ChatId,
        long MessageId,
        long UserId,
        int Attendee,
        string? Time,
        int? Comment,

        ActivePolls Poll
    );

    public interface IPollVoteUpdateCommand : ICommand<PollVoteUpdateRequest>
    {
    }

    public class PollVoteUpdate : IPollVoteUpdateCommand
    {
        readonly DatabaseFactory databaseFactory;

        public PollVoteUpdate(DatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
        }


        public void Execute(PollVoteUpdateRequest request)
        {
            using (var db = databaseFactory.CreateNew())
            {
                var vote = db.UserVotes.SingleOrDefault(x => x.PollId == request.Poll.Id && x.UserId == request.UserId);
                if (vote != null)
                { 
                    vote.Attendee = request.Attendee;
                    vote.Time = request.Time;
                    vote.Comment = request.Comment;
                }
                else
                {
                    db.UserVotes.Add(new UserVotes { PollId = request.Poll.Id, UserId = request.UserId, Attendee = request.Attendee, Time = request.Time, Comment = request.Comment });
                }

                db.SaveChanges();
            }
        }
    }
}
