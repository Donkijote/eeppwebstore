using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore.Models;

namespace WebStore.Controllers
{
    public class apiController : Controller
    {
        // GET: api
        public JsonResult States()
        {
            using (webstoreEntities db = new webstoreEntities())
            {

                var states = db.tblRegiones.Select(x => new { id = x.idRegion, nombre = x.strNombre, number = x.intNumber }).ToList();

                return Json(states, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Provinces(int id)
        {

            using (webstoreEntities db = new webstoreEntities())
            {

                var provinces = db.tblProvincias.Where(x => x.refRegion == id).Select(x => new { id = x.idProvincia, nombre = x.strNombre }).ToList();

                return Json(provinces, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Communes(int id)
        {
            using (webstoreEntities db = new webstoreEntities())
            {

                var communes = db.tblComunas.Where(x => x.refProvincia == id).Select(x => new { id = x.idComuna, nombre = x.strNombre }).ToList();

                return Json(communes, JsonRequestBehavior.AllowGet);
            }
        }
    }
}