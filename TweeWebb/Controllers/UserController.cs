using System;
using System.Collections.Generic;
using System.Linq;
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

        public ActionResult Index(string user)
        {
            var backup = RavenSession.Query<Backup>().FirstOrDefault(x => x.Username == user);

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

    }
}
