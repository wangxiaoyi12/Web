using System;
using System.Web.Mvc;
using Business;
using System.Linq;

namespace Web.Areas.Admin_Member.Controllers
{
    public class ActiveMemberController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_Member/ActiveMember
        public ActionResult Index()
        {
            return View();
        }

        #region 查询
        public string getDataSource(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.Member_Info.getSearchList(null, startTime, end, key, "未激活", out total, start, length)
                .Select(a => new
                {
                    a.ActiveAmount,
                    a.Code,
                    a.MemberId,
                    a.Mobile,
                    a.NickName,
                    a.CreateTime,
                    a.MemberLevelName,
                    a.RecommendCode,
                    a.RecommendName,
                    a.UpperCode
                }).ToList();

            return ToPage(list, total, start, length, draw);
        }
        #endregion

        #region 激活会员
        public JsonResult ToActive(string id, string isNullActive)
        {
            var r = DB.Member_Info.ActiveMember(id, CurrentUser, Convert.ToBoolean(isNullActive), true);
            return Json(r);
        }
        #endregion

        #region 删除会员
        public JsonResult Delete(string idList)
        {
            var r = DB.Member_Info.Delete(idList, Enums.LoginType.admin);
            return Json(r);
        }
        #endregion

        #region 导出excel
        public FileResult ToExcel(DateTime? startTime, DateTime? end, string key)
        {
            int total = 0;
            var list = DB.Member_Info.getSearchList(null, startTime, end, key, "未激活", out total, 0, int.MaxValue).ToList();
            return base.ToExcel(list);
        }
        #endregion 
    }
}