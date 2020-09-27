using System;
using System.Web.Mvc;
using Business;
using DataBase;

namespace Web.Areas.Member_Finance.Controllers
{
    /// <summary>
    /// 收益转换
    /// </summary>
    public class ConvertController : Web.Controllers.MemberBaseController
    {
        // GET: Member_Finance/Convert
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail(string id, string commentForm)
        {
            var m = DB.Member_Info.FindEntity(CurrentUser.Id);
            var model = new DataBase.Fin_Convert()
            {
                MemberCode = m.Code,
                MemberId = m.MemberId,
                NickName = m.NickName
            };
            ViewBag.Commission = m.Commission;
            ViewBag.Coins = m.Coins;
            if (!string.IsNullOrEmpty(id))
            {
                model = DB.Fin_Convert.FindEntity(Convert.ToInt32(id));
            }
            return View(model);
        }

        #region 查询
        public string getDataSource(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.Fin_Convert.getDataSource(CurrentUser.Id, startTime, end, key, out total, start, length);
            return ToPage(list, total, start, length, draw);
        }
        #endregion

        #region 保存
        public ActionResult Save(Fin_Convert DataBase)
        {
            var Pwd2 = Common.CryptHelper.DESCrypt.Encrypt(Request["Pwd2"]);

            DataBase.CreateTime = DateTime.Now;
            var r = DB.Fin_Convert.Save(Pwd2, DataBase);
            //if (r.IsSuccess)
            //{
            //    r.ReUrl = ControllerPath + "/Index";
            //}
            return Json(r);
        }
        #endregion
    }
}