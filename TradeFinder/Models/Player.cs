using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeFinder.Models
{
    public class Player
    {
        public int PlayerId { get; set; }
        public int TeamId { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public decimal Points { get; set; }
        public decimal TradeValue { get; set; }
    }
}