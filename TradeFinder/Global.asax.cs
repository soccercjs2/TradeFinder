using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TradeFinder.Data;
using TradeFinder.NumberFire;

namespace TradeFinder
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            Database.SetInitializer(new TradeFinderInitializer());
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Session_Start()
        {
            NumberFireConnection numberFireConnection = new NumberFireConnection();
            Session["Quarterbacks"] = numberFireConnection.Quarterbacks;
            Session["RunningBacks"] = numberFireConnection.RunningBacks;
            Session["WideReceivers"] = numberFireConnection.WideReceivers;
            Session["TightEnds"] = numberFireConnection.TightEnds;
        }
    }
}
