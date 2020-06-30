using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class PogoRaidUsers
    {
        public long ChatId { get; set; }
        public bool? Active { get; set; }
        public decimal? LonMin { get; set; }
        public decimal? LonMax { get; set; }
        public decimal? LatMin { get; set; }
        public decimal? LatMax { get; set; }
        public int? RaidLevel { get; set; }
        public int? CleanUp { get; set; }
        public string? Name { get; set; }
        public bool? Ingress { get; set; }
        public int? MinPokeLevel { get; set; }
        public int? TimeOffsetId { get; set; }
        public int? RoundToMinute { get; set; }
        public bool? KickInactive { get; set; }
    }
}
