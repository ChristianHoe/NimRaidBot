using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class Locations
    {
        public int Id { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? Name { get; set; }
        public int Type { get; set; }
        public int Order { get; set; }
    }
}
