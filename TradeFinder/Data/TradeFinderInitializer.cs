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
            List<League> leagues = GetLeagueList(leagueHosts[0]);
            List<Team> teams = GetTeamList(leagues[0]);

            foreach (LeagueHost leagueHost in leagueHosts)
            {
                context.LeagueHosts.Add(leagueHost);
            }

            foreach (League league in leagues)
            {
                context.Leagues.Add(league);
            }

            foreach (Team team in teams)
            {
                context.Teams.Add(team);
            }

            context.SaveChanges();
        }

        private List<LeagueHost> GetLeagueHostList()
        {
            List<LeagueHost> leagueHosts = new List<LeagueHost>
            {
                new LeagueHost{Name = "FleaFlicker", LoginUrl = "http://www.fleaflicker.com/nfl/login", PostData = "email={0}&password={1}", StarterTableName = "table_0" }
            };

            return leagueHosts;
        }

        private List<League> GetLeagueList(LeagueHost leagueHost)
        {
            List<League> leagues = new List<League>
            {
                new League { LeagueHost = leagueHost, Name = "NarFFL Premier - Bacon North", UserId = "d00d5632-d276-4b1a-a040-e36d7e544f51", AddedOn = DateTime.Now, UserName = "soccercjs2@gmail.com", Password = "united2" }
            };

            return leagues;
        }

        private List<Team> GetTeamList(League league)
        {
            List<Team> teams = new List<Team>
            {
                new Team { League = league, Name = "imfilichino's Giant Niner Warriors", AddedOn = DateTime.Now, Url = "http://www.fleaflicker.com/nfl/leagues/124676/teams/996071", MyTeam = false },
                new Team { League = league, Name = "Guamvaughan Says BelichickYoSelf", AddedOn = DateTime.Now, Url = "http://www.fleaflicker.com/nfl/leagues/124676/teams/1082471", MyTeam = false },
                new Team { League = league, Name = "Trimblco's Claret and Blue Army", AddedOn = DateTime.Now, Url = "http://www.fleaflicker.com/nfl/leagues/124676/teams/1082496", MyTeam = false },
                new Team { League = league, Name = "JezusGhoti's Penetrators", AddedOn = DateTime.Now, Url = "http://www.fleaflicker.com/nfl/leagues/124676/teams/878949", MyTeam = false },
                new Team { League = league, Name = "DeaconBlues Return of the Mack", AddedOn = DateTime.Now, Url = "http://www.fleaflicker.com/nfl/leagues/124676/teams/1172465", MyTeam = false },
                new Team { League = league, Name = "JoeFarish's #TeamPepe", AddedOn = DateTime.Now, Url = "http://www.fleaflicker.com/nfl/leagues/124676/teams/879996", MyTeam = false },
                new Team { League = league, Name = "Soccercjs2's Gods Among Men", AddedOn = DateTime.Now, Url = "http://www.fleaflicker.com/nfl/leagues/124676/teams/878935", MyTeam = true },
                new Team { League = league, Name = "Westexasman's Pack Attack", AddedOn = DateTime.Now, Url = "http://www.fleaflicker.com/nfl/leagues/124676/teams/879409", MyTeam = false },
                new Team { League = league, Name = "Mvparis's Ace Boogies", AddedOn = DateTime.Now, Url = "http://www.fleaflicker.com/nfl/leagues/124676/teams/996332", MyTeam = false },
                new Team { League = league, Name = "jda06's The Ickey Shuffle", AddedOn = DateTime.Now, Url = "http://www.fleaflicker.com/nfl/leagues/124676/teams/988629", MyTeam = false },
                new Team { League = league, Name = "StruggleBunny's Convicts", AddedOn = DateTime.Now, Url = "http://www.fleaflicker.com/nfl/leagues/124676/teams/878899", MyTeam = false },
                new Team { League = league, Name = "Carthac's Brown on Brown Crime", AddedOn = DateTime.Now, Url = "http://www.fleaflicker.com/nfl/leagues/124676/teams/1172448", MyTeam = false }
            };

            return teams;
        }
    }   
}