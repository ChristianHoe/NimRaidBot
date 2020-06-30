using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class PogoChatPoke
    {
        public long ChatId { get; set; }
        public int PokeId { get; set; }
        public bool Show { get; set; }
        public string? Gender { get; set; }
        public int? Iv { get; set; }
    }
}
