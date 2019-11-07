using System;
using System.Collections.Generic;
using System.IO;
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
            
            using (webstoreEntities db = new webstoreEntities())
            {
                var viewModel = new BindingCateogyFamilyChild
                {
                    family = db.tblFamily.Select(x => new Family { IdFamily = x.idFamily, StrName = x.strName, StrSeo = x.strSeo, IntOrder = x.intOrder }).OrderBy(y => y.IntOrder).ToList(),
                    category = db.tblCategories.Select(x => x).ToList()
                };
                if (Session["id"] != null)
                {
                    int UserId = Int32.Parse(Session["id"].ToString());
                    List<tblQuotingQueDet> quotingQueDet = (from a in db.tblQuotingQueDet
                                                            join b in db.tblQuotingQue
                                                            on a.refQuotingQue equals b.IdQuotingQue
                                                            where b.refUser == UserId
                                                            select a).ToList();

                    viewModel.Notification = new Notification { Quotings = quotingQueDet.Count(), Cart = 0 };
                }
                else
                {
                    List<QuotingsProductList> QuoteProductList = Session["QuotingList"] as List<QuotingsProductList>;
                    List<CartProductList> CartProductList = Session["CartList"] as List<CartProductList>;
                    viewModel.Notification = new Notification ();
                    if (QuoteProductList != null)
                    {
                        viewModel.Notification.Quotings = QuoteProductList.Count();
                    }
                    else
                    {
                        viewModel.Notification.Quotings = 0;
                    }
                    if (CartProductList != null)
                    {
                        viewModel.Notification.Cart = CartProductList.Count();
                    }
                    else
                    {
                        viewModel.Notification.Cart = 0;
                    }
                    viewModel.ProductList = CartProductList;
                }

                return PartialView(viewModel);
            }
        }

        public ActionResult MenuMobile()
        {
            var viewModel = new BindingCateogyFamilyChild();

            using (webstoreEntities db = new webstoreEntities())
            {
                viewModel.family = db.tblFamily.Select(x => new Family { IdFamily = x.idFamily, StrName = x.strName, StrSeo = x.strSeo, IntOrder = x.intOrder}).OrderBy(y => y.IntOrder).ToList();
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
                viewModel.BrandTotal = BrandTotal(SeoLocation);
                viewModel.BetweenPrices = BetweenPrices(SeoLocation);
            }

            ViewBag.active = active;
            ViewBag.minPrice = minPrice;
            ViewBag.maxPrice = maxPrice;
            return PartialView(viewModel);
        }

        public ActionResult ProfileMenu()
        {
            List<Avatars> Avatars = new List<Avatars>();

            foreach(string s in Directory.GetFiles(Server.MapPath("~/Content/img/avatars/icons/"), "*.svg").Select(Path.GetFileName))
            {
                Avatars.Add(new Avatars { Icon = s, Selected = false });
            }

            ViewBag.Avatars = Avatars;

            return PartialView();
        }

        private static List<TotalProductByCategory> CategoryTotal()
        {
            webstoreEntities db = new webstoreEntities();
            //ElectropEntities dbE = new ElectropEntities();
            var s = (from a in db.tblProducts
                        join b in db.tblCategories
                        on a.refCategory equals b.idCategoria
                        join c in db.tblFamily
                        on b.refFamily equals c.idFamily
                        select new { totalCategories = b.idCategoria, totalfamily = c.idFamily, products = a.idProduct })
                        .GroupBy(cate => cate.totalCategories)
                        .Select(x => new TotalProductByCategory { TotalProducts = x.Count(), CategoryId = x.Key })
                        .ToList();

            /*var s = (from a in dbE.iw_tprod
                        join b in dbE.iw_tsubgr
                        on a.CodSubGr equals b.CodSubGr
                        join c in dbE.iw_tgrupo
                        on a.CodGrupo equals c.CodGrupo
                        join d in dbE.iw_tlprprod
                        on a.CodProd equals d.CodProd
                        where d.CodLista == "15"
                        select new { Category = b.DesSubGr, Family = c.DesGrupo, Product = a.CodBarra })
                        .GroupBy(c => c.Category)
                        .Select(x => new TotalProductByCategory { TotalProducts = x.Count(), CategoryName = x.Key})
                        .ToList();*/
            return s;

        }

        private static List<TotalProductByFamily> FamilyTotal()
        {
            webstoreEntities db = new webstoreEntities();
            //ElectropEntities dbE = new ElectropEntities();
            var s = (from a in db.tblProducts
                        join b in db.tblCategories
                        on a.refCategory equals b.idCategoria
                        join c in db.tblFamily
                        on b.refFamily equals c.idFamily
                        select new { totalCategories = b.idCategoria, totalfamily = c.idFamily, products = a.idProduct })
                        .GroupBy(cate => cate.totalfamily)
                        .Select(x => new TotalProductByFamily { TotalProducts = x.Count(), FamilyId = x.Key })
                        .ToList();
            /*var s = (from a in dbE.iw_tprod
                     join b in dbE.iw_tsubgr
                     on a.CodSubGr equals b.CodSubGr
                     join c in dbE.iw_tgrupo
                     on a.CodGrupo equals c.CodGrupo
                     join d in dbE.iw_tlprprod
                     on a.CodProd equals d.CodProd
                     where d.CodLista == "15"
                     select new { Category = b.DesSubGr, Family = c.DesGrupo, Product = a.CodBarra })
            .GroupBy(c => c.Family)
            .Select(x => new TotalProductByFamily { TotalProducts = x.Count(), FamilyName = x.Key })
            .ToList();*/

            return s;
        }

        private static List<TotalProductByBrand> BrandTotal(string seo)
        {
            webstoreEntities db = new webstoreEntities();

            if(seo != "offerts")
            {
                var category = (from a in db.tblCategories
                                where a.strSeo == seo
                                select a)
                            .FirstOrDefault();
                if (category != null)
                {
                    var prod = (from p in db.tblProducts
                                join f in db.tblFamily
                                on p.refFamily equals f.idFamily
                                join c in db.tblCategories
                                on p.refCategory equals c.idCategoria
                                where c.strSeo == seo
                                select new TwoCode
                                {
                                    codProS = p.strCodeS,
                                    codProW = p.strCode
                                })
                                .ToList();
                    var group = GetTotalProductByBrandsReducer(prod, db);
                    return group;
                }
                else
                {
                    var family = (from a in db.tblFamily
                                  where a.strSeo == seo
                                  select a)
                                .FirstOrDefault();
                    var prod = (from p in db.tblProducts
                                join f in db.tblFamily
                                on p.refFamily equals f.idFamily
                                join c in db.tblCategories
                                on p.refCategory equals c.idCategoria
                                where f.strSeo == seo
                                select new TwoCode
                                {
                                    codProS = p.strCodeS,
                                    codProW = p.strCode
                                })
                                .ToList();
                    var group = GetTotalProductByBrandsReducer(prod, db);
                    return group;
                }
            }
            else
            {
                var prod = (from p in db.tblProducts
                            join c in db.tblCategories
                            on p.refCategory equals c.idCategoria
                            join o in db.tblOffert
                            on p.refOffert equals o.idOffert
                            select new TwoCode
                            {
                                codProS = p.strCodeS,
                                codProW = p.strCode
                            }).ToList();

                var group = GetTotalProductByBrandsReducer(prod, db);
                return group;
            }
        }

        private static List<TotalProductByBrand> GetTotalProductByBrandsReducer(List<TwoCode> prod, webstoreEntities db)
        {

            var brand = (from a in db.tblRelBrand
                         join b in db.tblBrand
                         on a.refBrand equals b.idBrand
                         select new { brandName = b.strName, brandId = b.idBrand, refProd = a.refProd })
                             .ToList();
            var group = (from p in prod
                         join b in brand
                         on p.codProW equals b.refProd
                         select new
                         {
                             products = p.codProW,
                             brand = b.brandName,
                             brandId = b.brandId
                         })
                         .GroupBy(g => g.brand)
                         .Select(x => new TotalProductByBrand
                         {
                             TotalProducts = x.Count(),
                             BrandName = x.Key
                         })
                         .ToList();
            return group;
        }

        private static List<PriceRange> BetweenPrices(string seo)
        {
            webstoreEntities db = new webstoreEntities();
            ElectropEntities dbE = new ElectropEntities();
            if(seo != "offerts")
            {
                var cat = (from a in db.tblCategories
                           where a.strSeo == seo
                           select new { CategoryName = a.strNombre })
                        .FirstOrDefault();

                if (cat != null)
                {
                    var ca = (from a in dbE.iw_tprod
                              join l in dbE.iw_tlprprod
                              on a.CodProd equals l.CodProd
                              join c in dbE.iw_tsubgr
                              on a.CodSubGr equals c.CodSubGr
                              where l.CodLista == "15" && c.DesSubGr == cat.CategoryName
                              select new { price = a.PrecioVta + (a.PrecioVta * l.ValorPct / 100) })
                          .ToList();

                    PriceRange prices = new PriceRange
                    {
                        lower = ca.Where(x => x.price > 0.00 && x.price < 10000.00).Count(),
                        low = ca.Where(x => x.price >= 10000.00 && x.price < 50000.00).Count(),
                        middle = ca.Where(x => x.price >= 50000.00 && x.price < 100000.00).Count(),
                        high = ca.Where(x => x.price >= 100000.00 && x.price < 200000.00).Count(),
                        higher = ca.Where(x => x.price >= 200000.00 && x.price < 300000.00).Count(),
                        highest = ca.Where(x => x.price >= 300000.00 && x.price < 500000.00).Count(),
                        top = ca.Where(x => x.price >= 500000.00).Count(),
                    };

                    List<PriceRange> listPrice = new List<PriceRange>();
                    listPrice.Add(prices);

                    return listPrice;
                }
                else
                {

                    var fam = (from c in db.tblFamily
                               where c.strSeo == seo
                               select new { FamilyName = c.strName })
                                .FirstOrDefault();

                    var f = (from a in dbE.iw_tprod
                             join l in dbE.iw_tlprprod
                             on a.CodProd equals l.CodProd
                             join c in dbE.iw_tgrupo
                             on a.CodGrupo equals c.CodGrupo
                             where l.CodLista == "15" && c.DesGrupo == fam.FamilyName
                             select new { price = a.PrecioVta + (a.PrecioVta * l.ValorPct / 100) })
                          .ToList();

                    PriceRange prices = new PriceRange
                    {
                        lower = f.Where(x => x.price > 0 && x.price < 10000).Count(),
                        low = f.Where(x => x.price >= 10000 && x.price < 50000).Count(),
                        middle = f.Where(x => x.price >= 50000 && x.price < 100000).Count(),
                        high = f.Where(x => x.price >= 100000 && x.price < 200000).Count(),
                        higher = f.Where(x => x.price >= 200000 && x.price < 300000).Count(),
                        highest = f.Where(x => x.price >= 300000.00 && x.price < 500000.00).Count(),
                        top = f.Where(x => x.price >= 500000.00).Count(),
                    };
                    List<PriceRange> listPrice = new List<PriceRange>();
                    listPrice.Add(prices);
                    return listPrice;
                }

            }
            else
            {
                var ca = (from a in dbE.iw_tprod
                          join l in dbE.iw_tlprprod
                          on a.CodProd equals l.CodProd
                          where l.CodLista == "16"
                          select new { price = a.PrecioVta + (a.PrecioVta * 30 / 100) })
                          .ToList();

                PriceRange prices = new PriceRange
                {
                    lower = ca.Where(x => x.price > 0.00 && x.price < 10000.00).Count(),
                    low = ca.Where(x => x.price >= 10000.00 && x.price < 50000.00).Count(),
                    middle = ca.Where(x => x.price >= 50000.00 && x.price < 100000.00).Count(),
                    high = ca.Where(x => x.price >= 100000.00 && x.price < 200000.00).Count(),
                    higher = ca.Where(x => x.price >= 200000.00 && x.price < 300000.00).Count(),
                    highest = ca.Where(x => x.price >= 300000.00 && x.price < 500000.00).Count(),
                    top = ca.Where(x => x.price >= 500000.00).Count(),
                };

                List<PriceRange> listPrice = new List<PriceRange>();
                listPrice.Add(prices);

                return listPrice;
            }
        }
    }

    public class TwoCode
    {
        public string codProS { get; set; }
        public string codProW { get; set; }
    }
}