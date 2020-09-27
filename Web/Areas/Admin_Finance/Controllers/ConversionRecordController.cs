using System;
using System.Web.Mvc;
using Business;

namespace Web.Areas.Admin_Finance.Controllers
{
    public class ConversionRecordController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_Finance/ConversionRecord
        public ActionResult Index()
        {
            return View();
        }

        #region  查询
        public string getDataSource(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.Fin_Convert.getDataSource(null, startTime, end, key, out total, start, length);

            return ToPage(list, total, start, length, draw);
        }
        #endregion 

        #region 导出excel
        public FileResult ToExcel(DateTime? startTime, DateTime? end, string key)
        {
            int total = 0;
            var list = DB.Fin_Convert.getDataSource(null, startTime, end, key, out total, 0, int.MaxValue);
            return base.ToExcel(list);
        }
        #endregion 

    }
}