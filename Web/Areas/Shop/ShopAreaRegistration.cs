using System.Web.Mvc;

namespace Web.Areas.Shop
{
    public class ShopAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Shop";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Shop_default2",
                "Shop/{controller}/{id}",
                new { controller = "Index", action = "Index", id = UrlParameter.Optional },
                new { id=@"\d+"}
            );
            context.MapRoute(
                "Shop_default",
                "Shop/{controller}/{action}/{id}",
                new { controller="Index", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}