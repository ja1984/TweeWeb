using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TweeWebb.Models;

namespace TweeWebb.Controllers
{
    public class ManageController : RavenDbController
    {
        private static string BannerUrl = "http://www.thetvdb.com/banners/fanart/original/{0}-1.jpg";

        //
        // GET: /Manage/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SaveImages()
        {
            var backup = RavenSession.Query<Backup>().ToList();

            var shows = backup.SelectMany(x => x.Shows).Distinct().ToList();


            List<string> missingImages = (from show in shows where !SaveImage(show.SeriesId) select show.SeriesId).ToList();

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

        [HttpPost]
        public JsonResult Add(Backup backup)
        {

            var oldBackup = RavenSession.Query<Backup>().FirstOrDefault(x => x.Username == backup.Username);

            if (oldBackup != null && oldBackup.Password != backup.Password)
                return Json(new { Success = false, Message = "Wrong password" });

            try
            {

                if (oldBackup != null)
                {
                    oldBackup.Shows = backup.Shows;
                    RavenSession.Store(oldBackup);
                }
                else
                {
                    RavenSession.Store(backup);
                }


                RavenSession.SaveChanges();

                return Json(new { Success = true, Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }

        }
    }
}
