using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace WebStore.Controllers
{
    public class DownloadsController : Controller
    {
        // GET: Downloads
        public ActionResult ProductTecnicalSheetPDF(string filename)
        {
            return File("~/Content/fichas/"+filename+".pdf", "application/pdf", Server.UrlEncode(filename+".pdf"));
        }

        public ActionResult ProductSecCertificationPDF(string filename)
        {
            return File("~/Content/img/certificates/Certification-Sec-" + filename + ".pdf", "application/pdf", Server.UrlEncode("Certification-Sec-"+filename + ".pdf"));
        }

        public ActionResult ProductDs43CertificationPDF(string filename)
        {
            return File("~/Content/img/certificates/ds43/Certification-Ds43-" + filename + ".pdf", "application/pdf", Server.UrlEncode("Certification-Ds43-" + filename + ".pdf"));
        }
    }
}