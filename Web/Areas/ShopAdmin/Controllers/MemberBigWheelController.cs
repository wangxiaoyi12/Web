using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.ShopAdmin.Controllers
{
    public class MemberBigWheelController : Web.Controllers.MemberBaseController
    {
        // GET: ShopAdmin/MemberBigWheel
        public ActionResult Index()
        {
            return View();
        }
        #region 查询
        public string getDataSource(string key, int start, int length, int draw)
        {
            var query = DB.ShopBigWheelLog.Where(a => a.MemberID == CurrentUser.Id
                  && (string.IsNullOrEmpty(key) ? true : (a.MemberCode.Contains(key) || a.NickName.Contains(key) || a.Result.Contains(key))))
                   .Select(a => new
                   {
                       a.ShopBigWheel.Title,
                       a.ID,
                       a.CreateTime,
                       a.Desc,
                       a.MemberCode,
                       a.NickName,
                       a.Remark,
                       a.Result,
                       a.ResultID
                   });
            var total = query.Count();
            var list = query.OrderByDescending(a => a.CreateTime).Skip(start).Take(length).ToList();

            return ToPage(list, total, start, length, draw);
        }
        #endregion
    }
}