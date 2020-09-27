using System.Web.Mvc;

namespace Web.Areas.Admin_Information
{
    public class Admin_InformationAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin_Information";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admin_Information_default",
                "Admin_Information/{controller}/{action}/{id}/{type}",
                new { action = "Index", id = UrlParameter.Optional, type = UrlParameter.Optional}
            );
        }
    }
}