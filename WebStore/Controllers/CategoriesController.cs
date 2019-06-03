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

namespace WebStore.Controllers
{
    public class CategoriesController : Controller
    {
        // GET: Categories
        public ActionResult All()
        {
            var viewModel = new BindingCateogyFamilyChild();
            using(webstoreEntities db = new webstoreEntities())
            {
                viewModel.family = (from a in db.tblFamily
                                    join b in db.tblImg
                                    on a.refImg equals b.idImg
                                    select new Family {
                                        IdFamily = a.idFamily,
                                        StrName = a.strName,
                                        StrSeo = a.strSeo,
                                        StrImgOne = b.strImgOne,
                                        StrImgTwo = b.strImgTwo,
                                        StrImgThree = b.strImgThree
                                    }).ToList();
                viewModel.category = db.tblCategories.Select(x => x).ToList();
            }
            return View(viewModel);
        }

        public ActionResult s(string id, string idp, int? Page, int? PerPage, string SortedBy)
        {
            if (String.IsNullOrWhiteSpace(idp))
            {
                using (webstoreEntities db = new webstoreEntities())
                {
                    string culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                    string translatedId = "";
                    if( culture != "en")
                    {
                        Translate dir = new Translate();
                        translatedId = dir.Translation(id, "es", "en");
                    }

                    string v = db.tblCategories.Where(a => a.strSeo == (culture != "en" ? translatedId : id))
                            .Select(i => i.strNombre).FirstOrDefault();

                    /*string v = (from a in db.tblCategories
                             where a.strSeo == (culture != "en" ? translatedId : id)
                             select new { Nombre = a.strNombre})
                             .FirstOrDefault();*/

                    var s = FamilyOrCategory((culture != "en" ? translatedId : id), v);

                    string url = Request.RawUrl;
                    string query = Request.Url.Query;
                    string isAllowed;
                    if (query != "")
                    {
                        if(!String.IsNullOrEmpty(Request.QueryString["View"]) || !String.IsNullOrWhiteSpace(Request.QueryString["View"]))
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
                    ViewBag.active = getTitle((culture != "en" ? translatedId : id));
                    ViewBag.Title = Resources.Categories.ResourceManager.GetString(getTitle((culture != "en" ? translatedId : id)));
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
                    else if(SortedBy == "high")
                    {
                        sorted = s.OrderByDescending(x => x.intPrecioNum);
                    }
                    else if(SortedBy == "low")
                    {
                        sorted = s.OrderBy(x => x.intPrecioNum);
                    }
                    return View("s", sorted.ToPagedList(Page ?? 1, PerPage ?? 15));
                }
            }
            else
            {
                webstoreEntities db = new webstoreEntities();
                ElectropEntities dbE = new ElectropEntities();
                /*var x = (from a in db.tblProducts
                        where a.strCodigo == idp
                        select new { strNombre = a.strNombre, strCodigo = a.strCodigo, intPrecio = a.intPrecio})
                        .AsEnumerable()
                        .Select(p => new Products { strNombre = p.strNombre, strCodigo = p.strCodigo, intPrecio = FormatNumber(p.intPrecio)});*/
                var o = dbE.iw_tlprprod.Where(i => i.CodLista == "16").ToList();
                var x = (from a in dbE.iw_tprod
                         join b in dbE.iw_tlprprod
                         on a.CodProd equals b.CodProd
                         join c in dbE.iw_tlispre
                         on b.CodLista equals c.CodLista
                         where a.CodBarra == idp && c.CodLista == "15"
                         select new {
                             strNombre = a.DesProd,
                             strCodigo = a.CodProd,
                             strCod = a.CodBarra,
                             intPrecio = a.PrecioVta,
                             intPercent = b.ValorPct })
                         .AsEnumerable()
                         .Select(p => new Products
                         {
                             strCodigo = p.strCod,
                             strNombre = p.strNombre.ToLower(),
                             intPrecio = FormatNumber((int)(p.intPrecio + (p.intPrecio * (p.intPercent / 100)))),
                             intPrecentOff = o.Any(l => l.CodProd == p.strCodigo) ? o.Where(i => i.CodProd == p.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() + "%" : "0",
                             intPrecioOff = o.Any(l => l.CodProd == p.strCodigo) ? FormatNumber((int)(p.intPrecio + (p.intPrecio * (p.intPercent / 100))) - (int)(((p.intPrecio + (p.intPrecio * (p.intPercent / 100))) * o.Where(i => i.CodProd == p.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() / 100))) : "0",
                             intPercent = p.intPercent + "%",
                             categorySeo = id
                         })
                         .ToList();
                ViewBag.Category = id;

                var tList = db.tblOffertTime.Where(t => t.strTime >= DateTime.Today).Select(j => j).ToList();

                foreach (var a in x)
                {
                    ViewBag.Title = Resources.Titles.Product +" "+ a.strNombre;
                    ViewBag.Breadcrumbs = Resources.Titles.Product + " #" + a.strCodigo;

                    var first = (from t in tList
                                    where t.strTime > DateTime.Now
                                    orderby t.strTime ascending
                                    select t.strTime).First();
                    ViewBag.first = first;
                    TimeSpan timeDiff = first.Value - DateTime.Now;
                    int percentOff = tList.Where(t => t.refCodProd == a.strCodigo && t.strTime == first).Select(t => (int)t.intPercentageTime).FirstOrDefault();
                    if (tList.Any(t => t.refCodProd == a.strCodigo && t.strTime == first))
                    {
                        a.TimeOffer = true;
                        a.intPrecentOff = percentOff + "%";
                        a.intPrecioOff = FormatNumber((int)Decimal.Parse(a.intPrecio) - (int)(Double.Parse(a.intPrecio) * percentOff / 100));
                        a.Time = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeDiff.Days, timeDiff.Hours, timeDiff.Minutes, timeDiff.Seconds);
                    }
                }

                return View("Product", x);
            }
        }

        private List<Products> FamilyOrCategory(string id, string categoryName)
        {
            webstoreEntities db = new webstoreEntities();
            ElectropEntities dbE = new ElectropEntities();
            if (categoryName == null)
            {
                var family = db.tblFamily.Where(a => a.strSeo == id).FirstOrDefault();
                
                var o = dbE.iw_tlprprod.Where(x => x.CodLista == "16").ToList();
                var s = (from a in dbE.iw_tprod
                         join b in dbE.iw_tgrupo
                         on a.CodGrupo equals b.CodGrupo
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
                             percent = c.ValorPct
                         })
                        .AsEnumerable()
                        .Select(x => new Products
                        {
                            strCodigo = x.strCod,
                            strNombre = Truncate(x.strNombre, 60).ToLower(),
                            intPrecio = FormatNumber((int)(x.intPrecio + (x.intPrecio * ( x.percent / 100 ) ))),
                            intPrecentOff = o.Any(l => l.CodProd == x.strCodigo) ? o.Where(i => i.CodProd == x.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() + "%" : "0",
                            intPrecioOff = o.Any(l => l.CodProd == x.strCodigo) ? FormatNumber((int)(x.intPrecio + (x.intPrecio * (x.percent / 100))) - (int)(((x.intPrecio + (x.intPrecio * (x.percent / 100))) * o.Where(i => i.CodProd == x.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() / 100))) : "0",
                            intPercent = x.percent + "%",
                            categorySeo = id,
                            intPrecioNum = o.Any(l => l.CodProd == x.strCodigo) ? (int)(x.intPrecio + (x.intPrecio * (x.percent / 100))) - (int)(((x.intPrecio + (x.intPrecio * (x.percent / 100))) * o.Where(i => i.CodProd == x.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() / 100)) : (int)(x.intPrecio + (x.intPrecio * (x.percent / 100)))
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
                        TimeSpan timeDiff = first.Value - DateTime.Now;
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
                                strNombre = Truncate(x.strNombre, 60).ToLower(),
                                intPrecio = FormatNumber( (int)(x.intPrecio + (x.intPrecio * ( x.percent / 100 ) ) ) ),
                                intPrecentOff = o.Any(l => l.CodProd == x.strCodigo) ? o.Where(i => i.CodProd == x.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() + "%" : "0",
                                intPrecioOff = o.Any(l => l.CodProd == x.strCodigo) ? FormatNumber( (int)( x.intPrecio + (x.intPrecio * (x.percent / 100)) )  -  (int)( ( (x.intPrecio + (x.intPrecio * (x.percent / 100))) * o.Where(i => i.CodProd == x.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() / 100) ) ) : "0",
                                intPercent = x.percent + "%",
                                categorySeo = id,
                                intPrecioNum = o.Any(l => l.CodProd == x.strCodigo) ? (int)(x.intPrecio + (x.intPrecio * (x.percent / 100))) - (int)(((x.intPrecio + (x.intPrecio * (x.percent / 100))) * o.Where(i => i.CodProd == x.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() / 100)) : (int)(x.intPrecio + (x.intPrecio * (x.percent / 100)))
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
                
                if(tList.Any())
                {
                    foreach (var a in s)
                    {
                        var first = (from t in tList
                                     where t.strTime > DateTime.Now
                                     orderby t.strTime ascending
                                     select t.strTime).First();
                        TimeSpan timeDiff = first.Value - DateTime.Now;
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
                
                return s;
            }
        }

        private static string Truncate(string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }

        private static string FormatNumber(int number)
        {
            return number.ToString("N0", CultureInfo.GetCultureInfo("es-CL"));
        }

        private string getTitle(string id)
        {
            using(webstoreEntities db = new webstoreEntities())
            {
                var f = db.tblFamily.Where(a => a.strSeo == id)
                        .Select(x => x)
                        .FirstOrDefault();
                if(f == null)
                {
                    var c = db.tblCategories.Where(a => a.strSeo == id)
                            .Select(x => x)
                            .FirstOrDefault();
                    if(c == null)
                    {
                        return "Error";
                    }
                    else
                    {
                        return c.strNombre;
                    }
                }
                else
                {
                    return f.strName;
                }
            }
        }
    }
}