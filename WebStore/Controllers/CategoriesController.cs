using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore.Models;
using X.PagedList;
using X.PagedList.Mvc;

namespace WebStore.Controllers
{
    public class CategoriesController : Controller
    {
        // GET: Categories
        [HttpGet]
        public ActionResult All()
        {
            return View();
        }

        [HttpGet]
        public ActionResult s(string id, string idp, int? page)
        {
            if (String.IsNullOrWhiteSpace(idp))
            {
                using (webstoreEntities db = new webstoreEntities())
                {
                    var v = db.tblCategories.Where(a => a.strNombre == id).FirstOrDefault();
                    var s = (from a in db.tblProducts
                            where a.refCategoria == 1
                            select new { strNombre = a.strNombre, strCodigo = a.strCodigo, intPrecio = a.intPrecio })
                            .AsEnumerable()
                            .Select( x => new Products { strNombre = Truncate(x.strNombre, 60), strCodigo = x.strCodigo, intPrecio = FormatNumber(x.intPrecio) }).ToList();

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
                    ViewBag.p = id;
                    ViewBag.Title = id;
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
                    
                    foreach(var a in x)
                    {
                        ViewBag.Title = Resources.Titles.Product +" #"+ a.strCodigo;
                    }
                    return View("Product", x.ToList());
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
    }
}