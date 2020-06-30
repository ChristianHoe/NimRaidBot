using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class PogoRaids
    {
        public int Id { get; set; }
        public int GymId { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finished { get; set; }
        public int PokeId { get; set; }
        public int Level { get; set; }
        public long? ChatId { get; set; }
        public int? Move2 { get; set; }
        public long? OwnerId { get; set; }
        public string? Title { get; set; }
    }
}
