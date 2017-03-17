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
       
      

        public ActionResult Repository(string repoId)
        {
            return null;

        }
            
        // GET: Stats
        public ActionResult Index()
        {
            var accessToken = Session["OAuthToken"] as string;
            if (accessToken != null)
            {
                GitHubClient client = new GitHubClient(new ProductHeaderValue(global::GitHubStats.Controllers.AppConfig.Current.AppName));
                client.Credentials = new Credentials(accessToken);
                

                var repositories = client.Repository.GetAllForCurrent().Result;
                var user = client.User.Current().Result;

                var totalDownload = 0;
                var totalStars = 0;
                foreach (var v in repositories)
                {
                    


                    var relase = client.Repository.Release.GetAll(v.Id).Result;
                    if (relase.Count > 0)
                    {
                        for (int i = 0; i < relase.Count; i++)
                        {
                            for (int k = 0; k < relase[i].Assets.Count; k++)
                            {
                                totalDownload += relase[i].Assets[k].DownloadCount;
                            }
                        }
                       

                    }
                 
                }



                return View(new UserStats()
                {
                    Repositories = repositories,
                    User= user,
                    TotalDownload= totalDownload


                } );

               
            }
            return new RedirectResult("~/");
        }



    }
}