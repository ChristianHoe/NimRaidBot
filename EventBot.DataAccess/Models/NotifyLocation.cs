using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class NotifyLocation
    {
        public long ChatId { get; set; }
        public int LocationId { get; set; }
    }
}
