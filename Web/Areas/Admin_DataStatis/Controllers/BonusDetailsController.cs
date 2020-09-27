using System;
using System.Web.Mvc;
using Business;

namespace Web.Areas.Admin_DataStatis.Controllers
{
    public class BonusDetailsController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_DataStatis/BonusDetails
        public ActionResult Index(string id, string code)
        {
            ViewBag.Date = id;
            ViewBag.Code = code;
            return View();
        }

        #region getDataSource
        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <param name="start">开始日期</param>
        /// <param name="end">结束日期</param>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        public string getDataSource(string code, DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var mid = string.Empty;
            if (!string.IsNullOrEmpty(code))
            {
                mid = DB.Member_Info.FindEntity(a => a.Code == code).MemberId;
            }
            var list = DB.Fin_Info.getDataSource(mid, startTime, end, key, out total, start, length);

            return ToPage(list, total, start, length, draw);
        }
        #endregion

        #region 导出excel
        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="end"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public FileResult ToExcel(string code, DateTime? startTime, DateTime? end, string key)
        {
            int total = 0;
            var mid = string.Empty;
            if (!string.IsNullOrEmpty(code))
            {
                mid = DB.Member_Info.FindEntity(a => a.Code == code).MemberId;
            }
            var list = DB.Fin_Info.getDataSource(mid, startTime, end, key, out total, 0, int.MaxValue);
            return base.ToExcel(list);
        }
        #endregion
    }
}