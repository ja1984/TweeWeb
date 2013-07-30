using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TweeWebb.Models;

namespace TweeWebb.Controllers
{
    public class UserController : RavenDbController
    {
        //
        // GET: /User/
        private static string BannerUrl = "http://www.thetvdb.com/banners/fanart/original/{0}-1.jpg";

        public ActionResult Index(string user)
        {
            var backup = RavenSession.Query<Backup>().FirstOrDefault(x => x.Username == user);


            if (backup.Shows.Any(x => x.Seasons != null && x.Seasons.Any()))
                return View(backup);

            foreach (var show in backup.Shows)
            {
                var seasons = show.Episodes.GroupBy(x => x.Season);
                show.Seasons = new List<Season>();
                foreach (var season in seasons)
                {

                    show.Seasons.Add(new Season()
                        {
                            Number = int.Parse(season.Key),
                            Unwatched = season.Where(x => x.Watched == "0").Count(),
                            Watched = season.Where(x => x.Watched == "1").Count()
                        });
                }
            }

            RavenSession.Store(backup);
            RavenSession.SaveChanges();


            return View(backup);
        }

        [HttpPost]
        public JsonResult Add(Backup backup)
        {

            try
            {
                RavenSession.Store(backup);
                RavenSession.SaveChanges();

                return Json(new { Success = true, Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }

        }


        public ActionResult SaveImages()
        {

            List<string> missingImages = new List<string>();

            var backup = RavenSession.Query<Backup>().FirstOrDefault(x => x.Username == "Jonathan");
            foreach (var show in backup.Shows)
            {
                if (!SaveImage(show.SeriesId))
                {
                    missingImages.Add(show.SeriesId);
                }
            }

            if (missingImages.Any())
            {
                RavenSession.Store(missingImages);
                RavenSession.SaveChanges();
            }

            return Content("Done");

        }

        private bool SaveImage(string id)
        {
            try
            {
                string remoteImageUrl = string.Format(BannerUrl, id);
                string strRealname = Path.GetFileName(remoteImageUrl);
                string exts = Path.GetExtension(remoteImageUrl);
                string location = Server.MapPath("~/banners/");

                string filename = id + ".jpg";

                string saveurl = location + filename;

                if (System.IO.File.Exists(saveurl))
                    return true;

                WebClient webClient = new WebClient();
                webClient.DownloadFile(remoteImageUrl, saveurl);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }
}
