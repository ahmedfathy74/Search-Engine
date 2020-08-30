using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Finalmodule.Startup))]
namespace Finalmodule
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
