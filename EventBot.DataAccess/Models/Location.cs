using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class Location
    {
        public int Id { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Name { get; set; } = null!;
        public int Type { get; set; }
        public int Order { get; set; }
    }
}
