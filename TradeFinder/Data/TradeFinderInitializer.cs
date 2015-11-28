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

        }
    }
}