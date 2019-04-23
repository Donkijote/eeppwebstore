using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;

namespace WebStore.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Lang()
        {
            string lang = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            if(lang == "es")
            {
                return RedirectToAction("Index", "Home", new { language = lang });
            }
            else
            {
                return RedirectToAction("Index", "Home", new { language = lang });
            }
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}