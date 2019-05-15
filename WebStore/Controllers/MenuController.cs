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

        public ActionResult LeftMenu(string active)
        {
            var viewModel = new BindingCateogyFamilyChild();

            using (webstoreEntities db = new webstoreEntities())
            {
                viewModel.family = db.tblFamily.Select(x => x).OrderBy(y => y.intOrder).ToList();
                viewModel.category = db.tblCategories.Select(x => x).ToList();
                viewModel.categoryTotal = categoryTotal();
                viewModel.familyTotal = familyTotal();
            }

            ViewBag.active = active;
            return PartialView(viewModel);
        }

        private static List<TotalProductByCategory> categoryTotal()
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

        private static List<TotalProductByFamily> familyTotal()
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
    }
}