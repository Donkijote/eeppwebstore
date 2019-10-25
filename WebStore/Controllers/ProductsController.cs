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
            string culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            Translate dir = new Translate();
            webstoreEntities db = new webstoreEntities();

            var s = (from p in db.tblProducts
                     join f in db.tblFamily
                     on p.refFamily equals f.idFamily
                     join c in db.tblCategories
                     on p.refCategory equals c.idCategoria
                     join o in db.tblOffert
                     on p.refOffert equals o.idOffert
                     select new
                     {
                         Codigo = p.strCode,
                         CodigoS = p.strCodeS,
                         Name = p.strName,
                         Price = p.intPrice,
                         Category = c.strNombre,
                         Percent = o.intPercentage,
                         OffertTime = p.refOfferTime
                     }).AsEnumerable()
                        .Select(x => new Products
                        {
                            strCodigo = x.Codigo,
                            strNombre = x.Name,
                            intPrecio = Function.FormatNumber(x.Price),
                            intPrecioNum = x.Price,
                            categorySeo = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x.Category),
                            intPrecentOff = x.Percent+"%",
                            OffertTime = x.OffertTime,
                            intPrecioOff = Function.FormatNumber(x.Price - (x.Price * x.Percent / 100))
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

            var tList = db.tblOffertTime.Where(t => t.strTime >= DateTime.Now).Select(j => j).ToList();
            if (tList.Any())
            {
                foreach (var a in s)
                {
                    if (a.OffertTime != null && a.OffertTime > 0)
                    {
                        var first = (from t in tList
                                     where t.strTime > DateTime.Now
                                     orderby t.strTime ascending
                                     select t.strTime).First();
                        TimeSpan timeDiff = first - DateTime.Now;
                        int percentOff = tList.Where(t => t.idOffertTime == a.OffertTime && t.strTime == first).Select(t => (int)t.intPercentageTime).FirstOrDefault();
                        if (tList.Any(t => t.idOffertTime == a.OffertTime && t.strTime == first))
                        {
                            a.TimeOffer = true;
                            a.intPrecentOff = percentOff + "%";
                            a.intPrecioOff = Function.FormatNumber((int)Decimal.Parse(a.intPrecio) - (int)(Decimal.Parse(a.intPrecio) * percentOff / 100));
                            a.Time = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeDiff.Days, timeDiff.Hours, timeDiff.Minutes, timeDiff.Seconds);
                        }
                    }
                }
            }

            var sorted = s.OrderBy(o => o.strNombre);

            if (SortedBy == null || SortedBy == "" || SortedBy == "nameA")
            {
                sorted = s.OrderBy(x => x.strNombre);
            }
            else if (SortedBy == "nameZ")
            {
                sorted = s.OrderByDescending(x => x.strNombre);
            }
            else if (SortedBy == "high")
            {
                sorted = s.OrderByDescending(x => x.intPrecioNum);
            }
            else if (SortedBy == "low")
            {
                sorted = s.OrderBy(x => x.intPrecioNum);
            }

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