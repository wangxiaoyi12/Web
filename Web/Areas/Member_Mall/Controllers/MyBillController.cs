using Business;
using System;
using System.Web.Mvc;

namespace Web.Areas.Member_Mall.Controllers
{
    public class MyBillController : Web.Controllers.MemberBaseController
    {
        // GET: Member_Mall/MyBill
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
        public string getDataSource(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;          
            var list = DB.Bill_Info.getDataSource(CurrentUser.Id, startTime, end, key, out total, start, length);

            return ToPage(list, total, start, length, draw);
        } 
    }
}