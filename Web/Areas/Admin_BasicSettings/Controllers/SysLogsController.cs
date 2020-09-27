using System;
using System.Web.Mvc;
using Business;

namespace Web.Areas.Admin_BasicSettings.Controllers
{
    public class SysLogsController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_BasicSettings/SysLogs
        public ActionResult Index()
        {
            return View();
        }

        #region 加载数据列表      
        public string getSysLogsData(string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.SysLogs.getList(key, out total, start, length);

            return ToPage(list, total, start, length, draw);
        }
        #endregion

        #region 导出excel       
        public FileResult ToExcel(DateTime? startTime, DateTime? end, string key)
        {
            int total = 0;
            var list = DB.SysLogs.getList(key, out total, 0, int.MaxValue);
            return base.ToExcel(list);
        }
        #endregion
    }
}