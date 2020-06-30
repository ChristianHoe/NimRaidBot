using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class Eventtypes
    {
        public long GroupId { get; set; }
        public int Id { get; set; }
        public int Threshold { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
