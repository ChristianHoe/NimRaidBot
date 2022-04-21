using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class PogoRaidTimeOffset
    {
        public int SettingId { get; set; }
        public int Order { get; set; }
        public int OffsetInMinutes { get; set; }
    }
}
