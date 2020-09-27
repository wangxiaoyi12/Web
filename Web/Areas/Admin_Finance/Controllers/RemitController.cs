using System;
using System.Web.Mvc;
using Business;

namespace Web.Areas.Admin_Finance.Controllers
{
    public class RemitController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_Finance/Remit
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Indexs()
        {
            return View();
        }
        #region 查询
        public string getDataSource(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.Fin_Remit.getDataSource(null, startTime, end, key, out total, start, length);

            return ToPage(list, total, start, length, draw);
        }
        public string getDataSources(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.Fin_LiuShui.getDataSources(null, startTime, end, key, out total, start, length);

            return ToPage(list, total, start, length, draw);
        }
        #endregion

        #region 审核        
        public ActionResult Isok(int id, int type)
        {
            return Json(DB.Fin_Remit.Isok(id, type, CurrentUser.Id, CurrentUser.Name));
        }
        #endregion

        #region 删除
        public ActionResult Delete(string idList)
        {
            return Json(DB.Fin_Remit.Delete(idList));
        }
        #endregion

        #region 导出excel
        public FileResult ToExcel(DateTime? startTime, DateTime? end, string key)
        {
            int total = 0;
            var list = DB.Fin_Remit.getDataSource(null, startTime, end, key, out total, 0, int.MaxValue);
            return base.ToExcel(list);
        }
        #endregion
    }
}