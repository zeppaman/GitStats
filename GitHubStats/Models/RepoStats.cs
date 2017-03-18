using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Octokit;

namespace GitHubStats.Models
{
    public class RepoStats
    {
        public Repository Repo {get;set;}
        public int TotalDownload { get; set; }
        public int ReleasesCount { get; set; }
    }
}