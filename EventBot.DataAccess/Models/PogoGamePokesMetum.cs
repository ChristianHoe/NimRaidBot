using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class PogoGamePokesMetum
    {
        public long ChatId { get; set; }
        public int MessageId { get; set; }
        public DateTime Created { get; set; }
        public int State { get; set; }
    }
}
