﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Bdl_Grupo2_ProyectoFinal_A
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Usuarios", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "Usuario",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Usuario", action = "Users", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "Tecnicos",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Tecnicos", action = "Index", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "Ticket",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Ticket", action = "Index", id = UrlParameter.Optional }
           );
        }
    }
}
