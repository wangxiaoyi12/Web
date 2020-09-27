using System.Web.Mvc;

namespace Web.Areas.Member_Center
{
    public class Member_CenterAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Member_Center";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Member_Center_default",
                "Member_Center/{controller}/{action}/{id}/",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}