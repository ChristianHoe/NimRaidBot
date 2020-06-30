using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class EventSetups
    {
        public long ChatId { get; set; }
        public long MessageId { get; set; }
        public long? TargetChatId { get; set; }
        public int? LocationId { get; set; }
        public int? Type { get; set; }
        public DateTime? Start { get; set; }
        public bool Modified { get; set; }
    }
}
