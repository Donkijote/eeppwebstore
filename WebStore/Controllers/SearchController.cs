using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore.Models;
using X.PagedList;
using X.PagedList.Mvc;
using WebStore.Routing;
using System.IO;
using WebStore.Functions;

namespace WebStore.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        public ActionResult Index(string id, int? Page, int? PerPage, string SortedBy)
        {
            using (webstoreEntities db = new webstoreEntities())
            {
                string culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                string translatedId = "";
                if (culture != "en")
                {
                    Translate dir = new Translate();
                    translatedId = dir.Translation(id, "es", "en");
                }

                string v = db.tblCategories.Where(a => a.strSeo == (culture != "en" ? translatedId : id))
                        .Select(i => i.strNombre).FirstOrDefault();

                var s = FamilyOrCategory((culture != "en" ? translatedId : id), v);

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
                ViewBag.active = Function.getTitle((culture != "en" ? translatedId : id));
                ViewBag.Title = Resources.Categories.ResourceManager.GetString(Function.getTitle((culture != "en" ? translatedId : id)));
                ViewBag.SeoLocation = culture != "en" ? translatedId : id;
                ViewBag.category = v;
                ViewBag.perPage = PerPage != null ? PerPage : 0;

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

        private List<Products> FamilyOrCategory(string id, string categoryName)
        {
            webstoreEntities db = new webstoreEntities();
            ElectropEntities dbE = new ElectropEntities();
            if (categoryName == null)
            {
                var family = db.tblFamily.Where(a => a.strSeo == id).FirstOrDefault();
                var categories = db.tblCategories.Where(a => a.refFamily == family.idFamily).ToList();
                var o = dbE.iw_tlprprod.Where(x => x.CodLista == "16").ToList();
                var s = (from a in dbE.iw_tprod
                         join b in dbE.iw_tgrupo
                         on a.CodGrupo equals b.CodGrupo
                         join ct in dbE.iw_tsubgr
                         on a.CodSubGr equals ct.CodSubGr
                         join c in dbE.iw_tlprprod
                         on a.CodProd equals c.CodProd
                         join d in dbE.iw_tlispre
                         on c.CodLista equals d.CodLista
                         where b.DesGrupo == family.strName && d.CodLista == "15"
                         select new
                         {
                             strCodigo = a.CodProd,
                             strCod = a.CodBarra,
                             strNombre = a.DesProd,
                             intPrecio = a.PrecioVta,
                             percent = c.ValorPct,
                             category = ct.DesSubGr
                         })
                        .AsEnumerable()
                        .Select(x => new Products
                        {
                            strCodigo = x.strCod,
                            strNombre = Function.Truncate(x.strNombre, 60).ToLower(),
                            intPrecio = Function.FormatNumber((int)(x.intPrecio + (x.intPrecio * (x.percent / 100)))),
                            intPrecentOff = o.Any(l => l.CodProd == x.strCodigo) ? o.Where(i => i.CodProd == x.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() + "%" : "0",
                            intPrecioOff = o.Any(l => l.CodProd == x.strCodigo) ? Function.FormatNumber((int)(x.intPrecio + (x.intPrecio * (x.percent / 100))) - (int)(((x.intPrecio + (x.intPrecio * (x.percent / 100))) * o.Where(i => i.CodProd == x.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() / 100))) : "0",
                            intPercent = x.percent + "%",
                            categorySeo = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(categories.Where(a => a.strNombre == x.category).Select(a => a.strSeo).FirstOrDefault()),
                            intPrecioNum = o.Any(l => l.CodProd == x.strCodigo) ? (int)(x.intPrecio + (x.intPrecio * (x.percent / 100))) - (int)(((x.intPrecio + (x.intPrecio * (x.percent / 100))) * o.Where(i => i.CodProd == x.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() / 100)) : (int)(x.intPrecio + (x.intPrecio * (x.percent / 100)))
                        })
                        .ToList();
                if (s.Any())
                {
                    ViewBag.minPrice = s.Min(x => x.intPrecioNum);
                    ViewBag.maxPrice = s.Max(x => x.intPrecioNum);
                }
                else
                {
                    ViewBag.minPrice = 0;
                    ViewBag.maxPrice = 0;
                }

                var brands = (from a in db.tblRelBrand
                              join b in db.tblBrand
                              on a.refBrand equals b.idBrand
                              select new
                              {
                                  refProd = a.refProd,
                                  brand = b.strName
                              }).ToList();

                foreach (var i in s)
                {
                    if (brands.Any(b => b.refProd == i.strCodigo))
                    {
                        i.Brand = brands.Where(b => b.refProd == i.strCodigo).Select(l => l.brand).FirstOrDefault();
                    }
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
                            a.intPrecioOff = Function.FormatNumber((int)Decimal.Parse(a.intPrecio) - (int)(Decimal.Parse(a.intPrecio) * percentOff / 100));
                            a.Time = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeDiff.Days, timeDiff.Hours, timeDiff.Minutes, timeDiff.Seconds);
                        }
                    }
                }

                return s;
            }
            else
            {
                var o = dbE.iw_tlprprod.Where(x => x.CodLista == "16").ToList();
                var s = (from a in dbE.iw_tprod
                         join b in dbE.iw_tsubgr
                         on a.CodSubGr equals b.CodSubGr
                         join c in dbE.iw_tlprprod
                         on a.CodProd equals c.CodProd
                         join d in dbE.iw_tlispre
                         on c.CodLista equals d.CodLista
                         where b.DesSubGr == categoryName && d.CodLista == "15"
                         select new
                         {
                             strCodigo = a.CodProd,
                             strCod = a.CodBarra,
                             strNombre = a.DesProd,
                             intPrecio = a.PrecioVta,
                             percent = c.ValorPct
                         })
                            .AsEnumerable()
                            .Select(x => new Products
                            {
                                strCodigo = x.strCod,
                                strNombre = Function.Truncate(x.strNombre, 60).ToLower(),
                                intPrecio = Function.FormatNumber((int)(x.intPrecio + (x.intPrecio * (x.percent / 100)))),
                                intPrecentOff = o.Any(l => l.CodProd == x.strCodigo) ? o.Where(i => i.CodProd == x.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() + "%" : "0",
                                intPrecioOff = o.Any(l => l.CodProd == x.strCodigo) ? Function.FormatNumber((int)(x.intPrecio + (x.intPrecio * (x.percent / 100))) - (int)(((x.intPrecio + (x.intPrecio * (x.percent / 100))) * o.Where(i => i.CodProd == x.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() / 100))) : "0",
                                intPercent = x.percent + "%",
                                categorySeo = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(id),
                                intPrecioNum = o.Any(l => l.CodProd == x.strCodigo) ? (int)(x.intPrecio + (x.intPrecio * (x.percent / 100))) - (int)(((x.intPrecio + (x.intPrecio * (x.percent / 100))) * o.Where(i => i.CodProd == x.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() / 100)) : (int)(x.intPrecio + (x.intPrecio * (x.percent / 100)))
                            })
                            .ToList();
                if (s.Any())
                {
                    ViewBag.minPrice = s.Min(x => x.intPrecioNum);
                    ViewBag.maxPrice = s.Max(x => x.intPrecioNum);
                }
                else
                {
                    ViewBag.minPrice = 0;
                    ViewBag.maxPrice = 0;
                }

                var brands = (from a in db.tblRelBrand
                              join b in db.tblBrand
                              on a.refBrand equals b.idBrand
                              select new
                              {
                                  refProd = a.refProd,
                                  brand = b.strName
                              }).ToList();

                foreach (var i in s)
                {
                    if (brands.Any(b => b.refProd == i.strCodigo))
                    {
                        i.Brand = brands.Where(b => b.refProd == i.strCodigo).Select(l => l.brand).FirstOrDefault();
                    }
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
                            a.intPrecioOff = Function.FormatNumber((int)Decimal.Parse(a.intPrecio) - (int)(Decimal.Parse(a.intPrecio) * percentOff / 100));
                            a.Time = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeDiff.Days, timeDiff.Hours, timeDiff.Minutes, timeDiff.Seconds);
                        }
                    }
                }

                return s;
            }
        }
    }
}