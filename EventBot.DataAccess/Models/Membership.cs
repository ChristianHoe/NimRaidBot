using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class Membership
    {
        public long GroupId { get; set; }
        public long UserId { get; set; }
        public DateTime? LastAccess { get; set; }
        public int? SecurityLevel { get; set; }
        public DateTime? Created { get; set; }
    }
}
