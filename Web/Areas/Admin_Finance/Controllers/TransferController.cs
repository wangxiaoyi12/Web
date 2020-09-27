using Business;
using System;
using System.Web.Mvc;
using Web.Controllers;

namespace Web.Areas.Admin_Finance.Controllers
{
    public class TransferController : AdminBaseController
    {
        // GET: Admin_Finance/Transfer
        public ActionResult Index()
        {
            return View();
        }

        public string getDataSource(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.Fin_Transfer.getDataSource(null, startTime, end, key, out total, start, length);
            return ToPage(list, total, start, length, draw);
        }
        #region 导出excel
        public FileResult ToExcel(DateTime? startTime, DateTime? end, string key)
        {
            int total = 0;
            var list = DB.Fin_Transfer.getDataSource(null, startTime, end, key, out total, 0, int.MaxValue);
            return base.ToExcel(list);
        }
        #endregion
    }
}