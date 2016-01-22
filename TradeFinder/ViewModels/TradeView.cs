using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeFinder.ViewModels
{
    public class TradeView
    {
        public string MyPlayersHtml { get; set; }
        public string TheirPlayersHtml { get; set; }
        public decimal MyDifferential { get; set; }
        public decimal TheirDifferential { get; set; }
        public decimal Fairness { get; set; }
    }
}