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

        public ActionResult LeftMenu()
        {
            var viewModel = new BindingCateogyFamilyChild();

            using (webstoreEntities db = new webstoreEntities())
            {
                viewModel.family = db.tblFamily.Select(x => x).OrderBy(y => y.intOrder).ToList();
                viewModel.category = db.tblCategories.Select(x => x).ToList();
            }
            return PartialView(viewModel);
        }
    }
}