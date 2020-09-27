using System;
using System.Web.Mvc;
using Business;

namespace Web.Areas.Admin_DataStatis.Controllers
{
    public class BonusTotalController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_DataStatis/BonusTotal
        #region 根据日期统计的收益总表
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <param name="start">开始日期</param>
        /// <param name="end">结束日期</param>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        public string getDataSource(DateTime? startTime, DateTime? end, int start, int length, int draw)
        {
            int total = 0;
            var list = DB.Fin_Info.getSummaryTotalDataSource(startTime, end, out total, start, length);
            return ToPage(list, total, start, length, draw);
        }

        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="end"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public FileResult ToExcel(DateTime? startTime, DateTime? end, string key)
        {
            int total = 0;
            var list = DB.Fin_Info.getSummaryTotalDataSource(startTime, end, out total, 0, int.MaxValue);
            //return base.ToExcel(list);
            return null;
        }
        #endregion

        #region 根据会员统计的收益总表
        public ActionResult Member()
        {
            return View();
        }
               
        public string getMemberDataSource(int start, int length, int draw)
        {
            int total = 0;
            var list = DB.Fin_Info.getSummaryTotalDataSourceByCode(out total, start, length);
            return ToPage(list, total, start, length, draw);
        } 

        public FileResult ToExcelMember()
        {
            int total = 0;
            var list = DB.Fin_Info.getSummaryTotalDataSourceByCode(out total, 0, int.MaxValue);
            //return base.ToExcel(list);
            return null;
        }     
        #endregion 
    }
}