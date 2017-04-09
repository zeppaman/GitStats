using Octokit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using GitHubStats.Models;

namespace GitHubStats.Controllers
{
    public class BadgesController : Controller
    {
        // GET: Badges
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult UserBadge(string login = "")
        {
            if (login == "") throw new Exception("Login Missing");

            var client = GetClientForRequest(this);
            var user=client.User.Get(login).Result;
            var userStats = StatsController.GetUserStats(this, user);

            return View(userStats );

        }

        const string badgeTemplate = "https://img.shields.io/badge/{0}-{1}-{2}.svg";

        /// <summary>
        /// Exposet method that coumpute count and produce images using shield.io service
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RepositoryDownloads( long id=0)
        {
            if (id == 0) throw new Exception("Repository Id Missing");            
            int total=  GetDownloadCountForRepository(id);
            string url = string.Format(badgeTemplate, "downloads", total, "orange");
            return DownloadFile(url, "repositoryDownload.svg", true);

        }

        /// <summary>
        /// Compute download count for a given repository id
        /// Note: all assets of all versiona are summed together
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private int GetDownloadCountForRepository(long id)
        {
            int total = 0;
            GitHubClient client = GetClientForRequest(this);
            var repository = client.Repository.Get(id).Result;
            foreach (var rel in client.Repository.Release.GetAll(id).Result)
            {
                foreach (var asset in rel.Assets)
                {
                    total += asset.DownloadCount;
                }
            }
            return total;
        }

        /// <summary>
        /// Generic methods that donwload  and serves files
        /// </summary>
        /// <param name="url"> url of file to be downloaded</param>
        /// <param name="filename">name of file to be served</param>
        /// <param name="inline">show inline or download as attachment</param>
        /// <returns></returns>
        private ActionResult DownloadFile(string url, string filename, bool inline)
        {
            WebClient client = new WebClient();
            var bytes = client.DownloadData(url);
            string contentType = MimeMapping.GetMimeMapping(filename);
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = filename,
                Inline = inline,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(bytes, contentType);
        }






        public static  GitHubClient GetClientForRequest(Controller context)
        {

            GitHubClient client = new GitHubClient(new ProductHeaderValue(global::GitHubStats.Controllers.AppConfig.Current.AppName));
            var accessToken = context.Session["OAuthToken"] as string ?? ConfigurationManager.AppSettings["AnonymousToken"];

            if (!string.IsNullOrEmpty(accessToken))
            {
                client.Credentials = new Credentials(accessToken);
            }
            else
            {


                var appUsername = ConfigurationManager.AppSettings["AppUsername"];
                var appPassword = ConfigurationManager.AppSettings["AppPassword"];
                if (!string.IsNullOrEmpty(appUsername) && !string.IsNullOrEmpty(appPassword))
                {
                    var basicAuth = new Credentials(appUsername, appPassword);
                    client.Credentials = basicAuth;
                }
            }

            return client;
        }

       
    }

}