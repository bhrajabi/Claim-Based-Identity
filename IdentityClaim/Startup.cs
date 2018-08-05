using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IdentityClaim.Startup))]
namespace IdentityClaim
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
