using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class PogoRaidPreference
    {
        public long ChatId { get; set; }
        public int PokeId { get; set; }
    }
}
