using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebStore.Controllers
{
    public class PromotionsController : Controller
    {
        // GET: Promotions
        public ActionResult All()
        {
            return View();
        }
    }
}