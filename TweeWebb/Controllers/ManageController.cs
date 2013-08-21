using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ImageResizer;
using MoreLinq;
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

        public ActionResult Stats()
        {
            var backups = RavenSession.Query<Backup>().ToList();

            IEnumerable<IGrouping<string, Show>> shows = backups.Where(x => x.Shows != null).SelectMany(x => x.Shows).GroupBy(x => x.Name);

            var statsViewModel = new StatsViewModel
                                     {
                                         Shows = shows.OrderByDescending(x=>x.Count()).ThenBy(x=>x.Key).Take(10),
                                         Users = backups.Where(x => x.Shows != null).OrderByDescending(x => x.Shows.Count)
                                     };

            return View(statsViewModel);
        }

        public JsonResult SaveImages()
        {
            var backup = RavenSession.Query<Backup>().ToList();
            var shows = backup.Where(x => x.Shows != null).SelectMany(x => x.Shows).DistinctBy(x => x.Name).OrderBy(x => x.Name).ToList();

            var missingImages = (from show in shows where !SaveImage(show.SeriesId) select show.SeriesId).ToList();

            //if (missingImages.Any())
            //{
            //    RavenSession.Store(missingImages);
            //    RavenSession.SaveChanges();
            //}

            return Json("Done", JsonRequestBehavior.AllowGet);

        }

        private bool SaveImage(string id)
        {
            try
            {
                string remoteImageUrl = string.Format(BannerUrl, id);
                string strRealname = Path.GetFileName(remoteImageUrl);
                string exts = Path.GetExtension(remoteImageUrl);
                string location = Server.MapPath("~/headers/");

                string filename = id + ".jpg";

                string saveurl = location + filename;

                if (System.IO.File.Exists(saveurl))
                    return true;

                WebClient webClient = new WebClient();
                webClient.DownloadFile(remoteImageUrl, saveurl);

                ResizeImage(saveurl, location, filename);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void ResizeImage(String image, string location, string fileName)
        {
            //The resizing settings can specify any of 30 commands.. See http://imageresizing.net for details.
            //Destination paths can have variables like <guid> and <ext>, or 
            //even a santizied version of the original filename, like <filename:A-Za-z0-9>
            //ImageResizer.ImageJob i = new ImageResizer.ImageJob(file, "~/uploads/<guid>.<ext>", new ImageResizer.ResizeSettings("width=2000;height=2000;format=jpg;mode=max"));
            //i.CreateParentDirectory = true; //Auto-create the uploads directory.
            //i.Build();

            var srcImage = Image.FromFile(image);
            var newWidth = srcImage.Width / 2;
            var newHeight = srcImage.Height / 2;

            var imageJob = new ImageJob(srcImage, string.Format("{0}r_{1}", location, fileName), new ImageResizer.ResizeSettings(newWidth, newHeight, FitMode.None, null));
            try
            {
                imageJob.Build();
                srcImage.Dispose();

                //We are don, now delete image!
                System.IO.File.Delete(image);
            }
            catch (Exception ex)
            {
                throw ex;
            }




            //using (var srcImage = Image.FromFile(image))
            //{
            //    var newWidth = srcImage.Width / 2;
            //    var newHeight = srcImage.Height / 2;

            //    using (var newImage = new Bitmap(newWidth, newHeight))
            //    {
            //        using (var graphics = Graphics.FromImage(newImage))
            //        {
            //            graphics.SmoothingMode = SmoothingMode.HighSpeed;
            //            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            //            graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            //            graphics.DrawImage(srcImage, new Rectangle(0, 0, newWidth, newHeight));

            //            try
            //            {
            //                srcImage.Dispose();


            //                // Get an ImageCodecInfo object that represents the JPEG codec.
            //                ImageCodecInfo imageCodecInfo = this.GetEncoderInfo(ImageFormat.Jpeg);

            //                // Create an Encoder object for the Quality parameter.
            //                Encoder encoder = Encoder.Quality;

            //                // Create an EncoderParameters object. 
            //                EncoderParameters encoderParameters = new EncoderParameters(1);

            //                // Save the image as a JPEG file with quality level.
            //                EncoderParameter encoderParameter = new EncoderParameter(encoder, 50);
            //                encoderParameters.Param[0] = encoderParameter;
            //                newImage.Save(image, imageCodecInfo, encoderParameters);
            //            }
            //            catch (Exception ex)
            //            {
            //                throw ex;
            //            }
            //        }

            //    }
            //}
        }


        private ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
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
                    if (backup.Shows == null)
                        backup.Shows = new List<Show>();
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

    public class StatsViewModel
    {
        public IEnumerable<IGrouping<string, Show>> Shows;
        public IEnumerable<Backup> Users;
    }
}
