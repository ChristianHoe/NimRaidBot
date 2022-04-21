using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class PogoGamePoke
    {
        public long ChatId { get; set; }
        public int MessageId { get; set; }
        public int Difficulty { get; set; }
        public DateTime Finish { get; set; }
        public int TargetPokeId { get; set; }
        public int TargetPokeMoveTyp { get; set; }
        public int Choice1PokeId { get; set; }
        public int Choice1PokeMoveTyp { get; set; }
        public int Choice2PokeId { get; set; }
        public int Choice2PokeMoveTyp { get; set; }
        public int Choice3PokeId { get; set; }
        public int Choice3PokeMoveTyp { get; set; }
        public int Choice4PokeId { get; set; }
        public int Choice4PokeMoveTyp { get; set; }
    }
}
