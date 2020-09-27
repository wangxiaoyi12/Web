using System.Web.Mvc;
using Business;
using System;

namespace Web.Areas.Member_Market.Controllers
{
    /// <summary>
    /// 推荐结构
    /// </summary>
    public class RecommendController : Web.Controllers.MemberBaseController
    {
        // GET: Member_Market/Recommend
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
                membercode = CurrentUser.LoginName;
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
        public string getjsTreeData()
        {
            return DB.Member_Info.RecommendTreeData(CurrentUser.Id);
        }
     
        public JsonResult SearchNet(string memberCode)
        {
            if (string.IsNullOrEmpty(memberCode))
            {
                memberCode = CurrentUser.LoginName;
            }
            var currentMember = DB.Member_Info.FindEntity(CurrentUser.Id);
            return Json(DB.Member_Info.treeTableSearchNet(memberCode, currentMember, false));
        }
        public JsonResult SearchNodeNet(string memberCode)
        {
            if (string.IsNullOrEmpty(memberCode))
            {
                memberCode = CurrentUser.LoginName;
            }
            var currentMember = DB.Member_Info.FindEntity(CurrentUser.Id);
            return Json(DB.Member_Info.treeTableSearchNodeNet(memberCode, currentMember));
        }
        public JsonResult GetChildsNodeNet(string id)
        {
            return Json(DB.Member_Info.treeTableGetChildsNodeNet(id));
        }
    }
}