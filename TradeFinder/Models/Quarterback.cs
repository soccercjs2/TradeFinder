using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeFinder.Models
{
    public class Quarterback : Player
    {
        public decimal PassingCompletions { get; set; }
        public decimal PassingAttempts { get; set; }
        public decimal PassingYards { get; set; }
        public decimal PassingTouchdowns { get; set; }
        public decimal Interceptions { get; set; }
        public decimal RushingAttempts { get; set; }
        public decimal RushingYards { get; set; }
        public decimal RushingTouchdowns { get; set; }

        public virtual Player Player { get; set; }
    }
}