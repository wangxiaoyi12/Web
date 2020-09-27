using System.Linq;
using System.Web.Mvc;
using Business;
using System;

namespace Web.Areas.Member_Member.Controllers
{
    /// <summary>
    /// 会员升级
    /// </summary>
    public class UpgradeController : Web.Controllers.MemberBaseController
    {
        // GET: Member_Member/Upgrade
        public ActionResult Index()
        {
            var m = DB.Member_Info.FindEntity(CurrentUser.Id);
            ViewData["Levels"] = DB.Sys_Level.getList().Where(a => a.LevelId > m.MemberLevelId).OrderBy(a => a.LevelId);

            return View(m);
        }

        /// <summary>
        /// 升级
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ActionResult Save(DataBase.Member_Info DataBase)
        {
            var Pwd2 = Request["Pwd2"];
            var levelId = Request["LevelId"];
            var memberId = CurrentUser.Id;

            var r = DB.Member_Info.Upgrade(memberId, Pwd2, levelId);
            if (r.IsSuccess)
            {
                r.ReUrl = ControllerPath + "/Index";   //刷新本页面
            }
            return Json(r);
        }
    }
}