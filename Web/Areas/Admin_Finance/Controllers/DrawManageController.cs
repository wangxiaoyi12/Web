using System;
using System.Web.Mvc;
using Business;

namespace Web.Areas.Admin_Finance.Controllers
{
    public class DrawManageController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_Finance/DrawManage
        public ActionResult Index()
        {
            return View();
        }

        #region 查询       
        public string getDataSource(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.Fin_Draw.getDataSource(null, startTime, end, key, out total, start, length);

            return ToPage(list, total, start, length, draw);
        }
        #endregion

        #region 审核操作
        public ActionResult Issue(int id, int type, string summary)
        {
            return Json(DB.Fin_Draw.Issue(id, type, CurrentUser.Id, CurrentUser.Name, summary));
        }
        #endregion      

        #region 删除
        public ActionResult Delete(string idList)
        {
            return Json(DB.Fin_Draw.Delete(idList));
        }
        #endregion

        #region 导出excel
        public FileResult ToExcel(DateTime? startTime, DateTime? end, string key)
        {
            int total = 0;
            var list = DB.Fin_Draw.getDataSource(null, startTime, end, key, out total, 0, int.MaxValue);
            return base.ToExcel(list);
        }
        #endregion
    }
}