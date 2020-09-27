using System.Web.Mvc;
using Business;
using System;

namespace Web.Areas.Member_Center.Controllers
{
    /// <summary>
    /// 申请店主
    /// </summary>
    public class ServiceApplyController : Web.Controllers.MemberBaseController
    {
        // GET: Member_Center/ServiceApply
        public ActionResult Index()
        {
            var model = DB.Member_Info.FindEntity(CurrentUser.Id);
            return View(model);
        }

        /// <summary>
        /// 提交申请
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ActionResult Save()
        {
            var entity = DB.Member_Info.FindEntity(CurrentUser.Id);
            return Json(DB.Member_Info.ServiceApply(entity));
        }
    }
}