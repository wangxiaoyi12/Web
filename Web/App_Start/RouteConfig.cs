using System.Web.Mvc;
using System.Web.Routing;

namespace Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
           name: "LoginIndex",
           url: "",
           defaults: new { controller = "MemberLogin", action = "Index", id = UrlParameter.Optional }
           ).DataTokens.Add("Area", "Mobile");

            routes.MapRoute(
             name: "Index",
             url: "member",
             defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional }
             ).DataTokens.Add("Area", "SysManage");

            routes.MapRoute(
               name: "JNWZ200915",
               url: "JNWZ200915",
               defaults: new { controller = "Account", action = "Admin_Index", id = UrlParameter.Optional }
            ).DataTokens.Add("Area", "SysManage");

            routes.MapRoute(
 name: "EveryDingShi",
  url: "Reg/EveryDingShi",
  defaults: new { controller = "Reg", action = "EveryDingShi", id = UrlParameter.Optional }
 );
            routes.MapRoute(
name: "EveryUpdateOrder",
url: "Reg/EveryUpdateOrder",
defaults: new { controller = "Reg", action = "EveryUpdateOrder", id = UrlParameter.Optional }
);

            routes.MapRoute(
              name: "RegSave",
              url: "Reg/Save",
              defaults: new { controller = "Reg", action = "Save", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "Reg",
               url: "Reg/{id}",
               defaults: new { controller = "Reg", action = "Index", id = UrlParameter.Optional }
           );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
