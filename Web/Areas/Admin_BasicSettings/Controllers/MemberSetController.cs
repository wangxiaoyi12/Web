using System.Web.Mvc;
using Business;
using System;

namespace Web.Areas.Admin_BasicSettings.Controllers
{
    public class MemberSetController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_BasicSettings/MemberSet
        public ActionResult Index()
        {
            ViewData["LevelData"] = DB.Sys_Level.getList();
            return View();
        }

        #region 保存
        public ActionResult Save(string strList)
        {
            return Json(DB.Sys_Level.Save(strList));
        }
        #endregion

        #region 导出excel
        public FileResult ToExcel()
        {
            var list = DB.Sys_Level.getList();
            return base.ToExcel(list);
        }
        #endregion
    }
}