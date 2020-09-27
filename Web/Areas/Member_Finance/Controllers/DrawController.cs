using System;
using System.Web.Mvc;
using Business;
using DataBase;
using System.Web.Helpers;

namespace Web.Areas.Member_Finance.Controllers
{
    /// <summary>
    /// 提现申请
    /// </summary>
    public class DrawController : Web.Controllers.MemberBaseController
    {
        // GET: Member_Finance/Draw
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail(string id, string commentForm)
        {
            var m = DB.Member_Info.FindEntity(CurrentUser.Id);
            var model = new Fin_Draw()
            {
                MemberCode = m.Code,
                MemberId = m.MemberId,
                NickName = m.NickName,
                ServiceCenterCode = m.ServiceCenterCode,
                ServiceCenterId = m.ServiceCenterId,
                BankAccount = m.BankAccount,
                BankCode = m.BankCode,
                BankAddress = m.BankAddress,
                BankName = m.BankName,
                OpenBank = m.OpenBank
            };
            ViewBag.Commission = m.Commission;
            if (!string.IsNullOrEmpty(id))
            {
                model = DB.Fin_Draw.FindEntity(Convert.ToInt32(id));
            }
            return View(model);
        }

        #region 查询
        public string getDataSource(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.Fin_Draw.getDataSource(CurrentUser.Id, startTime, end, key, out total, start, length);
            return ToPage(list, total, start, length, draw);
        }
        #endregion

        #region 保存
        public ActionResult Save(Fin_Draw DataBase)
        {
            JsonHelp json = new JsonHelp() { };
            var Pwd2 = Common.CryptHelper.DESCrypt.Encrypt(Request["Pwd2"]);
            int weeknow = Convert.ToInt32(DateTime.Today.DayOfWeek);
            string tixian = DB.XmlConfig.XmlSite.DrawName;
            if (tixian.IndexOf(weeknow.ToString()) == -1)
            {
                json.Msg = "暂未到提现时间"; ;

                json.IsSuccess = false;
            }
            else
            {
                DataBase.DrawState = "未发放";
                DataBase.CreateTime = DateTime.Now;

                var member = DB.Member_Info.FindEntity(CurrentUser.Id);
                DataBase.BankAccount = string.IsNullOrEmpty(member.BankAccount) ? member.BankCode : member.BankAccount;
                DataBase.BankAddress = member.BankAddress;
                DataBase.BankCode = member.BankCode;
                DataBase.BankName = member.BankName;
                DataBase.OpenBank = member.OpenBank;
                DB.Fin_Draw.Save(Pwd2, DataBase);
                if (json.IsSuccess)
                {
                    json.ReUrl = ControllerPath + "/Index";
                }
            }
            return Json(json);
        }
        #endregion 
    }
}