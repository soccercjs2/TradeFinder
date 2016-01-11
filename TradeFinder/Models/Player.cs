using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeFinder.Models
{
    public abstract class Player
    {
        public int PlayerId { get; set; }
        public string SessionId { get; set; }
        public int LeagueId { get; set; }
        public int? TeamId { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string NflTeam { get; set; }
        public string NflAlternateTeam { get; set; }
        public decimal Points { get; set; }
        public decimal TradeValue { get; set; }

        public virtual League League { get; set; }
        public virtual Team Team { get; set; }
    }
}