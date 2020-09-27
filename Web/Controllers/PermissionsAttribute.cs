using System;
using System.Web.Mvc;
using System.Web;
namespace Web.Controllers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PermissionsAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 全局过滤器，判断用户是否登陆以及用户可查看菜单权限
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //var controllerName = (filterContext.RouteData.Values["controller"]).ToString();
            //var actionName = (filterContext.RouteData.Values["action"]).ToString();
            //var areaName = (filterContext.RouteData.DataTokens["area"] == null ? "" : filterContext.RouteData.DataTokens["area"]).ToString();

            //var url = "/" + areaName + "/" + controllerName;

            //Account User = null;
            //if (controllerName.StartsWith("Admin_"))
            //{
            //    User = Common.SessionHelper.GetSession(Enums.LoginType.admin) as Account;
            //}
            //else
            //{
            //    User = Common.SessionHelper.GetSession(Enums.LoginType.member) as Account;
            //}
            //var menulist = MenuManage.getMenuByLinkUrl(url);
            //if (actionName == "Detail")
            //{
            //    if (filterContext.ActionParameters["id"] == null)
            //    {
            //        var data = User.Role_Nav.FindAll(p => p.nav_name == menulist.name && p.action_type == "Add");
            //        if (data.Count == 0)
            //        {
            //            //JsonResult json = new JsonResult();
            //            //json.Data = new { Status = "n", Msg = "对不起，您没有新建权限" };
            //            //filterContext.Result = json;
            //        }
            //    }
            //    else
            //    {
            //        var data = User.Role_Nav.FindAll(p => p.nav_name == menulist.name && p.action_type == "View");
            //        if (data.Count == 0)
            //        {
            //            //JsonResult json = new JsonResult();
            //            //json.Data = new { Status = "n", Msg = "对不起，您没有查看权限" };
            //            //filterContext.Result = json;
            //        }
            //    }
            //}
            //if (actionName == "Save")
            //{
            //    var data = User.Role_Nav.FindAll(p => p.nav_name == menulist.name && p.action_type == "Edit");
            //    if (data.Count == 0)
            //    {
            //        //JsonResult json = new JsonResult();
            //        //json.Data = new { Status = "n", Msg = "对不起，您没有编辑权限" };
            //        //filterContext.Result = json;
            //    }
            //}
            //if (actionName == "Delete")
            //{
            //    string redirectOnSuccess = filterContext.HttpContext.Request.Url.AbsolutePath;
            //    var data = User.Role_Nav.FindAll(p => p.nav_name == menulist.name && p.action_type == "Delete");
            //    if (data.Count == 0)
            //    {
            //        JsonResult json = new JsonResult();
            //        json.Data = new { Status = "n", Msg = "对不起，您没有删除权限" };
            //        filterContext.Result = json;
            //    }
            //}
        }
    }
}
namespace System.Web.Mvc
{
    /// <summary>
    /// 商城用户验证处理
    /// </summary>
    public class ShopUserCheckAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // base.OnActionExecuting(filterContext);

            //判断用户是否登录
            if (User_Shop.IsLogin() == false)
            {
                ActionResult result = new RedirectResult(Url_Shop.GetLogin());
                filterContext.Result = result;
            }
        }
    }
}