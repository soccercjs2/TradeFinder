using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using TradeFinder.Models;

namespace TradeFinder.Data
{
    public class TradeFinderContext : DbContext
    {
        public TradeFinderContext() : base("TradeFinderDb")
        {

        }

        public DbSet<League> Leagues { get; set; }
        public DbSet<LeagueHost> LeagueHosts { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Quarterback> Quarterbacks { get; set; }
        public DbSet<RunningBack> RunningBacks { get; set; }
        public DbSet<WideReceiver> WideReceivers { get; set; }
        public DbSet<TightEnd> TightEnds { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}