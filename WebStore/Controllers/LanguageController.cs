using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;
using WebStore.Routing;

namespace WebStore.Controllers
{
    public class LanguageController : Controller
    {
        // GET: Language
        public void ChangeCulture(string language)
        {
            if (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName != language)
            {
                if (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "es" && language == "en")
                {
                    Translate enObj = new Translate();
                    var en = enObj.En();

                    Change(Request.UrlReferrer.ToString(), language, en);
                }
                else if (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "en" && language == "es")
                {
                    Translate esObj = new Translate();
                    var es = esObj.Es();

                    Change(Request.UrlReferrer.ToString(), language, es);
                }
                else if (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "en" && language == "ja")
                {
                    var ja = new Dictionary<string, string>
                    {
                        { "Home", "いえ" },
                        { "Index", "インデッ" }
                    };

                    Change(Request.UrlReferrer.ToString(), language, ja);
                }
                else if (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "es" && language == "ja")
                {
                    var ja = new Dictionary<string, string>
                    {
                        { "Inicio", "いえ" },
                        { "Index", "インデッ" }
                    };

                    Change(Request.UrlReferrer.ToString(), language, ja);
                }
                else if (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "ja" && language == "es")
                {
                    var ja = new Dictionary<string, string>
                    {
                        { "いえ","Inicio" },
                        { "インデッ","Index" }
                    };

                    Change(Request.UrlReferrer.ToString(), language, ja);
                }
                else if (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "ja" && language == "en")
                {
                    var ja = new Dictionary<string, string>
                    {
                        { "いえ","Home" },
                        { "インデッ","Index" }
                    };

                    Change(Request.UrlReferrer.ToString(), language, ja);
                }
            }
        }

        private void Change(string url, string lang, Dictionary<string,string> dir)
        {
            string[] parts = url.Split('/');

            string controller = parts[4];

            string action = parts[5];

            var x = dir[controller];

            var y = dir[action];

            if (parts.Length == 7)
            {
                var id = parts[6];
                Translate translate = new Translate();
                var translation = translate.getCategoriesToTranslate(Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName, lang);
                var z = translation[id];
                setCultureChanges(lang);
                Response.RedirectToRoute(new { controller = x, action = y, language = lang, id = z });

            }
            else if (parts.Length == 8)
            {
                var id = parts[6];
                var idp = parts[7];
                Translate translate = new Translate();
                var translation = translate.getCategoriesToTranslate(Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName, lang);
                var z = translation[id];
                var w = translation[idp];
                setCultureChanges(lang);
                Response.RedirectToRoute(new { controller = x, action = y, language = lang, id = z, idp = w });

            }
            else
            {
                setCultureChanges(lang);
                Response.RedirectToRoute(new { controller = x, action = y, language = lang });
            }
        }

        private void setCultureChanges(string lang)
        {
            Response.Cookies.Remove("CultureInfo");

            HttpCookie languageCookie = System.Web.HttpContext.Current.Request.Cookies["CultureInfo"];

            if (languageCookie == null) languageCookie = new HttpCookie("CultureInfo");

            languageCookie.Value = lang;

            languageCookie.Expires = DateTime.Now.AddDays(10);

            Response.SetCookie(languageCookie);


            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
        }
    }
}