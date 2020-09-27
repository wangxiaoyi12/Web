using System;
using System.Web.Mvc;
using Business;
using System.Linq;

namespace Web.Areas.Member_Center.Controllers
{
    /// <summary>
    /// 店主下的会员列表
    /// </summary>
    public class ServiceController : Web.Controllers.MemberBaseController
    {
        // GET: Member_Center/Service        
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Detail(string id, string commentForm)
        {
            return RedirectToAction("Detail", "Register", new { id = id, commentForm = commentForm });
        }

        #region 查询
        public string getDataSource(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.Member_Info.getSearchList(CurrentUser.Id, startTime, end, key, null, out total, start, length)
                  .Select(a => new
                  {
                      a.ActiveAmount,
                      a.Code,
                      a.MemberId,
                      a.Mobile,
                      a.NickName,
                      a.CreateTime,
                      a.IsActive,
                      a.MemberLevelName
                  }).ToList();
            return ToPage(list, total, start, length, draw);
        }
        #endregion

    }
}