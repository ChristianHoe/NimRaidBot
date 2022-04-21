using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class PogoPoke
    {
        public int Id { get; set; }
        public int PokeId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime Finished { get; set; }
        public int? Iv { get; set; }
        public int? Cp { get; set; }
        public int? Gender { get; set; }
        public int? Level { get; set; }
        public int? MapId { get; set; }
        public int? WeatherBoosted { get; set; }
    }
}
