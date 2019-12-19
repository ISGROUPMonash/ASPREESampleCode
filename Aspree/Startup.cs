using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Aspree.Startup))]
namespace Aspree
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
