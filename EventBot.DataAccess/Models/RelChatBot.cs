using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class RelChatBot
    {
        public long ChatId { get; set; }
        public long BotId { get; set; }
        public bool AllowNotification { get; set; }
    }
}
