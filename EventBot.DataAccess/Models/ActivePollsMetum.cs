using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class ActivePollsMetum
    {
        public int PollId { get; set; }
        public DateTime Created { get; set; }
        public bool? Farm { get; set; }
        public bool? Poke { get; set; }
    }
}
