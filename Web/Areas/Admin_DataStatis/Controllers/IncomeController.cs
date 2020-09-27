using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using Common;
using DataBase;
using Business;
namespace Web.Areas.Admin_DataStatis.Controllers
{
    public class IncomeController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_DataStatis/Income
        public ActionResult Index()
        {
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
        public string getDataSource(DateTime? startTime, DateTime? end, int start, int length, int draw)
        {
            var total = 0;
            //获取时间分页
            List<DateTime> dateList = new List<DateTime>();
            if (end == null)
                end = DateTime.Now.Date;
            if (startTime == null)
                startTime = end.Value.AddDays(-7);
            for (int i = 0; i <= (end - startTime).Value.TotalDays; i++)
            {
                dateList.Add(startTime.Value.AddDays(i));
            }
            total = dateList.Count;
            List<DateTime> datePage = dateList.OrderBy(q => q).Skip(start).Take(length).ToList();

            //获取时间内的总收入和人数
            List<IncomeItem> list = new List<IncomeItem>();
            foreach (var item in datePage)
            {
                DateTime tempStart = item.Date;
                DateTime tempEnd = tempStart.AddDays(1);

                IncomeItem income = new IncomeItem()
                {
                    Date = tempStart.ToString("yyyy-MM-dd"),
                    AllCount = DB.Member_Info.Where(q => q.CreateTime >= tempStart && q.CreateTime < tempEnd).Count(),
                    AllAmount = DB.Member_Info.Where(q => q.CreateTime >= tempStart && q.CreateTime < tempEnd)
                    .Select(q => q.ActiveAmount).Sum() ?? 0
                };
                list.Add(income);
            }

            return ToPage(list, total, start, length, draw);
        }

        public class IncomeItem
        {
           // public int ID { get; set; }
            public string Date { get; set; }

            public int AllCount { get; set; }
            public decimal AllAmount { get; set; }
        }
        #endregion
    }
}