using System;
using System.Web.Mvc;
using Web.Controllers;
using Common;
using Business;
using System.Linq;

namespace Web.Areas.Member_Center.Controllers
{
    /// <summary>
    /// 未激活会员列表
    /// </summary>
    public class ActiveController : MemberBaseController
    {
        // GET: Member_Center/Active
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
            var list = DB.Member_Info.getSearchList(CurrentUser.Id, startTime, end, key, "未激活", out total, start, length)
                 .Select(a => new
                 {
                     a.IsActive,
                     a.ActiveAmount,
                     a.Code,
                     a.MemberId,
                     a.Mobile,
                     a.NickName,
                     a.CreateTime,
                     a.MemberLevelName,
                     a.RecommendCode,
                     a.UpperCode
                 }).ToList();

            return ToPage(list, total, start, length, draw);
        }
        #endregion

        #region 激活会员
        public JsonResult ToActive(string id)
        {
            JsonHelp json = new JsonHelp(false, "当前不是店主，不能激活会员");
            var member = DB.Member_Info.FindEntity(CurrentUser.Id);
            if (member != null)
            {
                if (member.IsServiceCenter == "是")
                {
                    json = DB.Member_Info.ActiveMember(id, CurrentUser, false);
                }
            }
            return Json(json);
        }
        #endregion

        #region 删除未激活的会员
        public JsonResult Delete(string idList)
        {
            var r = DB.Member_Info.Delete(idList, Enums.LoginType.member);
            return Json(r);
        }
        #endregion
    }
}