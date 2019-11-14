using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using WebStore.Models;
using System.Web.Security;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;

namespace WebStore.Functions
{
    public class Function
    {
        public static string Truncate(string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }

        public static string FormatNumber(int number)
        {
            return number.ToString("N0", CultureInfo.GetCultureInfo("es-CL"));
        }

        public static string HighlightKeywords(string input, string keywords)
        {
            if (input == string.Empty || keywords == string.Empty)
            {
                return input;
            }

            string[] sKeywords = keywords.Split(' ');
            foreach (string sKeyword in sKeywords)
            {
                try
                {
                    input = Regex.Replace(input, sKeyword, string.Format("<strong>{0}</strong>", "$0"), RegexOptions.IgnoreCase);
                }
                catch(Exception ex)
                {
                    input = ex.ToString();
                }
            }
            return input;
        }

        public static IOrderedEnumerable<Products> SetOrder(string SortedBy, List<Products> products)
        {
            var sorted = products.OrderBy(o => o.strNombre);

            if (SortedBy == null || SortedBy == "" || SortedBy == "nameA")
            {
                sorted = products.OrderBy(x => x.strNombre);
            }
            else if (SortedBy == "nameZ")
            {
                sorted = products.OrderByDescending(x => x.strNombre);
            }
            else if (SortedBy == "high")
            {
                sorted = products.OrderByDescending(x => x.intPrecioNum);
            }
            else if (SortedBy == "low")
            {
                sorted = products.OrderBy(x => x.intPrecioNum);
            }

            return sorted;
        }

        public static string getTitle(string id)
        {
            using (webstoreEntities db = new webstoreEntities())
            {
                var f = db.tblFamily.Where(a => a.strSeo == id)
                        .Select(x => x)
                        .FirstOrDefault();
                if (f == null)
                {
                    var c = db.tblCategories.Where(a => a.strSeo == id)
                            .Select(x => x)
                            .FirstOrDefault();
                    if (c == null)
                    {
                        return "Error";
                    }
                    else
                    {
                        return c.strNombre;
                    }
                }
                else
                {
                    return f.strName;
                }
            }
        }

        public static MailMessage GenerateEmail(MailAddress toEmail, string subject, string body, string width = "500")
        {
            var message = new MailMessage(Configuration.GetEmail(), toEmail)
            {
                Subject = subject,
                Body = PopulateEmailBody(body, width),
                IsBodyHtml = true
            };
            return message;
        }

        public static string PopulateEmailBody(string bodyContent, string width = "500")
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/Shared/_EmailTemplate.html")))
            {
                body = reader.ReadToEnd();
            }
            //body = body.Replace("{Url}", url);
            body = body.Replace("{BodyContent}", bodyContent);
            body = body.Replace("{unSubcribeUrl}", "#");
            body = body.Replace("{Width}", width);
            return body;
        }

        public static string GetLocalIp()
        {
            string strHostName = Dns.GetHostName();
            string ipEntry = Dns.GetHostEntry(strHostName).AddressList[1].ToString();
            return ipEntry;
        }

        public static string GetUserIp()
        {
            return HttpContext.Current.Request.UserHostAddress;
        }

        public static string GetRandomBanner()
        {
            var rand = new Random();
            var files = Directory.GetFiles(HttpContext.Current.Server.MapPath("~/Content/img/bannerCategories/"), "*.jpg");
            return Path.GetFileName(files[rand.Next(files.Length)]);
        }

        public static Products GetHistoryMainItems(string Code, webstoreEntities db)
        {
            return (from p in db.tblProducts
                    join f in db.tblFamily
                    on p.refFamily equals f.idFamily
                    join c in db.tblCategories
                    on p.refCategory equals c.idCategoria
                    where p.strCode == Code
                    select new
                    {
                        Codigo = p.strCode,
                        Name = p.strName,
                        Price = p.intPrice,
                        Category = c.strSeo,
                        Offert = p.refOffert,
                        OffertTime = p.refOfferTime
                    }).AsEnumerable()
                    .Select(s => new Products
                    {
                        strCodigo = s.Codigo,
                        strNombre = s.Name,
                        intPrecio = Function.FormatNumber(s.Price),
                        intPrecioNum = s.Price,
                        categorySeo = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.Category),
                        Offert = s.Offert,
                        OffertTime = s.OffertTime
                    }).FirstOrDefault();
        }

        public static string SetQuotingQueDet(webstoreEntities db, tblQuotingQue x, QuotingsProductList y)
        {
            return AddQuotingQueDet(db, x, y);
        }
        public static string SetCartQueDet(webstoreEntities db, tblCartQue x, CartProductList y)
        {
            return AddCartQueDet(db, x, y);
        }

        public static List<CartProductList> GetCartProducts(List<tblCartQueDet> x, webstoreEntities y)
        {
            return CartProducts(x, y);
        }

        public static List<Products> GetOffertOrOffertTime(List<Products> pro, webstoreEntities db)
        {
            return CheckOffertOrOffertTime(pro, db);
        }
        private static List<Products> CheckOffertOrOffertTime(List<Products> pro, webstoreEntities db)
        {
            var oList = db.tblOffert.Select(x => x).ToList();
            var tList = db.tblOffertTime.Where(t => t.strTime >= DateTime.Now).Select(j => j).ToList();
            if (oList.Any())
            {
                foreach (var i in pro)
                {
                    if (i.Offert != null && i.Offert > 0)
                    {
                        int percet = (int)oList.Where(o => o.idOffert == i.Offert).Select(x => x.intPercentage).FirstOrDefault();
                        i.intPrecentOff = percet + "%";
                        i.intPrecioOff = Function.FormatNumber(i.intPrecioNum - (i.intPrecioNum * percet / 100));
                        i.intPrecioOffNum = i.intPrecioNum - (i.intPrecioNum * percet / 100);
                    }
                }
            }

            if (tList.Any())
            {
                foreach (var a in pro)
                {
                    if (a.OffertTime != null && a.OffertTime > 0)
                    {
                        var z = (from t in tList
                                 where t.strTime > DateTime.Now
                                 orderby t.strTime ascending
                                 select t.strTime).First();
                        TimeSpan timeDiff = z - DateTime.Now;
                        int percentOff = tList.Where(t => t.refCodProd == a.strCodigo && t.strTime == z).Select(t => (int)t.intPercentageTime).FirstOrDefault();
                        if (tList.Any(t => t.refCodProd == a.strCodigo && t.strTime == z))
                        {
                            a.TimeOffer = true;
                            a.intPrecentOff = percentOff + "%";
                            a.intPrecioOff = Function.FormatNumber((int)Decimal.Parse(a.intPrecio) - (int)(Double.Parse(a.intPrecio) * percentOff / 100));
                            a.intPrecioOffNum = (int)Decimal.Parse(a.intPrecio) - (int)(Double.Parse(a.intPrecio) * percentOff / 100);
                            a.Time = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeDiff.Days, timeDiff.Hours, timeDiff.Minutes, timeDiff.Seconds);
                        }
                    }
                }
            }

            return pro;
        }

        public static List<GetProducts> GetProductsList(webstoreEntities db)
        {
            return GetProducts(db);
        }
        public static List<Products> GetFamilyOrCategory(string id, string categoryName, webstoreEntities db)
        {
            return FamilyOrCategory(id, categoryName, db);
        }
        private static List<Products> FamilyOrCategory(string id, string categoryName, webstoreEntities db)
        {
            ElectropEntities dbE = new ElectropEntities();
            if (categoryName == null)
            {
                var s = GetProducts(db).Where(x => x.FamilySeo == id)
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

                s = SetBrands(s, db);

                s = CheckOffertOrOffertTime(s, db);

                return s;
            }
            else
            {
                var s = GetProducts(db).Where(x => x.CategorySeo == categoryName)
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


                s = SetBrands(s, db);

                s = CheckOffertOrOffertTime(s, db);

                return s;
            }
        }
        private static List<GetProducts> GetProducts(webstoreEntities db)
        {
            return (from p in db.tblProducts
                    join f in db.tblFamily
                    on p.refFamily equals f.idFamily
                    join c in db.tblCategories
                    on p.refCategory equals c.idCategoria
                    select new GetProducts
                    {
                        Id = p.idProduct,
                        Codigo = p.strCode,
                        Name = p.strName,
                        Price = p.intPrice,
                        Category = c.strNombre,
                        Offert = p.refOffert,
                        OffertTime = p.refOfferTime,
                        FamilySeo = f.strSeo,
                        CategorySeo = c.strNombre
                    }).ToList();
        }
        private static List<Products> SetBrands(List<Products> products,webstoreEntities db)
        {
            var brands = (from a in db.tblRelBrand
                          join b in db.tblBrand
                          on a.refBrand equals b.idBrand
                          select new
                          {
                              refProd = a.refProd,
                              brand = b.strName
                          }).ToList();

            foreach (var i in products)
            {
                if (brands.Any(b => b.refProd == i.strCodigo))
                {
                    i.Brand = brands.Where(b => b.refProd == i.strCodigo).Select(l => l.brand).FirstOrDefault();
                }
            }

            return products;
        }

        private static string AddQuotingQueDet(webstoreEntities db, tblQuotingQue quotingQue, QuotingsProductList quotingsProduct)
        {
            var quotingQueDet = db.tblQuotingQueDet.Where(x => x.refQuotingQue == quotingQue.IdQuotingQue && x.refCodProd == quotingsProduct.Code).FirstOrDefault();
            if (quotingQueDet != null)
            {
                try
                {
                    quotingQueDet.Quantity += quotingsProduct.Quantity;
                    db.SaveChanges();
                    return "ok";
                }
                catch (Exception ex)
                {
                    return ex.ToString();                    
                }
            }
            else
            {
                try
                {
                    tblQuotingQueDet newQuotingQueDet = new tblQuotingQueDet
                    {
                        refQuotingQue = quotingQue.IdQuotingQue,
                        refCodProd = quotingsProduct.Code,
                        Quantity = quotingsProduct.Quantity
                    };

                    db.tblQuotingQueDet.Add(newQuotingQueDet);
                    db.SaveChanges();
                    return "ok";                    
                }
                catch (Exception ex)
                {
                    return ex.ToString();                   
                }
            }
        }
        private static string AddCartQueDet(webstoreEntities db, tblCartQue cartQue, CartProductList cartProduct)
        {
            var quotingQueDet = db.tblCartQueDet.Where(x => x.refCartQue == cartQue.IdCartQue && x.refCodProd == cartProduct.Code).FirstOrDefault();
            if (quotingQueDet != null)
            {
                try
                {
                    quotingQueDet.Quantity += cartProduct.Quantity;
                    db.SaveChanges();
                    return "ok";
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
            else
            {
                try
                {
                    tblCartQueDet newCartQueDet = new tblCartQueDet
                    {
                        refCartQue = cartQue.IdCartQue,
                        refCodProd = cartProduct.Code,
                        Quantity = cartProduct.Quantity
                    };

                    db.tblCartQueDet.Add(newCartQueDet);
                    db.SaveChanges();
                    return "ok";
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
        }

        private static List<CartProductList> CartProducts(List<tblCartQueDet> products, webstoreEntities db)
        {
            List<Products> productList = new List<Products>();

            foreach (var i in products)
            {
                productList.Add(
                    GetProductsList(db).Where(x => x.Codigo == i.refCodProd).Select(x => new Products
                    {

                        strCodigo = x.Codigo,
                        strNombre = x.Name,
                        intPrecio = Function.FormatNumber(x.Price),
                        intPrecioNum = x.Price,
                        categorySeo = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x.Category),
                        Offert = x.Offert,
                        OffertTime = x.OffertTime
                    }).FirstOrDefault()
                );

            }

            productList = GetOffertOrOffertTime(productList, db);

            List<CartProductList> productsToReturn = new List<CartProductList>();

            foreach (var i in productList)
            {
                int quantity = products.Where(p => p.refCodProd == i.strCodigo).Select(s => s.Quantity).FirstOrDefault();
                productsToReturn.Add(new CartProductList
                {
                    Code = i.strCodigo,
                    Name = i.strNombre,
                    Price = i.intPrecio,
                    PriceInt = i.intPrecioNum,
                    PriceOff = i.intPrecioOff,
                    PriceOffInt = i.intPrecioOffNum,
                    Category = i.categorySeo,
                    PercentageOff = i.intPrecentOff,
                    SubtotalInt = i.intPrecioNum * quantity,
                    SubtotalStr = Function.FormatNumber(i.intPrecioNum * quantity),
                    TotalInt = i.intPrecioOffNum > 0 ? i.intPrecioOffNum * quantity : i.intPrecioNum * quantity,
                    TotalStr = Function.FormatNumber(i.intPrecioOffNum > 0 ? i.intPrecioOffNum * quantity : i.intPrecioNum * quantity),
                    Quantity = quantity
                });
            }

            return productsToReturn;
        }
    }

    public sealed class Configuration
    {
        private static MailAddress fromEmail = new MailAddress("eepp@eepp.cl", "Electro Productos");
        private static string fromEmailPassword = "eepp.2019.";
        public static SmtpClient GetSmtp()
        {
            return new SmtpClient
            {
                Host = "mail.eepp.cl",
                Port = 25,
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };
        }

        public static MailAddress GetEmail()
        {
            return fromEmail;
        }
    }

    public sealed class CookieHelper
    {
        private HttpRequestBase _request;
        private HttpResponseBase _response;

        public CookieHelper(HttpRequestBase request,
        HttpResponseBase response)
        {
            _request = request;
            _response = response;
        }

        //[DebuggerStepThrough()]
        public void SetLoginCookie(string userName, string password, bool isPermanentCookie)
        {
            if (_response != null)
            {
                if (isPermanentCookie)
                {
                    FormsAuthenticationTicket userAuthTicket = new FormsAuthenticationTicket(1, userName, DateTime.Now, DateTime.MaxValue, true, password, FormsAuthentication.FormsCookiePath);
                    string encUserAuthTicket = FormsAuthentication.Encrypt(userAuthTicket);
                    HttpCookie userAuthCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encUserAuthTicket);
                    if (userAuthTicket.IsPersistent) userAuthCookie.Expires = userAuthTicket.Expiration;
                    userAuthCookie.Path = FormsAuthentication.FormsCookiePath;
                    _response.Cookies.Add(userAuthCookie);
                }
                else
                {
                    FormsAuthenticationTicket userAuthTicket = new FormsAuthenticationTicket(userName,false, 60);
                    string encUserAuthTicket = FormsAuthentication.Encrypt(userAuthTicket);
                    HttpCookie userAuthCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encUserAuthTicket);
                    userAuthCookie.Expires = DateTime.Now.AddMinutes(60);
                    _response.Cookies.Add(userAuthCookie);
                }
            }
        }
    }
}