using System.Web.Mvc;
using Business;
using System;
using System.Linq;

namespace Web.Areas.Admin_Member.Controllers
{
    /// <summary>
    /// 推荐关系
    /// </summary>
    public class RecommendController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_Member/Recommend
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Index_Net()
        {
            var membercode = Request.QueryString["membercode"];
            var expandLevel = Request.QueryString["expandLevel"];
            if (string.IsNullOrEmpty(membercode))
            {
                membercode = "admin";
            }
            if (string.IsNullOrEmpty(expandLevel))
            {
                expandLevel = "1";
            }
            ViewBag.MemberCode = membercode;
            ViewBag.expandLevel = expandLevel;
            return View();
        }
        /// <summary>
        /// 加载菜单树
        /// </summary>
        /// <returns></returns>
        public string getjsTreeData(string code)
        {
            if (string.IsNullOrEmpty(code))
                code = "admin";
            var m = DB.Member_Info.FindEntity(a => a.Code == code);
            if (m == null)
                return "0";
            return DB.Member_Info.RecommendTreeData(m.MemberId);
        }
        public JsonResult SearchNet(string memberCode)
        {
            if (string.IsNullOrEmpty(memberCode))
            {
                memberCode = CurrentUser.LoginName;
            }
            var currentMember = DB.Member_Info.Where(a => a.Code == "admin").FirstOrDefault();
            return Json(DB.Member_Info.treeTableSearchNet(memberCode, currentMember, true));
        }
        public JsonResult SearchNodeNet(string memberCode)
        {
            if (string.IsNullOrEmpty(memberCode))
            {
                memberCode = CurrentUser.LoginName;
            }
            var currentMember = DB.Member_Info.Where(a => a.Code == "admin").FirstOrDefault();
            return Json(DB.Member_Info.treeTableSearchNodeNet(memberCode, currentMember));
        }
        public JsonResult GetChildsNodeNet(string id)
        {
            return Json(DB.Member_Info.treeTableGetChildsNodeNet(id));
        }
    }
}