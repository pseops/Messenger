using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TestTask_TI.Startup))]
namespace TestTask_TI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
