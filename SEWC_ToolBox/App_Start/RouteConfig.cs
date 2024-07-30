using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SEWC_ToolBox_Project
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            #region 页面路由

            routes.MapRoute(
                name: "Error",
                url: "Error/{action}/{id}",
                defaults: new { controller = "Error", action = "NotFound", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SCM_Entrance",
                url: "SCM/{action}/{Sub_Action}/{Third_Action}/{Fourth_Action}",
                defaults: new { controller = "SCM", action = "Home", Sub_Action = UrlParameter.Optional, Third_Action = UrlParameter.Optional, Fourth_Action = UrlParameter.Optional }
            );

            routes.MapRoute(
                   name: "QM_Entrance",
                   url: "QM/{action}/{Sub_Action}/{Third_Action}/{Fourth_Action}",
                   defaults: new { controller = "QM", action = "Home", Sub_Action = UrlParameter.Optional, Third_Action = UrlParameter.Optional, Fourth_Action = UrlParameter.Optional }
               );

            routes.MapRoute(
                name: "IT_Entrance",
                url: "IT/{action}/{Sub_Action}/{Third_Action}/{Fourth_Action}",
                defaults: new { controller = "IT", action = "Organization", Sub_Action = UrlParameter.Optional, Third_Action = UrlParameter.Optional, Fourth_Action = UrlParameter.Optional }
            );

            // routes.MapRoute(
            //    name: "SWEC_Entrance",
            //    url:  "{controller}/{action}/{Sub_Action}/{Third_Action}/{Fourth_Action}",
            //    defaults: new { controller = "SEWC", action = "Home", Entrance = UrlParameter.Optional, Sub_Action = UrlParameter.Optional, Third_Action = UrlParameter.Optional, Fourth_Action = UrlParameter.Optional }
            //);

            routes.MapRoute(
                name: "ToolBoxFunctional",
                url: "ToolBoxFunctional/{action}/{id}",
                defaults: new { controller = "ToolBoxFunctional", action = "Reporting_Add", id = UrlParameter.Optional }
            );

            routes.MapRoute(
              name: "SSML_Entrance",
              url: "{controller}/{action}/{Sub_Action}/{Third_Action}/{Fourth_Action}",
              defaults: new { controller = "SSML", action = "Home", Entrance = UrlParameter.Optional, Sub_Action = UrlParameter.Optional, Third_Action = UrlParameter.Optional, Fourth_Action = UrlParameter.Optional }
          );

            #endregion

            #region 缺省默认匹配主页

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "HomePage", id = UrlParameter.Optional }
            //);
            routes.MapRoute(
                name: "Default",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "SEWC", action = "Home", id = UrlParameter.Optional }
            );
            #endregion

        }
    }
}
