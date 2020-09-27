using System.Web.Mvc;

namespace Web.Areas.Admin_DataStatis
{
    public class Admin_DataStatisAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin_DataStatis";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admin_DataStatis_default",
                "Admin_DataStatis/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}