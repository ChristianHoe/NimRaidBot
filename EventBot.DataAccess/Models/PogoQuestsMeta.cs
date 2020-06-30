using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class PogoQuestsMeta
    {
        public int StopId { get; set; }
        public DateTime Created { get; set; }
        public bool? Processed { get; set; }
    }
}
