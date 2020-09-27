using System;
using System.Web.Mvc;
using Business;
using System.Linq;

namespace Web.Areas.Member_Center.Controllers
{
    /// <summary>
    /// 已激活的会员列表
    /// </summary>
    public class ActivedController : Web.Controllers.MemberBaseController
    {

        // GET: Member_Center/Actived
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
            var list = DB.Member_Info.getSearchList(CurrentUser.Id, startTime, end, key, "已激活", out total, start, length).ToList()
                  .Select(a => new
                  {
                      a.IsActive,
                      a.ActiveAmount,
                      a.Code,
                      a.MemberId,
                      a.Mobile,
                      a.NickName,
                      a.ActiveTime,
                      a.RecommendName,
                      a.UpperName,
                      a.MemberLevelName
                  }).ToList();
            return ToPage(list, total, start, length, draw);
        }
        #endregion

    }
}