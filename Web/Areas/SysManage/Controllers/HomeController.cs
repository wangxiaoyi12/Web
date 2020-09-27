using System;
using System.Web.Mvc;
using Business;
using System.Linq;
using EntityFramework.Extensions;
using DataBase;

namespace Web.Areas.SysManage.Controllers
{
    public class HomeController : Web.Controllers.AdminBaseController
    {
        // GET: SysManage/Home
        public ActionResult Index()
        {
            var start_day = DateTime.Now.Date;
            var end_day = start_day.AddDays(1);
            var start_week = getMonday();
            var end_week = getSunday();
            var start_month = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var end_month = start_month.AddMonths(1);
            var start_year = new DateTime(DateTime.Now.Year, 1, 1);
            var end_year = start_year.AddYears(1);



            #region 减少数据库查询次数
            using (var db = new DbMallEntities())
            {
                var r = db.Sys_Level.Where(a => true).Select(x => new
                {
                    addMember_dd = db.Member_Info.Count(a => a.CreateTime >= start_day && a.CreateTime < end_day),
                    addMember_week = db.Member_Info.Count(a => a.CreateTime >= start_week && a.CreateTime < end_week),
                    addMember_mm = db.Member_Info.Count(a => a.CreateTime >= start_month && a.CreateTime < end_month),
                    addMember_yy = db.Member_Info.Count(a => a.CreateTime >= start_year && a.CreateTime < end_year),

                    income_dd = db.ShopOrders.Where(a => a.PayTime >= start_day && a.OrderType!= "积分优惠价" && a.PayTime < end_day).Sum(a => (decimal?)a.RealAmount) ?? 0,
                    income_week = db.ShopOrders.Where(a => a.PayTime >= start_week && a.OrderType != "积分优惠价" && a.PayTime < end_day).Sum(a => (decimal?)a.RealAmount) ?? 0,
                    income_mm = db.ShopOrders.Where(a => a.PayTime >= start_month && a.OrderType != "积分优惠价" && a.PayTime < end_month).Sum(a => (decimal?)a.RealAmount) ?? 0,
                    income_yy = db.ShopOrders.Where(a => a.PayTime >= start_year && a.OrderType != "积分优惠价" && a.PayTime < end_year).Sum(a => (decimal?)a.RealAmount) ?? 0,

                    income_dds = db.ShopOrders.Where(a => a.PayTime >= start_day && a.OrderType == "积分优惠价" && a.PayTime < end_day).Sum(a => (decimal?)a.RealAmount) ?? 0,
                    income_weeks = db.ShopOrders.Where(a => a.PayTime >= start_week && a.OrderType == "积分优惠价" && a.PayTime < end_day).Sum(a => (decimal?)a.RealAmount) ?? 0,
                    income_mms = db.ShopOrders.Where(a => a.PayTime >= start_month && a.OrderType == "积分优惠价" && a.PayTime < end_month).Sum(a => (decimal?)a.RealAmount) ?? 0,
                    income_yys = db.ShopOrders.Where(a => a.PayTime >= start_year && a.OrderType == "积分优惠价" && a.PayTime < end_year).Sum(a => (decimal?)a.RealAmount) ?? 0,

                    //income_dd = db.ShopOrders.Where(a => a.PayTime >= start_day && a.PayTime < end_day).Sum(a => a.RealAmount),
                    //income_week = db.ShopOrders.Where(a => a.PayTime >= start_week && a.PayTime < end_day).Sum(a => a.RealAmount),
                    //income_mm = db.ShopOrders.Where(a => a.PayTime >= start_month && a.PayTime < end_month).Sum(a => a.RealAmount),
                    //income_yy = db.ShopOrders.Where(a => a.PayTime >= start_year && a.PayTime < end_year).Sum(a => a.RealAmount),

                    expend_dd = db.Fin_Info.Where(a => a.CreateTime >= start_day && a.CreateTime < end_day).Sum(a => a.RealAmount),
                    expend_week = db.Fin_Info.Where(a => a.CreateTime >= start_week && a.CreateTime < end_week).Sum(a => a.RealAmount),
                    expend_mm = db.Fin_Info.Where(a => a.CreateTime >= start_month && a.CreateTime < end_month).Sum(a => a.RealAmount),
                    expend_yy = db.Fin_Info.Where(a => a.CreateTime >= start_year && a.CreateTime < end_year).Sum(a => a.RealAmount),

                    Active_y = db.Member_Info.Count(p => p.IsActive == "已激活"),
                    Active_n = db.Member_Info.Count(p => p.IsActive == "未激活"),
                    Drawcount = db.Fin_Draw.Count(p => p.DrawState == "未发放"),
                    Remitcount = db.Fin_Remit.Count(p => p.RemitState == "申请中")
                }).FirstOrDefault();
                ViewBag.addMember_dd = r.addMember_dd;
                ViewBag.addMember_week = r.addMember_week;
                ViewBag.addMember_mm = r.addMember_mm;
                ViewBag.addMember_yy = r.addMember_yy;

                ViewBag.income_dd = r.income_dd;
                ViewBag.income_week = r.income_week;
                ViewBag.income_mm = r.income_mm;
                ViewBag.income_yy = r.income_yy;

                ViewBag.income_dds = r.income_dds;
                ViewBag.income_weeks = r.income_weeks;
                ViewBag.income_mms = r.income_mms;
                ViewBag.income_yys = r.income_yys;

                ViewBag.expend_dd = r.expend_dd ?? 0;
                ViewBag.expend_week = r.expend_week ?? 0;
                ViewBag.expend_mm = r.expend_mm ?? 0;
                ViewBag.expend_yy = r.expend_yy ?? 0;

                ViewBag.Active_y = r.Active_y;
                ViewBag.Active_n = r.Active_n;
                ViewBag.Drawcount = r.Drawcount;
                ViewBag.Remitcount = r.Remitcount;

                ViewBag.OutIn_dd = getOutIn(Convert.ToDecimal(ViewBag.income_dd), Convert.ToDecimal(ViewBag.expend_dd));
                ViewBag.OutIn_week = getOutIn(Convert.ToDecimal(ViewBag.income_week), Convert.ToDecimal(ViewBag.expend_week));
                ViewBag.OutIn_mm = getOutIn(Convert.ToDecimal(ViewBag.income_mm), Convert.ToDecimal(ViewBag.expend_mm));
                ViewBag.OutIn_yy = getOutIn(Convert.ToDecimal(ViewBag.income_yy), Convert.ToDecimal(ViewBag.expend_yy));
            }
            #endregion

            DB.XmlConfig.AutoSetUrl(Request);

            ViewBag.LingShouQuZongJin = DB.ShopOrder.Where(p => p.PayTime >= start_day && p.PayTime < end_day && p.OrderType == "零售区").Sum(p => (decimal?)p.RealAmount) ?? 0;
            ViewBag.LingShouQuCount = DB.ShopOrder.Where(p => p.PayTime >= start_day && p.PayTime < end_day && p.OrderType == "零售区").Sum(p => (decimal?)p.Count) ?? 0;
            ViewBag.QiangGou = DB.ShopOrder.Where(p => p.PayTime >= start_day && p.PayTime < end_day && p.Type == "挂卖中" && p.OrderType=="批发区").Count(); ;
            return View();
        }
        //获取周一  
        private DateTime getMonday()
        {
            DateTime now = DateTime.Now;
            DateTime temp = new DateTime(now.Year, now.Month, now.Day);
            int count = now.DayOfWeek - DayOfWeek.Monday;
            if (count == -1) count = 6;

            return temp.AddDays(-count);
        }//获取周天  
        private DateTime getSunday()
        {
            DateTime now = DateTime.Now;
            DateTime temp = new DateTime(now.Year, now.Month, now.Day);
            int count = now.DayOfWeek - DayOfWeek.Sunday;
            if (count != 0) count = 7 - count;

            return temp.AddDays(count);
        }



        #region 获取本月业绩图
        public string PerformanceToBar()
        {
            return DB.Member_Info.getActiveMemberLine();
        }
        #endregion 

        #region 会员分布地图
        public string getMemberDistribution()
        {
            return DB.Member_Info.getMemberDistribution();
        }
        #endregion

        #region 辅助方法
        protected string getOutIn(decimal inAmount, decimal outAmount)
        {
            decimal result = 0;
            if (inAmount != 0)
            {
                result = ((outAmount / inAmount) * 100);
            }
            else if (outAmount != 0)
            {
                result = 100;
            }

            return result.ToString("0.##");
        }
        #endregion
    }
}