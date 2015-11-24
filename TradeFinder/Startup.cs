using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TradeFinder.Startup))]
namespace TradeFinder
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
