using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class PogoRelPokesChat
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public long MessageId { get; set; }
        public long PokeId { get; set; }
        public bool? Deleted { get; set; }
    }
}
