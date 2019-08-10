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

namespace WebStore.Functions
{
    public class Function
    {
        public string Truncate(string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }

        public string FormatNumber(int number)
        {
            return number.ToString("N0", CultureInfo.GetCultureInfo("es-CL"));
        }

        public string getTitle(string id)
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