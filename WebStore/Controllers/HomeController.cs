using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;
using WebStore.Models;

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
            DateTime currentDate = DateTime.Now;
            DateTime nextDate = new DateTime(2019, 5, 6, 15, 0, 0, 0);

            TimeSpan timeDiff = nextDate - currentDate;

            if (timeDiff.Hours < 0 || timeDiff.Minutes < 0 || timeDiff.Seconds < 0)
            {
                ViewBag.Datime = "00:00:00:00";
            }
            else
            {
                ViewBag.Datetime = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeDiff.Days, timeDiff.Hours, timeDiff.Minutes, timeDiff.Seconds);
            }
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