using System;
using System.Web.Mvc;
using Business;

namespace Web.Areas.Member_Statistics.Controllers
{
    /// <summary>
    /// 会员收益明细
    /// </summary>
    public class MemberBonusController : Web.Controllers.MemberBaseController
    {
        // GET: Member_Statistics/MemberBonus
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Scores()
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
        public string getDataSource(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.Fin_Info.getDataSource(CurrentUser.Id, startTime, end, key, out total, start, length);
            return ToPage(list, total, start, length, draw);
        }
        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <param name="start">开始日期</param>
        /// <param name="end">结束日期</param>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        public string getDataSourceScores(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.SysLogs.getList(CurrentUser.Id, startTime, end, key, out total, start, length);
            return ToPage(list, total, start, length, draw);
        }
    }
}