using System.Data.Entity;
using TradeFinder.Models;

namespace TradeFinder.Data
{
    public class TradeFinderContext : DbContext
    {
        public DbSet<League> Leagues { get; set; }
    }
}