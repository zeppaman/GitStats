using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GitHubStats.Startup))]
namespace GitHubStats
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
