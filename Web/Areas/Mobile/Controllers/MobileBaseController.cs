using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DataBase;
using Business;
using Common;
namespace Web.Areas.Mobile.Controllers
{
    /// <summary>
    /// 手机端基础类
    /// </summary>
    public class MobileBaseController : Controller
    {

        protected override void OnException(ExceptionContext filterContext)
        {
            //错误处理
            //判断是否登录，判断用户是否存在
            if (User_Shop.IsLogin())
            {
                if (filterContext.RequestContext.HttpContext.Request.HttpMethod.ToLower() == "get")
                {
                    Account curUser = User_Shop.GetAccount();
                    Member_Info model = DB.Member_Info.FindEntity(curUser.Id);
                    if (model == null)
                    {
                        CookieHelper.ClearCookie("openid");
                        User_Shop.Clear();

                        filterContext.Result = new RedirectResult("~/mobile/memberlogin");
                        filterContext.ExceptionHandled = true;
                    }
                }
            }
            base.OnException(filterContext);
        }
        // GET: Mobile/MobileBase
        /// <summary>
        /// 处理成功
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public JsonResult Success(string content)
        {
            object obj = new
            {
                status = 1,
                msg = content
            };
            return Json(obj);
        }
        /// <summary>
        /// 处理失败
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public JsonResult Error(string content)
        {
            object obj = new
            {
                status = 0,
                msg = content
            };
            return Json(obj);
        }

        /// <summary>
        /// 处理失败
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public JsonResult Error(Exception error)
        {
            return Error(error.Message);
        }
        public string MemberID
        {
            get
            {
                var memberid = User_Shop.GetMemberID();
                return memberid;
            }          
        }



    }
}