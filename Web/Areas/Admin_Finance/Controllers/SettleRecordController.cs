using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DataBase;
using Business;
namespace Web.Areas.Admin_Finance.Controllers
{
    /// <summary>
    /// 结算记录
    /// </summary>
    public class SettleRecordController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_Finance/SettleRecord
        public ActionResult Index()
        {
            return View();
        }

        #region 查询
        public string getDataSource(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.SettleRecord.getDataSource(startTime, end, out total, start, length);

            return ToPage(list, total, start, length, draw);
        }
        #endregion


        ///// <summary>
        ///// 师长分红测试按钮
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult FH()
        //{
        //    try
        //    {
        //        DB.Jiang.ShiZhangFenHong();
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(ex.Message);
        //    }
        //}

        //public ActionResult Week()
        //{
        //    try
        //    {
        //        DB.SettleRecord.Send(Enums.SettleType.周结算.GetHashCode());
        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(ex.Message + "<br />" + ex.StackTrace);
        //    }
        //}

        public ActionResult Month()
        {
            try
            {
                DB.SettleRecord.Send(Enums.SettleType.月计算.GetHashCode());
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return Content(ex.Message + "<br />" + ex.StackTrace);
            }
        }
    }
}