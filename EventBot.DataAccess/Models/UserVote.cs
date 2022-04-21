using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class UserVote
    {
        public int PollId { get; set; }
        public long UserId { get; set; }
        public string? Time { get; set; }
        public int Attendee { get; set; }
        public int? Comment { get; set; }
    }
}
