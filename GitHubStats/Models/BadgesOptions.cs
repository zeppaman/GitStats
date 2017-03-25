using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Octokit;

namespace GitHubStats.Models
{
    public class BadgesOptions
    {
        public IReadOnlyList<Octokit.Repository> Repositories { get; set; }
        public string BaseUrl{get;set;}
        public User CurrentUser { get; set; }

        public BadgesOptions()
        {
            
        }
    }
}