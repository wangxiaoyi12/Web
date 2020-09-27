using System;
using System.Web.Mvc;
using Business;
using DataBase;

namespace Web.Areas.Member_Finance.Controllers
{
    /// <summary>
    /// 汇款申请
    /// </summary>
    public class RemitController : Web.Controllers.MemberBaseController
    {
        // GET: Member_Finance/Remit
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Detail(string id, string commentForm)
        {
            ViewData["Banks"] = DB.Sys_BasicData.getBasicDataByType("银行");
            var m = DB.Member_Info.FindEntity(CurrentUser.Id);
            var model = new DataBase.Fin_Remit()
            {
                MemberCode = m.Code,
                MemberId = m.MemberId,
                NickName = m.NickName,
                ServiceCenterCode = m.ServiceCenterCode,
                ServiceCenterId = m.ServiceCenterId,
                BankCode = m.BankCode,
                BankAccount = m.BankAccount,
                BankName = m.BankName
            };
            if (!string.IsNullOrEmpty(id))
            {
                model = DB.Fin_Remit.FindEntity(Convert.ToInt32(id));
            }
            ViewBag.img = DB.XmlConfig.XmlSite.img;
            ViewBag.ZhiFuB = DB.XmlConfig.XmlSite.imgZhi;
            ViewBag.ZhiFuBName = DB.XmlConfig.XmlSite.ZhiFuBName;
            ViewBag.YinHang = DB.XmlConfig.XmlSite.YinHang;
            ViewBag.YinHangName = DB.XmlConfig.XmlSite.YinHangName;
            return View(model);
        }

        #region 查询
        public string getDataSource(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.Fin_Remit.getDataSource(CurrentUser.Id, startTime, end, key, out total, start, length);
            return ToPage(list, total, start, length, draw);
        }
        #endregion

        #region 保存
        public ActionResult Save(Fin_Remit entity)
        {
            entity.RemitState = "申请中";
            entity.CreateTime = DateTime.Now;
            var r = DB.Fin_Remit.Save(entity);
            if (r.IsSuccess)
            {
                r.ReUrl = ControllerPath + "/Index";
            }
            return Json(r);
        }
        #endregion 
    }
}