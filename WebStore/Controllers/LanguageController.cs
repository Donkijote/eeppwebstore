using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;

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
                    var es = new Dictionary<string, string>
                    {
                        { "Inicio", "Home" },
                        { "Index", "Index" },
                        { "Nosotros", "About" },
                        { "Contactos", "Contact" },
                        { "Cuenta", "Account" },
                        { "Ordenes", "Orders" },
                        { "Rastreo", "Traking" },
                        { "Detalles", "Details" },
                        { "Direcciones", "Address" },
                        { "Configuracion", "Settings" },
                        { "Deseos", "WishList" },
                        { "Categorias", "Categories" },
                        { "Usuario", "User" },
                        { "Carrito", "Cart" },
                        { "Pagar", "CheckOut" },
                        { "Iniciar", "LogIn" },
                        { "Promociones", "Promotions" },
                        { "Todas", "All" },
                        { "CambiarIdioma", "ChangeCulture" },
                        { "Idioma", "Language" },
                        { "Añadir", "Add" },
                        { "Editar", "Edit" },
                        { "Tickets", "Tickets" }

                    };

                    Change(Request.UrlReferrer.ToString(), language, es);
                }
                else if (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "en" && language == "es")
                {
                    var es = new Dictionary<string, string>
                    {
                        { "Home", "Inicio" },
                        { "Index", "Index" },
                        { "About", "Nosotros" },
                        { "Contact", "Contactos" },
                        { "Account", "Cuenta" },
                        { "Orders", "Ordenes" },
                        { "Traking", "Rastreo" },
                        { "Details", "Detalles" },
                        { "Address", "Direcciones" },
                        { "Settings", "Configuracion" },
                        { "WishList", "Deseos" },
                        { "Categories", "Categorias" },
                        { "User", "Usuario" },
                        { "Cart", "Carrito" },
                        { "CheckOut", "Pagar" },
                        { "LogIn", "Iniciar" },
                        { "Promotions", "Promociones" },
                        { "All", "Todas" },
                        { "ChangeCulture", "CambiarIdioma" },
                        { "Language", "Idioma" },
                        { "Add", "Añadir" },
                        { "Edit", "Editar" },
                        { "Tickets", "Tickets" }
                    };

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

            string newUrl = lang + "/" + x + "/" + y;

            Response.Cookies.Remove("CultureInfo");

            HttpCookie languageCookie = System.Web.HttpContext.Current.Request.Cookies["CultureInfo"];

            if (languageCookie == null) languageCookie = new HttpCookie("CultureInfo");

            languageCookie.Value = lang;

            languageCookie.Expires = DateTime.Now.AddDays(10);

            Response.SetCookie(languageCookie);


            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);

            Response.RedirectToRoute(new { controller = x, action = y, language = lang });
        }
    }
}