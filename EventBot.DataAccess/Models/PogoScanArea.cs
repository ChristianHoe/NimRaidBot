using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class PogoScanArea
    {
        public int Id { get; set; }
        public bool? Active { get; set; }
        public decimal? LonMin { get; set; }
        public decimal? LonMax { get; set; }
        public decimal? LatMin { get; set; }
        public decimal? LatMax { get; set; }
        public int MapId { get; set; }
    }
}
