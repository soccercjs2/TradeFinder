using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeFinder.Models
{
    public class LeagueHost
    {
        public int LeagueHostId { get; set; }
        public string Name { get; set; }
        public string LoginUrl { get; set; }
        public string PostData { get; set; }
        public string StarterTableName { get; set; }
        public string BenchTableName { get; set; }
        public string KickerTableName { get; set; }
        public string DefenseTableName { get; set; }

        public virtual ICollection<League> Leagues { get; set; }
    }
}