using Business;
using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class RegController : Controller
    {
        // GET: Reg
        public ActionResult Index(string id)
        {
            if (DB.XmlConfig.XmlSite.webstatus == "关闭" || DB.XmlConfig.XmlSite.webstatus == "维护")
            {
                return Redirect("/");
            }

            //判断是否是微信浏览
            //if (Url_Mobile.IsWechat())
            //{
            //    Member_Info model = DB.Member_Info.Where(q => q.Code == id).FirstOrDefault();
            //    if (model != null)
            //    {
            //        string url = Url_Mobile.GetQrCodeLink(model);
            //        return Redirect(url);
            //    }
            //}

            #region 获取相关数据，如会员级别，银行，省等
            ViewData["Level"] = DB.Sys_Level.Where().ToList();
            ViewData["Banks"] = DB.Sys_BasicData.getBasicDataByType("银行");
            ViewData["Prov"] = DB.Area_Province.ToList();
            #endregion

            #region 自动生成Code  HY+N倍随机数字
            //var code = "ZD" + new Random().Next(1000000, 9999999);
            //var exist = DB.Member_Info.Any(a => a.Code == code);
            //while (exist)
            //{
            //    code = "ZD" + new Random().Next(1000000, 9999999);
            //    exist = DB.Member_Info.Any(a => a.Code == code);
            //}
            #endregion 

            return View(new DataBase.Member_Info() {RecommendCode = id });
        }
        public PartialViewResult EveryDingShi()
        {
            TaskTime t = new TaskTime();
            //分红奖
            //t.FenHong();
            //突出贡献奖
            //t.TuChu();
            //发放奖金
            t.FaFang();
            return PartialView();
        }
        //public PartialViewResult EveryUpdateOrder()
        //{
        //    TaskTime t = new TaskTime();
        //    t.UpdateOrder();
        //    return PartialView();
        //}
        
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ActionResult Save(Member_Info entity)
        {
            entity.CreateMemberId = string.Empty;
            entity.CreateMemberName = string.Empty;
            entity.CreateTime = DateTime.Now;
            //entity.Pwd2 = "222222";
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
            var json = DB.Member_Info.Save(entity, Enums.LoginType.nologin,"","",0,0,0,0);
            if (json.Status == "y")
            {
                json.ReUrl = "/";   //注册成功就跳转到 激活页
            }
            return Json(json);
        }
    }
}