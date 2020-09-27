using System.Web.Mvc;
using Business;
using DataBase;
using System;

namespace Web.Areas.Member_Member.Controllers
{
    /// <summary>
    /// 个人信息管理
    /// </summary>
    [Web.Controllers.Permissions]
    public class MemberController : Web.Controllers.MemberBaseController
    {
        // GET: Member_Member/Member
        public ActionResult Index()
        {
            ViewData["Banks"] = DB.Sys_BasicData.getBasicDataByType("银行");
            ViewData["Prov"] = DB.Area_Province.ToList();

            var model = DB.Member_Info.FindEntity(CurrentUser.Id);
            ViewData["City"] = DB.Area_City.ToList(a => a.pid == model.ProvId);
            ViewData["County"] = DB.Area_County.ToList(a => a.cid == model.CityId);

            if (model != null)
            {
                ViewBag.RecommendId = model.RecommendCode;
                //推荐人数
                ViewBag.RCount = DB.Member_Info.Count(a => a.RecommendId == CurrentUser.Id);
            }
            return View(model);
        }

        #region 保存
        public ActionResult Save(Member_Info DataBase)
        {
            //赋值省市区
            if (DataBase.ProvId != null)
            {
                DataBase.ProvName = DB.Area_Province.FindEntity(DataBase.ProvId).name;
            }
            if (DataBase.CityId != null)
            {
                DataBase.CityName = DB.Area_City.FindEntity(DataBase.CityId).name;
            }
            if (DataBase.CountyId != null)
            {
                DataBase.CountyName = DB.Area_County.FindEntity(DataBase.CountyId).name;
            }
            var r = DB.Member_Info.Save(DataBase, Enums.LoginType.member,"","",0,0,0,0);
            if (r.IsSuccess)
            {
                r.ReUrl = ControllerPath + "/Index";
                Tools.WriteCookie(DB.Member_Info.FindEntity(DataBase.MemberId));
            }
            return Json(r);
        }
        #endregion 

    }
}