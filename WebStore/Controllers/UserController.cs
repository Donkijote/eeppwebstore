using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebStore.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Cart()
        {
            return View();
        }

        public ActionResult CheckOut()
        {
            return View();
        }

        public ActionResult LogIn()
        {
            return View();
        }
    }
}