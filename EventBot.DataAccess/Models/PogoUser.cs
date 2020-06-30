using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class PogoUser
    {
        public long UserId { get; set; }
        public bool Active { get; set; }
        public decimal? LonMin { get; set; }
        public decimal? LonMax { get; set; }
        public decimal? LatMin { get; set; }
        public decimal? LatMax { get; set; }
        public string? FirstName { get; set; }
        public string? IngameName { get; set; }
        public int? Level { get; set; }
        public int? Team { get; set; }
        public string? IngressName { get; set; }
        public int? GroupMembers { get; set; }
    }
}
