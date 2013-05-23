using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using RestSharp;
using TweeWebb.Models;

namespace TweeWebb.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Thanks()
        {
            return View();
        }


        public JsonResult GetIssues()
        {
            var client = new RestClient("https://api.bitbucket.org/1.0/");
            client.Authenticator = new HttpBasicAuthenticator(Secret.Username, Secret.Password);
            var request = new RestRequest("repositories/_booty/twee/issues/");
            var response = client.Execute<BitBucketResponse>(request);

            var issues = response.Data.Issues.ToList();

            foreach (var issue in issues)
            {
                issue.resource_uri = issue.resource_uri.Replace("/1.0/repositories/", "https://bitbucket.org/").Replace("issues","issue");
            }
            return Json(issues, JsonRequestBehavior.AllowGet);

        }

        private class BitBucketResponse
        {
            public int Count { get; set; }
            public string Filter { get; set; }
            public string Search { get; set; }
            public List<Issue> Issues { get; set; }
        }

        private class Issue
        {
            public string Status { get; set; }
            public string Priority { get; set; }
            public string Title { get; set; }
            public string Utc_last_updated { get; set; }
            public string Comment_count { get; set; }
            public Metadata Metadata { get; set; }
            public string resource_uri { get; set; }
            public string Content { get; set; }
        }

        private class Metadata
        {
            public string Kind { get; set; }
        }
    }
}
