using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using HtmlAgilityPack;

namespace TradeFinder.Models
{
    public class Team
    {
        public int TeamId { get; set; }
        public int LeagueId { get; set; }
        public string Name { get; set; }
        public bool MyTeam { get; set; }
        public DateTime AddedOn { get; set; }
        public string Url { get; set; }

        public virtual League League { get; set; }
    }
}