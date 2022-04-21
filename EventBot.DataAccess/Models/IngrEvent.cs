using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class IngrEvent
    {
        public int Id { get; set; }
        public int? LocationId { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? Finished { get; set; }
        public long? ChatId { get; set; }
        public int? TypeId { get; set; }
    }
}
