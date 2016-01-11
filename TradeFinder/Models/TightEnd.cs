using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeFinder.Models
{
    public class TightEnd : Player
    {
        public decimal Receptions { get; set; }
        public decimal ReceivingYards { get; set; }
        public decimal ReceivingTouchdowns { get; set; }

        public virtual Player Player { get; set; }
    }
}