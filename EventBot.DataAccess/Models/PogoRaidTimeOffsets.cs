using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class PogoRaidTimeOffsets
    {
        public int SettingId { get; set; }
        public int Order { get; set; }
        public int OffsetInMinutes { get; set; }
    }
}
