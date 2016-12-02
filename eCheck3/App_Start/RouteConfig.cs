using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace eCheck3
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                // Add the namespace of your desktop controllers here
                new[] { "eCheck3.Controllers" }
            );
        }
    }
}
