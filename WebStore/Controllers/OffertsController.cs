using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebStore.Controllers
{
    public class OffertsController : Controller
    {
        // GET: Offerts
        public ActionResult Offerts(string SortedBy)
        {
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
            return View();
        }
    }
}