using Octokit;
using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace GitHubStats.Controllers
{

    public class AppConfig
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AppName { get; set; }

        private static AppConfig _current;

        public static AppConfig Current
        {
            get
            { return _current ?? (_current = NewConfig()); }

        }

        private static AppConfig NewConfig()
        {
            var config = new AppConfig();
            config.AppName = ConfigurationSettings.AppSettings["AppName"];
            config.ClientSecret = ConfigurationSettings.AppSettings["ClientSecret"];
            config.ClientId = ConfigurationSettings.AppSettings["ClientId"];
            return config;
        }

        public GitHubClient GetClient()
        {
            var client = new GitHubClient(new ProductHeaderValue(AppName));
            return client;
        }
    }

    public class OAuthController : Controller
    {



        // GET: OAuth
        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// code=92753cc957561014b8ef&state=%5BbKCBfYvK.%2BD_2dc0KLyy%243-
        /// </summary>
        /// <returns></returns>
        public ActionResult Authorize(string code, string state)
        {
            try
            {
                var client = AppConfig.Current.GetClient();


                if (String.IsNullOrEmpty(code))
                    return RedirectToAction("Index");

                var expectedState = Session["CSRF:State"] as string;
                if (state != expectedState) throw new InvalidOperationException("SECURITY FAIL!");
                Session["CSRF:State"] = null;

                var request = new OauthTokenRequest(AppConfig.Current.ClientId, AppConfig.Current.ClientSecret, code);
                var token = client.Oauth.CreateAccessToken(request).Result;
                Session["OAuthToken"] = token.AccessToken;



                return RedirectToAction("Index", "Stats");
            }
            catch (Exception exc)
            {
                Response.Write(exc.Message);
                Response.Write(exc.StackTrace);
                Response.End();
                throw new Exception("Not reachable code");
            }


            
        }


        public ActionResult Login()
        {
            var client = AppConfig.Current.GetClient();
            // NOTE: this is not required, but highly recommended!
            // ask the ASP.NET Membership provider to generate a random value 
            // and store it in the current user's session
            string csrf = Membership.GeneratePassword(24, 1);
            Session["CSRF:State"] = csrf;

            var request = new OauthLoginRequest(AppConfig.Current.ClientId)
            {
                Scopes = { "user", "notifications" },
                State = csrf
            };

            // NOTE: user must be navigated to this URL
            var oauthLoginUrl = client.Oauth.GetGitHubLoginUrl(request);

            return Redirect(oauthLoginUrl.ToString());
        }
    }
}