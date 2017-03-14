using GitHubStats.Models;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GitHubStats.Controllers
{
    public class StatsController : Controller
    {
        const string appName = "GItStats";
        static GitHubClient client = new GitHubClient(new ProductHeaderValue(appName));

        // GET: Stats
        public ActionResult Index()
        {
            var accessToken = Session["OAuthToken"] as string;
            if (accessToken != null)
            {
                client.Credentials = new Credentials(accessToken);

                var repositories = client.Repository.GetAllForCurrent().Result;


                return View(new UserStats()
                {
                    Repositories = repositories
                } );

               
            }
            throw new Exception("NON ATTIVO");
        }



    }
}