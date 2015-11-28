using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeFinder.Models
{
    public class WideReceiver
    {
        public int WideReceiverId { get; set; }
        public int PlayerId { get; set; }
        public decimal RushingAttempts { get; set; }
        public decimal RushingYards { get; set; }
        public decimal RushingTouchdowns { get; set; }
        public decimal Receptions { get; set; }
        public decimal ReceivingYards { get; set; }
        public decimal ReceivingTouchdowns { get; set; }

        public virtual Player Player { get; set; }
    }
}