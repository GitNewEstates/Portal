using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Portal_MVC.Startup))]
namespace Portal_MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            

        }
    }
}
