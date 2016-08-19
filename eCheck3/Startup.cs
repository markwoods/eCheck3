using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(eCheck3.Startup))]
namespace eCheck3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
