using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebStore.Routing
{
    public class Translate
    {
        public Dictionary<string, string> En()
        {
            var en = new Dictionary<string, string>
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
                { "Tickets", "Tickets" },
                { "s", "s" }
            };
            return en;
        }

        public Dictionary<string, string> Es()
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
                { "Tickets", "Tickets" },
                { "s", "s" }
            };
            return es;
        }

        public String Translation(string word, string cultureFrom, string CultureTo)
        {
            var dir = CategoriesToEnIds(cultureFrom, CultureTo);

            return dir[word];

        }

        private Dictionary<string, string> CategoriesToEnIds(string cultureFrom, string cultureTo)
        {
            if (cultureFrom == "es" && cultureTo == "en")
            {
                Dictionary<string, string> categoriesES = new Dictionary<string, string>
            {
                { "atmoferas-explosivas", "explosive-atmospheres" },
                { "tableros", "boards" },
                { "botoneras", "bottons" },
                { "interruptores-tomas-y-dimmers", "breakers-power-points-and-dimmers" },
                { "interruptores", "breakers" },
                { "prensa-cables", "cable-press" },
                { "cables", "cables" },
                { "contactor-y-rele-termico", "contactor-and-thermic-rele" },
                { "control-y-comando", "control-and-command" },
                { "cordones", "cordons" },
                { "diferenciales", "differentials" },
                { "cajas-de-distribucion", "distribution-boxes" },
                { "repartidores", "distributors" },
                { "canalizacion-electrica", "electrical-canalization"},
                { "conductores-electricos", "electrical-conductor" },
                { "ferreteria-electrica", "electrical-hardware-store" },
                { "emergencia", "emergency" },
                { "tableros-de-faena", "faena-boards" },
                { "hembras", "females" },
                { "variador-de-frecuencia", "frequency-variator" },
                { "jardin", "garden"},
                { "hermeticas", "hermetics" },
                { "hogar", "home" },
                { "iluminacion-hogar", "home-lights" },
                { "industrial", "industrial" },
                { "iluminacion-industrial", "industrial-lights" },
                { "enchufes-industriales", "industrial-plugs" },
                { "escalerillas", "ladders" },
                { "ampolletas-y-tubos", "lightbulbs-and-tubes"},
                { "machos", "males" },
                { "guarda-motor", "motor-safer" },
                { "otros", "others" },
                { "placas", "plates" },
                { "postes", "posts" },
                { "publica", "public" },
                { "pulsadores", "pushes" },
                { "pvc", "pvc" },
                { "tableros-q-din", "q-din-boards" },
                { "residencial", "residential" },
                { "partidores-suaves", "soft-breaker" },
                { "stadium", "stadium" },
                { "prensa-estopa", "stoma-press" },
                { "solar", "sun" },
                { "bandejas", "trays" },
                { "tubos", "tubes" },
                { "alambres", "wires" }
            };

                return categoriesES;
            }
            else
            {
                Dictionary<string, string> categoriesEN = new Dictionary<string, string>
            {
                { "explosive-atmospheres", "atmoferas-explosivas" },
                { "boards", "tableros" },
                { "bottons", "botoneras" },
                { "breakers-power-points-and-dimmers", "interruptores-tomas-y-dimmers" },
                { "breakers", "interruptores" },
                { "cable-press", "prensa-cables" },
                { "cables", "cables" },
                { "contactor-and-thermic-rele", "contactor-y-rele-termico" },
                { "control-and-command", "control-y-comando" },
                { "cordons", "cordones" },
                { "differentials", "diferenciales" },
                { "distribution-boxes", "cajas-de-distribucion" },
                { "distributors", "repartidores" },
                { "electrical-canalization", "canalizacion-electrica" },
                { "electrical-conductor", "conductores-electricos" },
                { "electrical-hardware-store", "ferreteria-electrica" },
                { "emergency", "emergencia" },
                { "faena-boards", "tableros-de-faena" },
                { "females", "hembras" },
                { "frequency-variator", "variador-de-frecuencia" },
                { "garden", "jardin" },
                { "hermetics", "hermeticas" },
                { "home", "hogar" },
                { "home-lights", "iluminacion-hogar" },
                { "industrial", "industrial" },
                { "industrial-lights", "iluminacion-industrial" },
                { "industrial-plugs", "enchufes-industriales" },
                { "ladders", "escalerillas" },
                { "lightbulbs-and-tubes", "ampolletas-y-tubos" },
                { "males", "machos" },
                { "motor-safer", "guarda-motor" },
                { "others", "otros" },
                { "plates", "placas" },
                { "posts", "postes" },
                { "public", "publica" },
                { "pushes", "pulsadores" },
                { "pvc", "pvc" },
                { "q-din-boards", "tableros-q-din" },
                { "residential", "residencial" },
                { "soft-breaker", "partidores-suaves" },
                { "stadium", "stadium" },
                { "stoma-press", "prensa-estopa" },
                { "sun", "solar" },
                { "trays", "bandejas" },
                { "tubes", "tubos" },
                { "wires", "alambres" }
            };

                return categoriesEN;
            }
        }

        public Dictionary<string, string> getCategoriesToTranslate(string from, string culture)
        {
            return CategoriesToEnIds(from, culture);
        }
    }
}