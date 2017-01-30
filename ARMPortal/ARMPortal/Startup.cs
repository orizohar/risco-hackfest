using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof (ARMPortal.Startup))]
namespace ARMPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);
        }
    }
}