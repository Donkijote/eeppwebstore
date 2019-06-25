using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore.Models;

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
            webstoreEntities db = new webstoreEntities();
            var viewModel = new BindSelect();

            viewModel.countries = db.tblCountry.Select(x => x).ToList();

            viewModel.regions = db.tblRegiones.Select(x => x).ToList();

            viewModel.comunes = db.tblComunas.Where(x => x.refProvincia == 1).ToList();

            return View(viewModel);
        }

        public ActionResult LogIn()
        {
            return View();
        }
    }
}