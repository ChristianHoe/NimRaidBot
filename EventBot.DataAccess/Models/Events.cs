using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class Events
    {
        public int Id { get; set; }
        public long OwnerId { get; set; }
        public int GroupId { get; set; }
        public int EventtypeId { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
    }
}
