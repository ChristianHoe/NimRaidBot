﻿using System;
using System.Collections.Generic;

namespace EventBot.DataAccess.Models
{
    public partial class IngrEventsMeta
    {
        public int EventId { get; set; }
        public DateTime Created { get; set; }
        public bool? Farm { get; set; }
    }
}
