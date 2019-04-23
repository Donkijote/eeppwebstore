using System;
using System.Collections.Generic;
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
                using (WebStore.Models.webstoreEntities db = new WebStore.Models.webstoreEntities())
                {
                    var v = db.tblCategories.Where(a => a.strNombre == id).FirstOrDefault();
                    var s = from a in db.tblProducts
                            where a.refCategoria == 1
                            select new Products { strNombre = a.strNombre, strCodigo = a.strCodigo, intPrecio = a.intPrecio };

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
                    return View("s",s.ToList().ToPagedList(page ?? 1, 3));
                }
            }
            else
            {
                using (WebStore.Models.webstoreEntities db = new WebStore.Models.webstoreEntities())
                {
                    var x = from a in db.tblProducts
                            where a.strCodigo == idp
                            select new Products { strNombre = a.strNombre, strCodigo = a.strCodigo, intPrecio = a.intPrecio};
                    ViewBag.Category = id;
                    
                    foreach(var a in x)
                    {
                        ViewBag.Title = WebStore.Resources.Titles.Product +" #"+ a.strCodigo;
                    }
                    return View("Product", x.ToList());
                }
            }
        }
    }
}