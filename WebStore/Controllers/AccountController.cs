using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore.Models;
using System.Threading;
using System.Globalization;
using System.Web.Routing;

namespace WebStore.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Home()
        {

            string url = Request.RawUrl;
            string[] location = url.Split('/');

            ViewBag.url = location[2];

            return View();
        }

        public ActionResult Orders(string id = null)
        {
            string url = Request.RawUrl;
            string[] location = url.Split('/');

            ViewBag.url = location[2];

            if (String.IsNullOrWhiteSpace(id))
            {
                ViewBag.orderType = "Order";
                return View();
            }
            else if(id == "op")
            {
                ViewBag.orderType = "OpenOrders";
                return View();
            }
            else

            {
                ViewBag.orderType = "NullOrders";
                return View();
            }      
        }

        public ActionResult Traking()
        {
            string url = Request.RawUrl;
            string[] location = url.Split('/');

            ViewBag.url = location[2];

            return View();
        }

        public ActionResult Details(string x)
        {
            return View();
        }

        public ActionResult Address(string id = null)
        {
            string url = Request.RawUrl;
            string[] location = url.Split('/');

            ViewBag.url = location[2];
            if (String.IsNullOrWhiteSpace(id))
            {
                return View();
            }
            else
            {
                if(id == "Add")
                {
                    using(WebStore.Models.webstoreEntities db = new WebStore.Models.webstoreEntities())
                    {
                        var viewModel = new BindSelect();

                        viewModel.countries = db.tblCountry.Select(x=> x).ToList();

                        viewModel.regions = db.tblRegiones.Select(x=>x).ToList();

                        viewModel.comunes = db.tblComunas.Where(x => x.refProvincia == 1).ToList();

                        return View("addAddress", viewModel);
                    }
                }
                else if (id == "Edit")
                {
                    return View("editAddress");
                }
                else
                {
                    return View();
                }
            }
        }

        public JsonResult Provinces(int id)
        {

            using (WebStore.Models.webstoreEntities db = new WebStore.Models.webstoreEntities())
            { 

                var provinces = db.tblProvincias.Where(x => x.refRegion == id).Select(x=> new { id = x.idProvincia, nombre = x.strNombre}).ToList();

                return Json(provinces, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Communes (int id)
        {
            using (WebStore.Models.webstoreEntities db = new WebStore.Models.webstoreEntities())
            {

                var communes = db.tblComunas.Where(x => x.refProvincia == id).Select(x => new { id = x.idComuna, nombre = x.strNombre }).ToList();

                return Json(communes, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Settings()
        {
            string url = Request.RawUrl;
            string[] location = url.Split('/');

            ViewBag.url = location[2];

            return View();
        }

        public ActionResult WishList()
        {
            string url = Request.RawUrl;
            string[] location = url.Split('/');

            ViewBag.url = location[2];

            return View();
        }

        public ActionResult Tickets(int id = 0 )
        {
            string url = Request.RawUrl;
            string[] location = url.Split('/');

            ViewBag.url = location[2];

            if (id == 0)
            {
                return View();
            }
            else
            {
                ViewBag.Title = "Ticket ID:" + id;
                return View("detailsTickets");
            }
        }
    }
}