using System;
using System.Web.Mvc;
using Business;
using System.Linq;

namespace Web.Areas.Admin_Member.Controllers
{
    /// <summary>
    /// 店主
    /// </summary>
    public class ServiceCenterController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_Member/ServiceCenter
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DLIndex()
        {
            return View();
        }
        #region 查询
        public string getDataSource(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.Member_Info.getServiceList(startTime, end, key, out total, start, length)
                  .Select(a => new
                  {
                      a.IsServiceCenter,
                      a.ActiveAmount,
                      a.Code,
                      a.MemberId,
                      a.Mobile,
                      a.NickName,
                      a.CreateTime,
                      // a.FenHong,
                      // a.YongJin,
                      a.AppellationLeveName,
                      a.MemberLevelName,
                      a.JingDu,
                      a.WeiDu,
                      a.DiZhi,
                      a.ShopName
                  }).ToList();
            return ToPage(list, total, start, length, draw);
        }

        public string getDataSourceDL(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.Member_Info.getServiceListDL(startTime, end, key, out total, start, length)
                  .Select(a => new
                  {
                      a.IsServiceCenter,
                      a.ActiveAmount,
                      a.Code,
                      a.MemberId,
                      a.DLName,
                      a.IsDL,
                      a.Sheng,
                      a.Shi,
                      a.Xian,
                      a.Mobile,
                      a.NickName,
                      a.CreateTime,
                      // a.FenHong,
                      // a.YongJin,
                      a.AppellationLeveName,
                      a.MemberLevelName
                  }).ToList();
            return ToPage(list, total, start, length, draw);
        }

        #endregion

        #region 增减电子币、收益
        public JsonResult SaveData(string id, decimal? Commission, decimal? Coins)
        {
            Commission = Commission ?? 0;
            Coins = Coins ?? 0;
            var json = DB.Member_Info.ModifyCommissionCoins(id, Commission.Value, Coins.Value);
            return Json(json);
        }
        #endregion

        #region 取消店主
        public JsonResult CancelCenter(string id)
        {
            var json = DB.Member_Info.CancelCenter(id);
            return Json(json);
        }
        public JsonResult CancelCenterDL(string id)
        {
            var json = DB.Member_Info.CancelCenterDL(id);
            return Json(json);
        }
        #endregion

        #region 导出excel
        public FileResult ToExcel(DateTime? startTime, DateTime? end, string key)
        {
            int total = 0;
            var list = DB.Member_Info.getServiceList(startTime, end, key, out total, 0, int.MaxValue).ToList();
            return base.ToExcel(list);
        }
        #endregion

        #region 审核 提交申请的店主 -> 通过/驳回
        public ActionResult Isok(string id, string type)
        {
            return Json(DB.Member_Info.IsokServerCenter(id, type));
        }
        public ActionResult IsokDL(string id, string type)
        {
            return Json(DB.Member_Info.IsokDL(id, type));
        }
        #endregion 
    }
}