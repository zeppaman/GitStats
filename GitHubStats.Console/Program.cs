using Octokit;
using Octokit.Internal;
using Octokit.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubStats
{
    class Program
    {
        static void Main(string[] args)
        {

            var client = new GitHubClient(new ProductHeaderValue("MyAmazingApp"));

            var user = client.User.Get("zeppaman").Result;
            var repos = client.Repository.GetAllForUser(user.Login).Result;
            var apiConnection = new ApiConnection(client.Connection);
            var releases = new ReleasesClient(apiConnection);

            foreach (var v in repos)
            {
                Console.WriteLine(v.Name);

              
                var items = releases.GetAll(v.Id).Result;
                var latest = items[0];
                Console.WriteLine(
                    "The latest release is tagged at {0} and is named {1}",
                    latest.TagName,
                    latest.Name);
            }
            Console.WriteLine(user.Followers + " folks love the half ogre!");
        }
    }
}
