using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebStore
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            System.Globalization.CultureInfo cultureEN = System.Globalization.CultureInfo.GetCultureInfo("en");
            System.Globalization.CultureInfo cultureJA = System.Globalization.CultureInfo.GetCultureInfo("ja");
            System.Globalization.CultureInfo cultureES = System.Globalization.CultureInfo.GetCultureInfo("es");

            DictionaryRouteValueTranslationProvider translationProvider = new DictionaryRouteValueTranslationProvider(
                new List<RouteValueTranslation> {
                new RouteValueTranslation(cultureEN, "Home", "Home"),
                new RouteValueTranslation(cultureEN, "About", "About"),
                new RouteValueTranslation(cultureEN, "Contact", "Contact"),
                new RouteValueTranslation(cultureEN, "History", "History"),
                new RouteValueTranslation(cultureEN, "Knowus", "Knowus"),
                new RouteValueTranslation(cultureEN, "Account", "Account"),
                new RouteValueTranslation(cultureEN, "Orders", "Orders"),
                new RouteValueTranslation(cultureEN, "Traking", "Traking"),
                new RouteValueTranslation(cultureEN, "Details", "Details"),
                new RouteValueTranslation(cultureEN, "Address", "Address"),
                new RouteValueTranslation(cultureEN, "Add", "Add"),
                new RouteValueTranslation(cultureEN, "Edit", "Edit"),
                new RouteValueTranslation(cultureEN, "Settings", "Settings"),
                new RouteValueTranslation(cultureEN, "WishList", "WishList"),
                new RouteValueTranslation(cultureEN, "Category", "Category"),
                new RouteValueTranslation(cultureEN, "Categories", "Categories"),
                new RouteValueTranslation(cultureEN, "User", "User"),
                new RouteValueTranslation(cultureEN, "Cart", "Cart"),
                new RouteValueTranslation(cultureEN, "CheckOut", "CheckOut"),
                new RouteValueTranslation(cultureEN, "LogIn", "LogIn"),
                new RouteValueTranslation(cultureEN, "Promotions", "Promotions"),
                new RouteValueTranslation(cultureEN, "All", "All"),
                new RouteValueTranslation(cultureEN, "ChangeCulture", "ChangeCulture"),
                new RouteValueTranslation(cultureEN, "Language", "Language"),
                new RouteValueTranslation(cultureEN, "Tickets", "Tickets"),
                new RouteValueTranslation(cultureEN, "Offerts", "Offerts"),
                new RouteValueTranslation(cultureEN, "Products", "Products"),
                new RouteValueTranslation(cultureEN, "Product", "Product"),
                new RouteValueTranslation(cultureEN, "Downloads", "Downloads"),
                new RouteValueTranslation(cultureEN, "ProductTecnicalSheetPDF", "ProductTecnicalSheetPDF"),
                new RouteValueTranslation(cultureEN, "ProductSecCertificationPDF", "ProductSecCertificationPDF"),
                new RouteValueTranslation(cultureEN, "ProductDs43CertificationPDF", "ProductDs43CertificationPDF"),
                new RouteValueTranslation(cultureES, "Home", "Inicio"),
                new RouteValueTranslation(cultureES, "About", "Nosotros"),
                new RouteValueTranslation(cultureES, "Contact", "Contactos"),
                new RouteValueTranslation(cultureES, "History", "Historia"),
                new RouteValueTranslation(cultureES, "Knowus", "Conocenos"),
                new RouteValueTranslation(cultureES, "Account", "Cuenta"),
                new RouteValueTranslation(cultureES, "Orders", "Ordenes"),
                new RouteValueTranslation(cultureES, "Traking", "Rastreo"),
                new RouteValueTranslation(cultureES, "Details", "Detalles"),
                new RouteValueTranslation(cultureES, "Address", "Direcciones"),
                new RouteValueTranslation(cultureES, "Add", "Añadir"),
                new RouteValueTranslation(cultureES, "Editar", "Editar"),
                new RouteValueTranslation(cultureES, "Settings", "Configuracion"),
                new RouteValueTranslation(cultureES, "WishList", "Deseos"),
                new RouteValueTranslation(cultureES, "Category", "Categoria"),
                new RouteValueTranslation(cultureES, "Categories", "Categorias"),
                new RouteValueTranslation(cultureES, "User", "Usuario"),
                new RouteValueTranslation(cultureES, "Cart", "Carrito"),
                new RouteValueTranslation(cultureES, "CheckOut", "Pagar"),
                new RouteValueTranslation(cultureES, "LogIn", "Iniciar"),
                new RouteValueTranslation(cultureES, "Promotions", "Promociones"),
                new RouteValueTranslation(cultureES, "All", "Todas"),
                new RouteValueTranslation(cultureES, "ChangeCulture", "CambiarIdioma"),
                new RouteValueTranslation(cultureES, "Language", "Idioma"),
                new RouteValueTranslation(cultureES, "Tickets", "Tickets"),
                new RouteValueTranslation(cultureES, "Offerts", "Ofertas"),
                new RouteValueTranslation(cultureES, "Products", "Productos"),
                new RouteValueTranslation(cultureES, "Product", "Producto"),
                new RouteValueTranslation(cultureES, "Downloads", "Descargas"),
                new RouteValueTranslation(cultureES, "ProductTecnicalSheetPDF", "FichaTecnicaProductoPDF"),
                new RouteValueTranslation(cultureES, "ProductSecCertificationPDF", "CertificationSecProductoDPF"),
                new RouteValueTranslation(cultureES, "ProductDs43CertificationPDF", "CertificationDs43ProductoDPF"),
                new RouteValueTranslation(cultureJA, "Home", "いえ"),
                new RouteValueTranslation(cultureJA, "Index", "インデッ")
                }
            );

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapTranslatedRoute(
                "TranslatedRoute",
                "{language}/{controller}/{action}/{id}/{idp}",
                new { language = System.Threading.Thread.CurrentThread.CurrentUICulture.Name, controller = "Home", action = "Lang", id = UrlParameter.Optional, idp = UrlParameter.Optional },
                new { controller = translationProvider, action = translationProvider },
                true
            );

            routes.MapRoute(
                name: "Default",
                url: "{language}/{controller}/{action}/{id}/{idp}",
                defaults: new { language = System.Threading.Thread.CurrentThread.CurrentUICulture.Name, controller = "Home", action = "Lang", id = UrlParameter.Optional, idp = UrlParameter.Optional }
            );

        }
    }
}
