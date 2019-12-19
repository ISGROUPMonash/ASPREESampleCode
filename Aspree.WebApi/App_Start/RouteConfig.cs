using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Aspree.WebApi
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
            name: "swagger_root",
            routeTemplate: "",
            defaults: null,
            constraints: null,
            handler: new Swagger.Net.Application.RedirectHandler((message => message.RequestUri.ToString()), "swagger"));


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{guid}",
                //url: "{controller}/{action}/{guid}",
                defaults: new { controller = "Home", action = "Index", guid = UrlParameter.Optional }
            );

           // routes.MapRoute(
           //    name: "MvcDefault",
           //    url: "{controller}/{action}/{guid}",
           //    defaults: new { controller = "Home", action = "Index", guid = UrlParameter.Optional }
           //);
        }
    }
}
