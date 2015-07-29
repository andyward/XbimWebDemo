using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(XbimDemoWebApp.Startup))]
namespace XbimDemoWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
