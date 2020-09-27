using System.Web.Mvc;
using Business;
using System;

namespace Web.Areas.Member_Member.Controllers
{
    /// <summary>
    /// 修改密码
    /// </summary>
    [Web.Controllers.Permissions]
    public class PasswordController : Web.Controllers.MemberBaseController
    {
        // GET: Member_Member/Password
        public ActionResult Index()
        {
            var m = DB.Member_Info.FindEntity(CurrentUser.Id);
            ViewBag.Code = m.Code;
            return View();
        }

        #region 修改密码
        public ActionResult Save()
        {
            var type = Request["PwdType"];
            var oldPwd = Request["OldPwd"];
            var Password = Request["Password"];

            var r = DB.Member_Info.ModifyPassword(CurrentUser.Id, type, oldPwd, Password);
            if (r.IsSuccess)
            {
                Tools.WriteCookie(DB.Member_Info.FindEntity(CurrentUser.Id));
            }
            return Json(r);
        }
        #endregion

    }
}