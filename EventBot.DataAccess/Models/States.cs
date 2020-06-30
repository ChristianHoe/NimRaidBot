﻿using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class States
    {
        public long UserId { get; set; }
        public string? Command { get; set; }
        public int Step { get; set; }
        public int Level { get; set; }
    }
}
