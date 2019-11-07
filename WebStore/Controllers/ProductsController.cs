using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore.Models;
using WebStore.Functions;
using WebStore.Routing;
using System.Globalization;
using System.IO;
using X.PagedList;
using X.PagedList.Mvc;

namespace WebStore.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
        public ActionResult Offerts(int? Page, int? PerPage, string SortedBy)
        {
            string url = Request.RawUrl;
            string query = Request.Url.Query;
            string isAllowed;
            if (query != "")
            {
                if (!String.IsNullOrEmpty(Request.QueryString["View"]) || !String.IsNullOrWhiteSpace(Request.QueryString["View"]))
                {
                    isAllowed = Request.QueryString["View"];
                }
                else
                {
                    isAllowed = "";
                }
            }
            else
            {
                isAllowed = "";
            }
            ViewBag.type = isAllowed;

            using (webstoreEntities db = new webstoreEntities())
            {
                var s = Function.GetProductsList(db).Where(x => x.Offert > 0 || x.Offert != null)
                       .Select(x => new Products
                       {
                           strCodigo = x.Codigo,
                           strNombre = x.Name,
                           intPrecio = Function.FormatNumber(x.Price),
                           intPrecioNum = x.Price,
                           categorySeo = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x.Category),
                           Offert = x.Offert,
                           OffertTime = x.OffertTime
                       }).ToList();

                if (s.Any())
                {
                    ViewBag.minPrice = s.Min(x => Math.Round(Decimal.Parse(x.intPrecio), 0));
                    ViewBag.maxPrice = s.Max(x => Math.Round(Decimal.Parse(x.intPrecio), 0));
                }
                else
                {
                    ViewBag.minPrice = 0;
                    ViewBag.maxPrice = 0;
                }

                s = Function.GetOffertOrOffertTime(s, db);

                var sorted = Function.SetOrder(SortedBy, s);

                ViewBag.SeoLocation = "offerts";
                ViewBag.Title = Resources.Menu.left;

                var rand = new Random();
                var files = Directory.GetFiles(HttpContext.Server.MapPath("~/Content/img/bannerCategories/"), "*.jpg");
                var fileName = Path.GetFileName(files[rand.Next(files.Length)]);
                ViewBag.banner = fileName;

                if (Request.Browser.IsMobileDevice)
                {
                    ViewBag.Mobile = true;
                }
                else
                {
                    ViewBag.Mobile = false;
                }

                return View(sorted.ToPagedList(Page ?? 1, PerPage ?? 15));
            }
        }
    }
}