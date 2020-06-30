using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class PogoIgnore
    {
        public long UserId { get; set; }
        public int MonsterId { get; set; }
    }
}
