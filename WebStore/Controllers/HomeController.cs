using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;
using WebStore.Models;
using WebStore.Functions;
using WebStore.Routing;

namespace WebStore.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Lang()
        {
            string lang = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            if(lang == "es")
            {
                return RedirectToAction("Index", "Home", new { language = lang });
            }
            else
            {
                return RedirectToAction("Index", "Home", new { language = lang });
            }
        }

        public ActionResult Index()
        {
           
            var viewModel = new Index();

            webstoreEntities db = new webstoreEntities();
            ElectropEntities dbE = new ElectropEntities();

            viewModel.Binding = new BindingCateogyFamilyChild
                {
                    family = (from a in db.tblFamily
                              join b in db.tblImg
                              on a.refImg equals b.idImg
                              select new Family { IdFamily = a.idFamily, StrName = a.strName, StrSeo = a.strSeo, StrImgOne = b.strImgOne, StrImgTwo = b.strImgTwo, StrImgThree = b.strImgThree, IntOrder = a.intOrder })
                              .OrderBy(x => x.IntOrder)
                              .ToList(),
                    category = db.tblCategories.Select(x => x).ToList()
                };

            viewModel.Brands = (from a in db.tblBrand
                                join b in db.tblImg
                                on a.refImg equals b.idImg
                                select new Brands{ StrName = a.strName, StrImg = b.strImgOne})
                                .ToList();

            
            var first = (from t in db.tblOffertTime
                            where t.strTime > DateTime.Now
                            orderby t.strTime ascending
                            select t).FirstOrDefault();
            if(first != null)
            {
                TimeSpan timeDiff = first.strTime - DateTime.Now;
                int percentOff = first.intPercentageTime != null ? (int)first.intPercentageTime : 0;

                var s = (from a in dbE.iw_tprod
                         join b in dbE.iw_tgrupo
                         on a.CodGrupo equals b.CodGrupo
                         join e in dbE.iw_tsubgr
                         on a.CodSubGr equals e.CodSubGr
                         join c in dbE.iw_tlprprod
                         on a.CodProd equals c.CodProd
                         join d in dbE.iw_tlispre
                         on c.CodLista equals d.CodLista
                         where a.CodBarra == first.refCodProd && d.CodLista == "15"
                         select new
                         {
                             strCodigo = a.CodProd,
                             strCod = a.CodBarra,
                             strNombre = a.DesProd,
                             intPrecio = a.PrecioVta,
                             percent = c.ValorPct,
                             category = e.DesSubGr
                         })
                            .AsEnumerable()
                            .Select(x => new Products
                            {
                                strCodigo = x.strCod,
                                strNombre = Function.Truncate(first.strName, 60).ToLower(),
                                categoryName = x.category,
                                intPrecio = Function.FormatNumber((int)(x.intPrecio + (x.intPrecio * (x.percent / 100)))),
                                intPrecentOff = percentOff + "%",
                                intPrecioOff = Function.FormatNumber((int)(x.intPrecio + (x.intPrecio * (x.percent / 100))) - ((int)(x.intPrecio + (x.intPrecio * (x.percent / 100))) * percentOff / 100)),
                                intPercent = x.percent + "%",
                                TimeOffer = true,
                                Time = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeDiff.Days, timeDiff.Hours, timeDiff.Minutes, timeDiff.Seconds)
                            })
                            .ToList();
                foreach (var a in s)
                {
                    var seo = db.tblCategories.Where(x => x.strNombre == a.categoryName).Select(x => x.strSeo).FirstOrDefault();
                    a.categorySeo = seo;
                }
                ViewBag.TimeOffer = true;
                viewModel.ProductTime = s;
            }
            else
            {
                ViewBag.TimeOffer = false;
            }

            var o = dbE.iw_tlprprod.Where(x => x.CodLista == "16").ToList();
            var p = (from a in dbE.iw_tprod
                     join b in dbE.iw_tgrupo
                     on a.CodGrupo equals b.CodGrupo
                     join c in dbE.iw_tlprprod
                     on a.CodProd equals c.CodProd
                     join f in dbE.iw_tsubgr
                     on a.CodSubGr equals f.CodSubGr
                     join d in dbE.iw_tlispre
                     on c.CodLista equals d.CodLista
                     where d.CodLista == "15"
                     select new
                     {
                         strCodigo = a.CodProd,
                         strCod = a.CodBarra,
                         strNombre = a.DesProd,
                         intPrecio = a.PrecioVta,
                         percent = c.ValorPct,
                         category = f.DesSubGr
                     })
                    .AsEnumerable()
                    .Select(x => new Products
                    {
                        strCodigo = x.strCod,
                        strNombre = Function.Truncate(x.strNombre, 60).ToLower(),
                        intPrecio = Function.FormatNumber((int)(x.intPrecio + (x.intPrecio * (x.percent / 100)))),
                        intPrecentOff = o.Any(l => l.CodProd == x.strCodigo) ? o.Where(i => i.CodProd == x.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() + "%" : "0",
                        intPrecioOff = o.Any(l => l.CodProd == x.strCodigo) ? Function.FormatNumber((int)(x.intPrecio + (x.intPrecio * (x.percent / 100))) - (int)(((x.intPrecio + (x.intPrecio * (x.percent / 100))) * o.Where(i => i.CodProd == x.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() / 100))) : "0",
                        intPercent = x.percent + "%",
                        intPrecioNum = o.Any(l => l.CodProd == x.strCodigo) ? (int)(x.intPrecio + (x.intPrecio * (x.percent / 100))) - (int)(((x.intPrecio + (x.intPrecio * (x.percent / 100))) * o.Where(i => i.CodProd == x.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() / 100)) : (int)(x.intPrecio + (x.intPrecio * (x.percent / 100))),
                        categoryName = x.category
                    })
                    .ToList();
            var cate = db.tblCategories.Select(x => x).ToList();
            var brands = (from a in db.tblRelBrand
                          join b in db.tblBrand
                          on a.refBrand equals b.idBrand
                          select new
                          {
                              refProd = a.refProd,
                              brand = b.strName
                          }).ToList();

            foreach (var i in p)
            {
                if (brands.Any(b => b.refProd == i.strCodigo))
                {
                    i.Brand = brands.Where(b => b.refProd == i.strCodigo).Select(l => l.brand).FirstOrDefault();
                }
                i.categorySeo = cate.Where(x => x.strNombre == i.categoryName).Select(x => x.strSeo).FirstOrDefault();
            }
            viewModel.ProductsList = p.Take(12);
            viewModel.ProductsOffer = p.Where(x => x.intPrecioOff != "0");

            if (Session["id"]!= null)
            {

            }
            else
            {
                if (Request.Cookies["History"] != null)
                {
                    var History = Request.Cookies["History"];
                    var items = History.Values.AllKeys.SelectMany(History.Values.GetValues, (k, v) => new { key = k, value = v });

                    List<CategoryList> CategoryList = new List<CategoryList>();
                    List<Products> FirstHistory = new List<Products>();
                    List<Products> SecondHistory = new List<Products>();
                    List<Products> ThirdHistory = new List<Products>();
                    Translate dir = new Translate();
                    var tList = db.tblOffertTime.Where(t => t.strTime >= DateTime.Now).Select(j => j).ToList();
                    foreach (var w in items.Reverse())
                    {
                        var product = (from a in dbE.iw_tprod
                                       join b in dbE.iw_tlprprod
                                       on a.CodProd equals b.CodProd
                                       join c in dbE.iw_tlispre
                                       on b.CodLista equals c.CodLista
                                       join d in dbE.iw_tsubgr
                                       on a.CodSubGr equals d.CodSubGr
                                       where a.CodProd == w.value && c.CodLista == "15"
                                       select new CategoryList
                                       {
                                           CategoryCode = a.CodSubGr
                                       })
                                       .FirstOrDefault();
                        if(!CategoryList.Any(a => a.CategoryCode == product.CategoryCode))
                            CategoryList.Add(product);
                    }
                    int counter = 0;
                    foreach(var w in CategoryList.Take(3))
                    {
                        var product = (from a in dbE.iw_tprod
                                 join b in dbE.iw_tgrupo
                                 on a.CodGrupo equals b.CodGrupo
                                 join c in dbE.iw_tlprprod
                                 on a.CodProd equals c.CodProd
                                 join f in dbE.iw_tsubgr
                                 on a.CodSubGr equals f.CodSubGr
                                 join d in dbE.iw_tlispre
                                 on c.CodLista equals d.CodLista
                                 where d.CodLista == "15" && a.CodSubGr == w.CategoryCode
                                 select new
                                 {
                                     strCodigo = a.CodProd,
                                     strCod = a.CodBarra,
                                     strNombre = a.DesProd,
                                     intPrecio = a.PrecioVta,
                                     percent = c.ValorPct,
                                     category = f.DesSubGr
                                 })
                                .AsEnumerable()
                                .Select(pro => new Products
                                {
                                    strCodigo = pro.strCod,
                                    strNombre = Function.Truncate(pro.strNombre, 60).ToLower(),
                                    intPrecio = Function.FormatNumber((int)(pro.intPrecio + (pro.intPrecio * (pro.percent / 100)))),
                                    intPrecentOff = o.Any(l => l.CodProd == pro.strCodigo) ? o.Where(i => i.CodProd == pro.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() + "%" : "0",
                                    intPrecioOff = o.Any(l => l.CodProd == pro.strCodigo) ? Function.FormatNumber((int)(pro.intPrecio + (pro.intPrecio * (pro.percent / 100))) - (int)(((pro.intPrecio + (pro.intPrecio * (pro.percent / 100))) * o.Where(i => i.CodProd == pro.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() / 100))) : "0",
                                    intPercent = pro.percent + "%",
                                    intPrecioNum = o.Any(l => l.CodProd == pro.strCodigo) ? (int)(pro.intPrecio + (pro.intPrecio * (pro.percent / 100))) - (int)(((pro.intPrecio + (pro.intPrecio * (pro.percent / 100))) * o.Where(i => i.CodProd == pro.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() / 100)) : (int)(pro.intPrecio + (pro.intPrecio * (pro.percent / 100))),
                                    categorySeo = dir.Translation(pro.category.ToLower(), "en", "es"),
                                    categoryName = pro.category
                                })
                                .ToList();

                        if (tList.Any())
                        {
                            foreach (var a in product)
                            {
                                var z = (from t in tList
                                         where t.strTime > DateTime.Now
                                         orderby t.strTime ascending
                                         select t.strTime).First();
                                ViewBag.first = z;
                                TimeSpan timeDiff = z - DateTime.Now;
                                int percentOff = tList.Where(t => t.refCodProd == a.strCodigo && t.strTime == z).Select(t => (int)t.intPercentageTime).FirstOrDefault();
                                if (tList.Any(t => t.refCodProd == a.strCodigo && t.strTime == z))
                                {
                                    a.TimeOffer = true;
                                    a.intPrecentOff = percentOff + "%";
                                    a.intPrecioOff = Function.FormatNumber((int)Decimal.Parse(a.intPrecio) - (int)(Double.Parse(a.intPrecio) * percentOff / 100));
                                    a.Time = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeDiff.Days, timeDiff.Hours, timeDiff.Minutes, timeDiff.Seconds);
                                }
                            }
                        }

                        if (counter == 0)
                        {
                            FirstHistory = product;
                        }else if(counter == 1)
                        {
                            SecondHistory = product;
                        }
                        else if(counter == 2)
                        {
                            ThirdHistory = product;
                        }
                        counter++;
                    }

                    viewModel.FirstHistory = FirstHistory;
                    viewModel.SecondHistory = SecondHistory;
                    viewModel.ThirdHistory = ThirdHistory;
                }
            }

            return View(viewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Knowus()
        {
            return View("Us");
        }

        public ActionResult Projects()
        {
            using(webstoreEntities db = new webstoreEntities())
            {
                List<Projects> projects = (from a in db.tblProjects
                                     join b in db.tblImg
                                     on a.refImg equals b.idImg
                                     select new Projects
                                     {
                                         Title = a.strTitle,
                                         Description = a.strDescription,
                                         Date = a.strDate,
                                         Location = a.strLocation,
                                         Category = a.strCategory,
                                         ImgOne = b.strImgOne,
                                         ImgTwo = b.strImgTwo,
                                         ImgThree = b.strImgThree,
                                         ImgFour = b.strImgFour,
                                         ImgFive = b.strImgFive,
                                         ImgSix = b.strImgSix
                                     }).ToList();
                return View(projects);
            }
        }

        public ActionResult FAQ()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}