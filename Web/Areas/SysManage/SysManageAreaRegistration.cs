using System.Web.Mvc;

namespace Web.Areas.SysManage
{
    public class SysManageAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "SysManage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "SysManage_default",
                "SysManage/{controller}/{action}/{id}",
                new { controller = "Account", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}