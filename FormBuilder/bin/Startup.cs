using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FormBuilder.Startup))]
namespace FormBuilder
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
