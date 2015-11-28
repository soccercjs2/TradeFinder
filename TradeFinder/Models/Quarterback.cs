﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeFinder.Models
{
    public class Quarterback
    {
        public int QuarterbackId { get; set; }
        public int PlayerId { get; set; }
        public string PassingAttempts { get; set; }
        public decimal PassingTouchdowns { get; set; }
        public decimal Interceptions { get; set; }
        public decimal RushingAttempts { get; set; }
        public decimal RushingYards { get; set; }
        public decimal RushingTouchdowns { get; set; }

        public virtual Player Player { get; set; }
    }
}