using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TradeFinder.Models;

namespace TradeFinder.Data
{
    public class TradeFinderInitializer : DropCreateDatabaseIfModelChanges<TradeFinderContext>
    {
        protected override void Seed(TradeFinderContext context)
        {
            List<LeagueHost> leagueHosts = GetLeagueHostList();

            foreach (LeagueHost leagueHost in leagueHosts)
            {
                context.LeagueHosts.Add(leagueHost);
            }

            context.SaveChanges();
        }

        private List<LeagueHost> GetLeagueHostList()
        {
            List<LeagueHost> leagueHosts = new List<LeagueHost>
            {
                new LeagueHost{Name = "FleaFlicker", LoginUrl = "http://www.fleaflicker.com/nfl/login", PostData = "email={0}&password={1}" }
            };

            return leagueHosts;
        }
    }   
}