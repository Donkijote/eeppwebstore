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

        public static MailMessage GenerateEmail(MailAddress toEmail, string subject, string body)
        {
            var message = new MailMessage(Configuration.GetEmail(), toEmail)
            {
                Subject = subject,
                Body = Function.PopulateEmailBody(body),
                IsBodyHtml = true
            };
            return message;
        }

        public static string PopulateEmailBody(string bodyContent)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/Shared/_EmailTemplate.html")))
            {
                body = reader.ReadToEnd();
            }
            //body = body.Replace("{Url}", url);
            body = body.Replace("{BodyContent}", bodyContent);
            body = body.Replace("{unSubcribeUrl}", "#");
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
                            a.Time = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeDiff.Days, timeDiff.Hours, timeDiff.Minutes, timeDiff.Seconds);
                        }
                    }
                }
            }

            return pro;
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
                var s = (from p in db.tblProducts
                         join f in db.tblFamily
                         on p.refFamily equals f.idFamily
                         join c in db.tblCategories
                         on p.refCategory equals c.idCategoria
                         where f.strSeo == id
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

                var brands = (from a in db.tblRelBrand
                              join b in db.tblBrand
                              on a.refBrand equals b.idBrand
                              select new
                              {
                                  refProd = a.refProd,
                                  brand = b.strName
                              }).ToList();

                foreach (var i in s)
                {
                    if (brands.Any(b => b.refProd == i.strCodigo))
                    {
                        i.Brand = brands.Where(b => b.refProd == i.strCodigo).Select(l => l.brand).FirstOrDefault();
                    }
                }

                s = CheckOffertOrOffertTime(s, db);

                return s;
            }
            else
            {
                var s = (from p in db.tblProducts
                         join f in db.tblFamily
                         on p.refFamily equals f.idFamily
                         join c in db.tblCategories
                         on p.refCategory equals c.idCategoria
                         where c.strNombre == categoryName
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
                

                var brands = (from a in db.tblRelBrand
                              join b in db.tblBrand
                              on a.refBrand equals b.idBrand
                              select new
                              {
                                  refProd = a.refProd,
                                  brand = b.strName
                              }).ToList();

                foreach (var i in s)
                {
                    if (brands.Any(b => b.refProd == i.strCodigo))
                    {
                        i.Brand = brands.Where(b => b.refProd == i.strCodigo).Select(l => l.brand).FirstOrDefault();
                    }
                }

                s = CheckOffertOrOffertTime(s, db);

                return s;
            }
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