using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Threading;
using System.Globalization;

namespace WebStore
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {

            HttpCookie languageCookie = HttpContext.Current.Request.Cookies["CultureInfo"];
            if (languageCookie != null && languageCookie.Value != null)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(languageCookie.Value);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(languageCookie.Value);
            }
            else
            {
                if (Request.UserLanguages != null)
                {
                    string newLanguage = Request.UserLanguages[0];

                    if (newLanguage == "es-419")
                    {
                        newLanguage = "es";
                    }
                    if (newLanguage != "es" || newLanguage != "en")
                    {
                        newLanguage = "es";
                    }
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(newLanguage);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(newLanguage);
                }
            }

            /*var routes = RouteTable.Routes;

            var httpContext = Request.RequestContext.HttpContext;
            if (httpContext == null) return;

            var routeData = routes.GetRouteData(httpContext);

            var language = routeData.Values["language"] as string;
            var cultureInfo = new CultureInfo(language);

            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
            System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;*/

        }

        protected void Application_EndRequest()
        {
            if (Context.Response.StatusCode == 404)
            {
                Response.Clear();

                var rd = new RouteData();
                rd.Values["controller"] = "Error";
                rd.Values["action"] = "Http404";

                var rc = new RequestContext(new HttpContextWrapper(Context), rd);
                var c = ControllerBuilder.Current.GetControllerFactory().CreateController(rc, "Error");
                c.Execute(rc);
            }
        }
    }
}
