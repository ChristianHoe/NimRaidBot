using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class PogoPokesMeta
    {
        public int PogoPokeId { get; set; }
        public DateTime Created { get; set; }
        public bool? Poke { get; set; }
    }
}
