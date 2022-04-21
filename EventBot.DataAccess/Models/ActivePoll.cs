using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class ActivePoll
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public long MessageId { get; set; }
        public int? RaidId { get; set; }
        public bool Deleted { get; set; }
        public int? EventId { get; set; }
        public int? TimeOffsetId { get; set; }
    }
}
