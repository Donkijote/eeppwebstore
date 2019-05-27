using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore.Models;

namespace WebStore.Controllers
{
    public class FooterController : Controller
    {
        // GET: Footer
        public ActionResult Index()
        {
            var viewModel = new BindingCateogyFamilyChild();

            using (webstoreEntities db = new webstoreEntities())
            {
                viewModel.family = db.tblFamily.Select(x => new Family { StrName = x.strName, StrSeo = x.strSeo, IntOrder = x.intOrder }).OrderBy(y => y.IntOrder).ToList();
                viewModel.category = db.tblCategories.Select(x => x).ToList();
            }
            return PartialView(viewModel);
        }
    }
}