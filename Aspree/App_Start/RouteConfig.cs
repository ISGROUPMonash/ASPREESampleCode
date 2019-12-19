using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Aspree
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                name: "Index-Page",
                url: "{controller}",
                defaults: new {  action = "Index" }
            );

            //routes.MapRoute(
            //    name: "Test",
            //    url: "test/{controller}/{action}/{guid}",
            //    defaults: new { controller = "Account", action = "Login", guid = UrlParameter.Optional }
            //);
            routes.MapRoute(
                name: "Test",
                url: "uds-test/{controller}/{action}/{guid}",
                defaults: new { controller = "Account", action = "Login", guid = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{guid}",
                defaults: new { controller = "Account", action = "Login", guid = UrlParameter.Optional }
            );

        }
    }
}
