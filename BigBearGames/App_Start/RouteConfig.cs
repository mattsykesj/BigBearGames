using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BigBearGames
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: null,
               url: "Articles/Page{page}",
               defaults: new { Controller = "Article", Action = "Index", articleType = UrlParameter.Optional }
               );

            routes.MapRoute(
                name: null,
                url: "Articles/News/Page{page}",
                defaults: new { Controller = "Article", Action = "Index", articleType = 0 }
                );

            routes.MapRoute(
                name: null,
                url: "Articles/Blogs/Page{page}",
                defaults: new { Controller = "Article", Action = "Index", articleType = 1 }
                );

            routes.MapRoute(
                name: null,
                url: "Articles/Reviews/Page{page}",
                defaults: new { Controller = "Article", Action = "Index", articleType = 2 }
                );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
