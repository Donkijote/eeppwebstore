using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore.Models;

namespace WebStore.Controllers
{
    public class MenuController : Controller
    {
        // GET: Menu
        public ActionResult MenuBig()
        {
            var viewModel = new BindingCateogyFamilyChild();

            using (webstoreEntities db = new webstoreEntities())
            {
                viewModel.family = db.tblFamily.Select(x => x).OrderBy(y => y.intOrder).ToList();
                viewModel.category = db.tblCategories.Select(x => x).ToList();
            }
            return PartialView(viewModel);
        }

        public ActionResult MenuMobile()
        {
            var viewModel = new BindingCateogyFamilyChild();

            using (webstoreEntities db = new webstoreEntities())
            {
                viewModel.family = db.tblFamily.Select(x => x).OrderBy(y => y.intOrder).ToList();
                viewModel.category = db.tblCategories.Select(x => x).ToList();
            }
            return PartialView(viewModel);
        }

        public ActionResult LeftMenu(string active, string SeoLocation, string minPrice, string maxPrice)
        {
            var viewModel = new LeftMenu();

            using (webstoreEntities db = new webstoreEntities())
            {
                viewModel.Family = db.tblFamily.Select(x => x).OrderBy(y => y.intOrder).ToList();
                viewModel.Category = db.tblCategories.Select(x => x).ToList();
                viewModel.CategoryTotal = CategoryTotal();
                viewModel.FamilyTotal = FamilyTotal();

                if(SeoLocation != null || SeoLocation != "")
                {
                    viewModel.BrandTotal = BrandTotal(SeoLocation);
                }
            }

            ViewBag.active = active;
            ViewBag.minPrice = minPrice;
            ViewBag.maxPrice = maxPrice;
            return PartialView(viewModel);
        }

        private static List<TotalProductByCategory> CategoryTotal()
        {
            using(webstoreEntities db = new webstoreEntities())
            {
                var s = (from a in db.tblProducts
                         join b in db.tblCategories
                         on a.refCategoria equals b.idCategoria
                         join c in db.tblFamily
                         on b.refFamily equals c.idFamily
                         select new { totalCategories = b.idCategoria, totalfamily = c.idFamily, products = a.idProdcuto })
                         .GroupBy(cate => cate.totalCategories)
                         .Select(x => new TotalProductByCategory { TotalProducts = x.Count(), CategoryId = x.Key })
                         .ToList();
                return s;
            }
        }

        private static List<TotalProductByFamily> FamilyTotal()
        {
            using (webstoreEntities db = new webstoreEntities())
            {
                var s = (from a in db.tblProducts
                         join b in db.tblCategories
                         on a.refCategoria equals b.idCategoria
                         join c in db.tblFamily
                         on b.refFamily equals c.idFamily
                         select new { totalCategories = b.idCategoria, totalfamily = c.idFamily, products = a.idProdcuto })
                         .GroupBy(cate => cate.totalfamily)
                         .Select(x => new TotalProductByFamily { TotalProducts = x.Count(), FamilyId = x.Key })
                         .ToList();
                return s;
            }
        }

        private static List<TotalProductByBrand> BrandTotal(string seo)
        {
            using (webstoreEntities db = new webstoreEntities())
            {
                var ca = (from a in db.tblProducts
                         join b in db.tblCategories
                         on a.refCategoria equals b.idCategoria
                         join d in db.tblBrand
                         on a.refBrand equals d.idBrand
                         where b.strSeo == seo
                         select new { products = a.idProdcuto, category = b.idCategoria, brand = d.strName })
                         .GroupBy(q => q.brand)
                         .Select(x => new TotalProductByBrand { TotalProducts = x.Count(), BrandName = x.Key })
                         .ToList();
                if(ca.Any())
                {
                    return ca;
                }
                else
                {
                    
                    var f = (from a in db.tblProducts
                             join b in db.tblCategories
                             on a.refCategoria equals b.idCategoria
                             join c in db.tblFamily
                             on b.refFamily equals c.idFamily
                             join d in db.tblBrand
                             on a.refBrand equals d.idBrand
                             where c.strSeo == seo
                             select new { products = a.idProdcuto, category = b.idCategoria, brand = d.strName })
                             .GroupBy(q => q.brand)
                             .Select(x => new TotalProductByBrand { TotalProducts = x.Count(), BrandName = x.Key })
                             .ToList();
                    return f;
                }
            }
        }
    }
}