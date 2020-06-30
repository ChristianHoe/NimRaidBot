using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class BotFarm
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public long BoardId { get; set; }
    }
}
