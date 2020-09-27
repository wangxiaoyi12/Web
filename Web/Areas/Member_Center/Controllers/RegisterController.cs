using System;
using System.Web.Mvc;
using Business;
using DataBase;
using System.Linq;

namespace Web.Areas.Member_Center.Controllers
{
    /// <summary>
    /// 会员注册
    /// </summary>
    public class RegisterController : Web.Controllers.MemberBaseController
    {
        //   /Member_Center/Register
        /// <summary>
        /// 注册会员
        /// </summary>    
        /// <param name="position">来自网络图的注册位置</param>
        /// <returns></returns>
        public ActionResult Detail(string position, string upid)
        {
            #region 获取相关数据，如会员级别，银行，省等
            ViewData["Level"] = DB.Sys_Level.Where().ToList();
            ViewData["Banks"] = DB.Sys_BasicData.getBasicDataByType("银行");
            ViewData["Prov"] = DB.Area_Province.ToList();
            #endregion

            #region 自动生成Code  HY+N倍随机数字
            //var code = "HY" + new Random().Next(1000000, 9999999);
            //var exist = DB.Member_Info.Any(a => a.Code == code);
            //while (exist)
            //{
            //    code = "HY" + new Random().Next(1000000, 9999999);
            //    exist = DB.Member_Info.Any(a => a.Code == code);
            //}
            #endregion 

            return View(new DataBase.Member_Info() {RecommendCode = CurrentUser.LoginName });
        }

        #region 保存
        public ActionResult Save(Member_Info entity)
        {
            entity.CreateMemberId = CurrentUser.Id;
            entity.CreateMemberName = CurrentUser.Name;
            entity.CreateTime = DateTime.Now;
            //赋值省市区
            if (entity.ProvId != null)
            {
                entity.ProvName = DB.Area_Province.FindEntity(entity.ProvId).name;
            }
            if (entity.CityId != null)
            {
                entity.CityName = DB.Area_City.FindEntity(entity.CityId).name;
            }
            if (entity.CountyId != null)
            {
                entity.CountyName = DB.Area_County.FindEntity(entity.CountyId).name;
            }
            entity.Code = entity.Code.Trim();
            var json = DB.Member_Info.Save(entity, Enums.LoginType.member,"","",0,0,0,0);
            if (json.Status == "y")
            {
                json.ReUrl = "/SysManage/Desk/Index";   //注册成功就跳转到 激活页
            }
            return Json(json);
        }
        #endregion

    }
}