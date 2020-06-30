using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class PogoGamePokesAnswers
    {
        public long ChatId { get; set; }
        public int MessageId { get; set; }
        public long UserId { get; set; }
        public DateTime Created { get; set; }
        public int Choice { get; set; }
        public string? UserName { get; set; }
    }
}
