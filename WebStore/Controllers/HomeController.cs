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

                var s = (from p in db.tblProducts
                         join f in db.tblFamily
                         on p.refFamily equals f.idFamily
                         join c in db.tblCategories
                         on p.refCategory equals c.idCategoria
                         where p.strCode == first.refCodProd
                         select new
                         {
                             Codigo = p.strCode,
                             Name = p.strName,
                             Price = p.intPrice,
                             Category = c.strNombre,
                             Offert = p.refOffert,
                             OffertTime = p.refOfferTime,
                             CategoryName = c.strNombre
                         }).AsEnumerable()
                                .Select(x => new Products
                                {
                                    strCodigo = x.Codigo,
                                    strNombre = x.Name,
                                    intPrecio = Function.FormatNumber(x.Price),
                                    intPrecioNum = x.Price,
                                    categorySeo = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x.Category),
                                    categoryName = x.CategoryName,
                                    intPrecentOff = percentOff + "%",
                                    intPrecioOff = Function.FormatNumber(x.Price - (x.Price * percentOff / 100) ),
                                    TimeOffer = true,
                                    Time = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeDiff.Days, timeDiff.Hours, timeDiff.Minutes, timeDiff.Seconds)
                                }).ToList();
                
                ViewBag.TimeOffer = true;
                viewModel.ProductTime = s;
            }
            else
            {
                ViewBag.TimeOffer = false;
            }

            var pro = (from p in db.tblProducts
                       join f in db.tblFamily
                       on p.refFamily equals f.idFamily
                       join c in db.tblCategories
                       on p.refCategory equals c.idCategoria
                       select new
                       {
                           Codigo = p.strCode,
                           Name = p.strName,
                           Price = p.intPrice,
                           Category = c.strNombre,
                           Offert = p.refOffert,
                           OffertTime = p.refOfferTime
                       }).AsEnumerable()
                        .Select(x => new Products
                        {
                            strCodigo = x.Codigo,
                            strNombre = x.Name,
                            intPrecio = Function.FormatNumber(x.Price),
                            intPrecioNum = x.Price,
                            categorySeo = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x.Category),
                            Offert = x.Offert,
                            OffertTime = x.OffertTime
                        }).ToList();

            /*var brands = (from a in db.tblRelBrand
                          join b in db.tblBrand
                          on a.refBrand equals b.idBrand
                          select new
                          {
                              refProd = a.refProd,
                              brand = b.strName
                          }).ToList();

            foreach (var i in pro)
            {
                if (brands.Any(b => b.refProd == i.strCodigo))
                {
                    i.Brand = brands.Where(b => b.refProd == i.strCodigo).Select(l => l.brand).FirstOrDefault();
                }
            }*/

            pro = Function.GetOffertOrOffertTime(pro, db);

            viewModel.ProductsList = pro.Where(w => w.categorySeo == "Stadium").Take(12);
            viewModel.ProductsOffer = pro.Where(x => x.intPrecioOff != null);

            List<CategoryList> CategoryList = new List<CategoryList>();
            List<Products> FirstHistory = new List<Products>();
            List<Products> SecondHistory = new List<Products>();
            List<Products> ThirdHistory = new List<Products>();
            var tList = db.tblOffertTime.Where(t => t.strTime >= DateTime.Now).Select(j => j).ToList();
            if (Session["id"]!= null)
            {
                int UserId = int.Parse(Session["id"].ToString());
                tblHistory History = db.tblHistory.Where(h => h.refUser == UserId).FirstOrDefault();
                if(History != null)
                {
                    var items = db.tblHistoryDet.Where(x => x.refHistory == History.IdHistory).AsEnumerable();

                    foreach(var i in items.Reverse())
                    {
                        var product = (from a in db.tblProducts
                                       join f in db.tblFamily
                                       on a.refFamily equals f.idFamily
                                       join c in db.tblCategories
                                       on a.refCategory equals c.idCategoria
                                       where a.idProduct == i.refProduct
                                       select new CategoryList
                                       {
                                           CategoryCode = c.idCategoria
                                       })
                                       .FirstOrDefault();
                        if (!CategoryList.Any(a => a.CategoryCode == product.CategoryCode))
                            CategoryList.Add(product);
                    }
                    int counter = 0;
                    foreach (var w in CategoryList.Take(3))
                    {
                        var product = PopulateHistoryIndex(w.CategoryCode, db);

                        if (counter == 0)
                        {
                            FirstHistory = product;
                        }
                        else if (counter == 1)
                        {
                            SecondHistory = product;
                        }
                        else if (counter == 2)
                        {
                            ThirdHistory = product;
                        }
                        counter++;
                    }
                }
            }
            else
            {
                if (Request.Cookies["History"] != null)
                {
                    var History = Request.Cookies["History"];
                    var items = History.Values.AllKeys.SelectMany(History.Values.GetValues, (k, v) => new { key = k, value = v });
                    foreach (var w in items.Reverse())
                    {
                        var product = (from a in db.tblProducts
                                       join f in db.tblFamily
                                       on a.refFamily equals f.idFamily
                                       join c in db.tblCategories
                                       on a.refCategory equals c.idCategoria
                                       where a.strCode == w.value
                                       select new CategoryList
                                       {
                                           CategoryCode = c.idCategoria
                                       })
                                       .FirstOrDefault();
                        if(!CategoryList.Any(a => a.CategoryCode == product.CategoryCode))
                            CategoryList.Add(product);
                    }
                    int counter = 0;
                    foreach(var w in CategoryList.Take(3))
                    {
                        var product = PopulateHistoryIndex(w.CategoryCode, db);

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
                }
            }

            viewModel.FirstHistory = FirstHistory;
            viewModel.SecondHistory = SecondHistory;
            viewModel.ThirdHistory = ThirdHistory;
            ViewBag.Mobile = Request.Browser.IsMobileDevice;

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

        private List<Products> PopulateHistoryIndex(int Code, webstoreEntities db)
        {
            var product = (from p in db.tblProducts
                           join f in db.tblFamily
                           on p.refFamily equals f.idFamily
                           join c in db.tblCategories
                           on p.refCategory equals c.idCategoria
                           where c.idCategoria == Code
                           select new
                           {
                               Codigo = p.strCode,
                               Name = p.strName,
                               Price = p.intPrice,
                               Category = c.strNombre,
                               Offert = p.refOffert,
                               OffertTime = p.refOfferTime
                           }).AsEnumerable()
                                .Select(x => new Products
                                {
                                    strCodigo = x.Codigo,
                                    strNombre = x.Name,
                                    intPrecio = Function.FormatNumber(x.Price),
                                    intPrecioNum = x.Price,
                                    categorySeo = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x.Category),
                                    Offert = x.Offert,
                                    OffertTime = x.OffertTime
                                }).ToList();

            product = Function.GetOffertOrOffertTime(product, db);

            return product;
        }
    }
}