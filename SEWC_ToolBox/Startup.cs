using Microsoft.Owin;
using Owin;
[assembly: OwinStartupAttribute(typeof(SEWC_ToolBox.Startup))]
namespace SEWC_ToolBox
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
            ConfigureAuth1(app);
        }
    }
}