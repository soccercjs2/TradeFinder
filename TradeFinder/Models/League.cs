using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeFinder.Models
{
    public class League
    {
        public int LeagueId { get; set; }
        public string CurrentSessionId { get; set; }
        public int LeagueHostId { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public DateTime AddedOn { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public virtual LeagueHost LeagueHost { get; set; }
        public virtual ICollection<Team> Teams { get; set; }
    }
}