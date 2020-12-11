using System.Web.Mvc;

namespace Web.Areas.Admin_Infos
{
    public class Admin_InfosAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin_Infos";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                   "Admin_Infos_default",
                "Admin_Infos/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}