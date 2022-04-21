using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class PogoRaidsMetum
    {
        public int RaidId { get; set; }
        public DateTime Created { get; set; }
        public bool? Raid { get; set; }
    }
}
