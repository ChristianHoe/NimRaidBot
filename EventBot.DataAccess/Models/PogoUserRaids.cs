using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class PogoUserRaids
    {
        public long UserId { get; set; }
        public int? GymId { get; set; }
        public int? TimeMode { get; set; }
        public DateTime? Start { get; set; }
        public int? PokeId { get; set; }
        public int? Level { get; set; }
        public long? ChatId { get; set; }
        public int? RaidId { get; set; }
        public int UpdRaidId { get; set; }
        public string? Title { get; set; }
    }
}
