using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Sunvalley_PLSystem.Startup))]
namespace Sunvalley_PLSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
