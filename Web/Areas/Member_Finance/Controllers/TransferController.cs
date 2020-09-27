using System;
using System.Web.Mvc;
using Business;
using DataBase;

namespace Web.Areas.Member_Finance.Controllers
{
    /// <summary>
    /// 会员转账
    /// </summary>
    public class TransferController : Web.Controllers.MemberBaseController
    {
        // GET: Member_Finance/Transfer
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Detail(string id, string commentForm)
        {
            var m = DB.Member_Info.FindEntity(CurrentUser.Id);
            var model = new DataBase.Fin_Transfer()
            {
                FromMemberId = m.MemberId,
                FromMemberCode = m.Code,
                FromNickName = m.NickName
            };
            ViewBag.Commission = m.Commission;
            ViewBag.Coins = m.Coins;
            if (!string.IsNullOrEmpty(id))
            {
                model = DB.Fin_Transfer.FindEntity(Convert.ToInt32(id));
            }
            return View(model);
        }

        #region 查询
        public string getDataSource(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.Fin_Transfer.getDataSource(CurrentUser.Id, startTime, end, key, out total, start, length);
            return ToPage(list, total, start, length, draw);
        }
        #endregion

        #region 保存
        public ActionResult Save(Fin_Transfer DataBase)
        {
            var Pwd2 = Common.CryptHelper.DESCrypt.Encrypt(Request["Pwd2"]);

            DataBase.CreateTime = DateTime.Now;
            var result = DB.Fin_Transfer.Save(Pwd2, DataBase);
            if (result.IsSuccess)
            {
                result.ReUrl = ControllerPath + "/Index";
            }
            return Json(result);
        }
        #endregion 
    }
}