using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore.Models;
using WebStore.Functions;
using WebStore.Routing;

namespace WebStore.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Category(string id, string idp, int? Page, int? PerPage, string SortedBy)
        {
            webstoreEntities db = new webstoreEntities();
            ElectropEntities dbE = new ElectropEntities();
            Warehouse stock = new Warehouse();
            Function function = new Function();
            var o = dbE.iw_tlprprod.Where(i => i.CodLista == "16").ToList();
            var x = (from a in dbE.iw_tprod
                     join b in dbE.iw_tlprprod
                     on a.CodProd equals b.CodProd
                     join c in dbE.iw_tlispre
                     on b.CodLista equals c.CodLista
                     where a.CodBarra == idp && c.CodLista == "15"
                     select new
                     {
                         strNombre = a.DesProd,
                         strCodigo = a.CodProd,
                         strCod = a.CodBarra,
                         intPrecio = a.PrecioVta,
                         intPercent = b.ValorPct
                     })
                     .AsEnumerable()
                     .Select(p => new ProductsSingle
                     {
                         Cod = p.strCodigo,
                         strCodigo = p.strCod,
                         strNombre = p.strNombre.ToLower(),
                         intPrecio = function.FormatNumber((int)(p.intPrecio + (p.intPrecio * (p.intPercent / 100)))),
                         intPrecentOff = o.Any(l => l.CodProd == p.strCodigo) ? o.Where(i => i.CodProd == p.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() + "%" : "0",
                         intPrecioOff = o.Any(l => l.CodProd == p.strCodigo) ? function.FormatNumber((int)(p.intPrecio + (p.intPrecio * (p.intPercent / 100))) - (int)(((p.intPrecio + (p.intPrecio * (p.intPercent / 100))) * o.Where(i => i.CodProd == p.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() / 100))) : "0",
                         intPercent = p.intPercent + "%",
                         categorySeo = id
                     })
                     .ToList();
            ViewBag.Category = id;

            var tList = db.tblOffertTime.Where(t => t.strTime >= DateTime.Now).Select(j => j).ToList();

            if (tList.Any())
            {
                foreach (var a in x)
                {
                    ViewBag.Title = Resources.Titles.Product + " " + a.strNombre;
                    ViewBag.Breadcrumbs = Resources.Titles.Product + " #" + a.strCodigo;

                    var first = (from t in tList
                                 where t.strTime > DateTime.Now
                                 orderby t.strTime ascending
                                 select t.strTime).First();
                    ViewBag.first = first;
                    TimeSpan timeDiff = first - DateTime.Now;
                    int percentOff = tList.Where(t => t.refCodProd == a.strCodigo && t.strTime == first).Select(t => (int)t.intPercentageTime).FirstOrDefault();
                    if (tList.Any(t => t.refCodProd == a.strCodigo && t.strTime == first))
                    {
                        a.TimeOffer = true;
                        a.intPrecentOff = percentOff + "%";
                        a.intPrecioOff = function.FormatNumber((int)Decimal.Parse(a.intPrecio) - (int)(Double.Parse(a.intPrecio) * percentOff / 100));
                        a.Time = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeDiff.Days, timeDiff.Hours, timeDiff.Minutes, timeDiff.Seconds);
                    }
                }
            }
            else
            {
                foreach (var a in x)
                {
                    ViewBag.Title = Resources.Titles.Product + " " + a.strNombre;
                    ViewBag.Breadcrumbs = Resources.Titles.Product + " #" + a.strCodigo;
                }
            }

            var Ficha = db.tblFicha.Select(f => f);

            foreach (var p in x)
            {
                p.Stock = stock.GetStock(p.Cod);

                if (Ficha.Any(f => f.refCodProd == p.strCodigo))
                {
                    p.Ficha = Ficha;
                }
                else
                {
                    p.Ficha = new List<tblFicha>()
                    {

                    };
                }
            }

            return View("Product", x);
        }
    }
}