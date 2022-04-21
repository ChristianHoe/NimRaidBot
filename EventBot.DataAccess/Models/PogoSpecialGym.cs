using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class PogoSpecialGym
    {
        public int GymId { get; set; }
        public long ChatId { get; set; }
        public int Type { get; set; }
        public string? Data { get; set; }
    }
}
