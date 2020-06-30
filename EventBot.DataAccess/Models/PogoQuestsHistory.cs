using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class PogoQuestsHistory
    {
        public int StopId { get; set; }
        public DateTime Created { get; set; }
        public string? Task { get; set; }
        public string? Reward { get; set; }
    }
}
