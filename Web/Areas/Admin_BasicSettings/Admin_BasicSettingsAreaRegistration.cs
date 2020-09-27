using System.Web.Mvc;

namespace Web.Areas.Admin_BasicSettings
{
    public class Admin_BasicSettingsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin_BasicSettings";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admin_BasicSettings_default",
                "Admin_BasicSettings/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}