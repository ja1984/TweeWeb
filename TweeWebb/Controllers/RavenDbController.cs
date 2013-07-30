using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Raven.Client;
using Raven.Client.Document;

namespace TweeWebb.Controllers
{
    public abstract class RavenDbController : Controller
    {

        public static IDocumentStore DocumentStore
        {
            get
            {
                return new DocumentStore
                {
                    Url = "https://lark.ravenhq.com/",
                    DefaultDatabase = "twee-twee",
                    ApiKey = "4b193c17-a19d-4e90-b312-8ed1af1136fb"
                }.Initialize();
            }
        }

        public IDocumentSession RavenSession { get; protected set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            RavenSession = DocumentStore.OpenSession();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;

            using (RavenSession)
            {
                if (filterContext.Exception != null)
                    return;

                if (RavenSession != null)
                    RavenSession.SaveChanges();
            }
        }

        protected HttpStatusCodeResult HttpNotModified()
        {
            return new HttpStatusCodeResult(304);
        }


        protected new JsonResult Json(object data)
        {
            return base.Json(data, JsonRequestBehavior.AllowGet);
        }


    }
}
