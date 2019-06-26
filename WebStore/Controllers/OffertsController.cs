using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore.Models;
using WebStore.Routing;
using X.PagedList;
using X.PagedList.Mvc;

namespace WebStore.Controllers
{
    public class OffertsController : Controller
    {
        // GET: Offerts
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
            string translatedId = "";
            Translate dir = new Translate();
            webstoreEntities db = new webstoreEntities();
            ElectropEntities dbE = new ElectropEntities();

            var cate = db.tblCategories.Select(x => x).ToList();

            var s = (from a in dbE.iw_tprod
                     join b in dbE.iw_tgrupo
                     on a.CodGrupo equals b.CodGrupo
                     join e in dbE.iw_tsubgr
                     on a.CodSubGr equals e.CodSubGr
                     join c in dbE.iw_tlprprod
                     on a.CodProd equals c.CodProd
                     join d in dbE.iw_tlispre
                     on c.CodLista equals d.CodLista
                     where d.CodLista == "16"
                     select new
                     {
                         strCodigo = a.CodProd,
                         strCod = a.CodBarra,
                         strNombre = a.DesProd,
                         intPrecio = a.PrecioVta,
                         percent = c.ValorPct,
                         category = e.DesSubGr
                     })
                        .AsEnumerable()
                        .Select(x => new Products
                        {
                            strCodigo = x.strCod,
                            strNombre = Truncate(x.strNombre, 60).ToLower(),
                            intPrecio = FormatNumber((int)(x.intPrecio + (x.intPrecio * (30 / 100)))),
                            intPrecentOff = x.percent + "%",
                            intPrecioOff = FormatNumber((int)(x.intPrecio + (x.intPrecio * (30 / 100))) - (int)(((x.intPrecio + (x.intPrecio * (30 / 100))) * x.percent / 100))),
                            intPercent = "30%",
                            intPrecioNum = (int)(x.intPrecio + (x.intPrecio * (30 / 100))) - (int)(((x.intPrecio + (x.intPrecio * (x.percent / 100))) * x.percent / 100)),
                            categorySeo = cate.Any(c => c.strNombre == x.category) ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cate.Where(c => c.strNombre == x.category).Select(c => c.strSeo).FirstOrDefault()) : ""
                        })
                        .ToList();
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
                    var first = (from t in tList
                                 where t.strTime > DateTime.Now
                                 orderby t.strTime ascending
                                 select t.strTime).First();
                    TimeSpan timeDiff = first - DateTime.Now;
                    int percentOff = tList.Where(t => t.refCodProd == a.strCodigo && t.strTime == first).Select(t => (int)t.intPercentageTime).FirstOrDefault();
                    if (tList.Any(t => t.refCodProd == a.strCodigo && t.strTime == first))
                    {
                        a.TimeOffer = true;
                        a.intPrecentOff = percentOff + "%";
                        a.intPrecioOff = FormatNumber((int)Decimal.Parse(a.intPrecio) - (int)(Decimal.Parse(a.intPrecio) * percentOff / 100));
                        a.Time = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeDiff.Days, timeDiff.Hours, timeDiff.Minutes, timeDiff.Seconds);
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

        private static string Truncate(string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }

        private static string FormatNumber(int number)
        {
            return number.ToString("N0", CultureInfo.GetCultureInfo("es-CL"));
        }
    }
}