using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SAAsProject.Startup))]
namespace SAAsProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
