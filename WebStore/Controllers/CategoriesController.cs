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
                viewModel.family = db.tblFamily.Select(x => x).ToList();
                viewModel.category = db.tblCategories.Select(x => x).ToList();
            }
            return View(viewModel);
        }

        public ActionResult s(string id, string idp, int? page)
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

                    int v = db.tblCategories.Where(a => a.strSeo == (culture != "en" ? translatedId : id))
                            .Select(i => i.idCategoria).FirstOrDefault();

                    var s = FamilyOrCategory((culture != "en" ? translatedId : id), v);

                    string url = Request.RawUrl;
                    string query = Request.Url.Query;
                    string isAllowed;
                    if (query != "")
                    {
                        if(!String.IsNullOrEmpty(Request.QueryString["Utf8"]) || !String.IsNullOrWhiteSpace(Request.QueryString["Utf8"]))
                        {
                            isAllowed = Request.QueryString["Utf8"];
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
                    return View("s",s.ToPagedList(page ?? 1, 3));
                }
            }
            else
            {
                using (webstoreEntities db = new webstoreEntities())
                {
                    var x = (from a in db.tblProducts
                            where a.strCodigo == idp
                            select new { strNombre = a.strNombre, strCodigo = a.strCodigo, intPrecio = a.intPrecio})
                            .AsEnumerable()
                            .Select(p => new Products { strNombre = p.strNombre, strCodigo = p.strCodigo, intPrecio = FormatNumber(p.intPrecio)});
                    ViewBag.Category = id;

                    DateTime currentDate = DateTime.Now;
                    DateTime nextDate = new DateTime(2019, 5, 6, 15, 0, 0, 0);

                    TimeSpan timeDiff = nextDate - currentDate;

                    if(timeDiff.Hours < 0 || timeDiff.Minutes < 0 || timeDiff.Seconds < 0)
                    {
                        ViewBag.Datime = "00:00:00:00";
                    }
                    else
                    {
                        ViewBag.Datetime = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeDiff.Days, timeDiff.Hours, timeDiff.Minutes, timeDiff.Seconds);
                    }
                 
                    foreach(var a in x)
                    {
                        ViewBag.Title = Resources.Titles.Product +" #"+ a.strCodigo;
                    }
                    return View("Product", x.ToList());
                }
            }
        }

        private List<Products> FamilyOrCategory(string id, int v)
        {
            using(webstoreEntities db = new webstoreEntities())
            {
                if (v == 0)
                {
                    var family = db.tblFamily.Where(a => a.strSeo == id).FirstOrDefault();
                    var s = (from a in db.tblProducts
                             join b in db.tblCategories
                             on a.refCategoria equals b.idCategoria
                             join c in db.tblFamily
                             on b.refFamily equals c.idFamily
                             where b.refFamily == family.idFamily
                             select new { strNombre = a.strNombre, strCodigo = a.strCodigo, intPrecio = a.intPrecio, strSeo = b.strSeo })
                            .AsEnumerable()
                            .Select(x => new Products { strNombre = Truncate(x.strNombre, 60), strCodigo = x.strCodigo, intPrecio = FormatNumber(x.intPrecio), categorySeo = x.strSeo }).ToList();
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
                    
                    return s;
                }
                else
                {
                    var s = (from a in db.tblProducts
                             join b in db.tblCategories
                             on a.refCategoria equals b.idCategoria
                             where a.refCategoria == v
                             select new { strNombre = a.strNombre, strCodigo = a.strCodigo, intPrecio = a.intPrecio, strSeo = b.strSeo })
                            .AsEnumerable()
                            .Select(x => new Products { strNombre = Truncate(x.strNombre, 60), strCodigo = x.strCodigo, intPrecio = FormatNumber(x.intPrecio), categorySeo = x.strSeo }).ToList();
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
                    return s;
                }
            }
        }

        private static string Truncate(string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }

        private static string FormatNumber(decimal number)
        {
            return number.ToString("N", CultureInfo.GetCultureInfo("es-CL"));
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