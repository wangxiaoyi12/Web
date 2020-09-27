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
    public class LiuShuiController : Web.Controllers.MemberBaseController
    {
        // GET: Member_Finance/Draw
        public ActionResult Index()
        {
            return View();
        }

        #region 查询
        public string getDataSource(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.Fin_LiuShui.getDataSourceQian(CurrentUser.Id, startTime, end, key, out total, start, length);
            return ToPage(list, total, start, length, draw);
        }
        #endregion
    }
}