using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HochiSystem.Startup))]
namespace HochiSystem
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
