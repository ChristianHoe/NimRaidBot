using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class PogoRelScanChat
    {
        public int ScanAreaId { get; set; }
        public long ChatId { get; set; }
    }
}
