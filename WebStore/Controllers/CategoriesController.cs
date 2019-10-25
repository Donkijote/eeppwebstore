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

        public ActionResult s(string id, int? Page, int? PerPage, string SortedBy)
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

                var s = Function.GetFamilyOrCategory((culture != "en" ? translatedId : id), v, db);

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

                ViewBag.banner = Function.GetRandomBanner();

                if (Request.Browser.IsMobileDevice)
                {
                    ViewBag.Mobile = true;
                }
                else
                {
                    ViewBag.Mobile = false;
                }
                return View("s", sorted.ToPagedList(Page ?? 1, PerPage ?? 15));
            }
        }

        [HttpPost]
        public ActionResult GetSearchData(string id)
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

                var s = Function.GetFamilyOrCategory((culture != "en" ? translatedId : id), v, db);


                return PartialView(s);
            }
        }

    }
}