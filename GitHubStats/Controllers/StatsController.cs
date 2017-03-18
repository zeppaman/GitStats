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
        public ActionResult Badges()
        {

            var accessToken = Session["OAuthToken"] as string;
            if (accessToken != null)
            {
                GitHubClient client = new GitHubClient(new ProductHeaderValue(global::GitHubStats.Controllers.AppConfig.Current.AppName));
                client.Credentials = new Credentials(accessToken);



                var repositories = client.Repository.GetAllForCurrent().Result;
                var user = client.User.Current().Result;
                ViewBag.UserName = user.Name;
                ViewBag.AvatarUrl = user.AvatarUrl;
            }


                return View(new BadgesOptions()
            {
                


            });
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
                ViewBag.UserName = user.Name;
                ViewBag.AvatarUrl = user.AvatarUrl;
                

                var totalDownload = 0;
                var totalStars = 0;
                var repoDownload = 0;
                var releasesCount = 0;
                

                List<RepoStats> repoModel = new List<RepoStats>();
                foreach (var v in repositories)
                {
                    repoDownload = 0;
                    releasesCount = 0;
                  
                    var relase = client.Repository.Release.GetAll(v.Id).Result;
                    if (relase.Count > 0)
                    {
                        for (int i = 0; i < relase.Count; i++)
                        {
                            for (int k = 0; k < relase[i].Assets.Count; k++)
                            {
                                totalDownload += relase[i].Assets[k].DownloadCount;
                                repoDownload += relase[i].Assets[k].DownloadCount;
                                releasesCount++;
                            }
                        }
                       

                    }

                    

                    repoModel.Add(new RepoStats()
                    {
                        Repo = v,
                        TotalDownload = repoDownload,
                        ReleasesCount= releasesCount
                    });

                }

                

                return View(new UserStats()
                {
                    Repositories = repoModel,
                    User= user,
                    TotalDownload= totalDownload,
                    TotalReleases=repoModel.Sum(x=>x.ReleasesCount),
                    TotalStars=repoModel.Sum(x=>x.Repo.StargazersCount),
                     DiskUsage= repoModel.Sum(x=>x.Repo.Size)


                } );

               
            }
            return new RedirectResult("~/");
        }



    }
}