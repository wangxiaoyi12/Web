using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataBase;
using Business;
using Common;
using Common.CryptHelper;

namespace Web.Areas.Shop.Controllers
{
    /// <summary>
    /// 商城用户登录，注册模块
    /// </summary>
    public class LogInController : ShopBaseController
    {
        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.webstatus = DB.XmlConfig.XmlSite.webstatus;
            return View();
        }
        /// <summary>
        /// 注册页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {

            return View();
        }

        /// <summary>
        /// 登录验证
        /// </summary>
        public JsonResult CheckUser(string UserName, string Pwd, string ValidateCode = null)
        {
            //try
            //{
            //    //CheckValdateCode(ValidateCode);

            //    Pwd = DESCrypt.Encrypt(Pwd);
            //    Member_Info model = DB.Member_Info.FindEntity(q => q.Code == UserName && q.LoginPwd == Pwd);
            //    if (model != null)
            //    {
            //        string openid = CookieHelper.GetCookieValue("openid");
            //        //判断当前微信是否绑定了账户
            //        if (string.IsNullOrEmpty(openid) == false)
            //        {
            //            if (string.IsNullOrEmpty(model.OpenID))
            //            {
            //                model.OpenID = openid;
            //            }
            //            model.Photo = CookieHelper.GetCookieValue("headimgurl");
            //            DB.Member_Info.Update(model);
            //        }

            //        //保存信息到客户端
            //        User_Shop.SetUser(model);

            //        return Success("登录成功");
            //    }
            //    return Error("用户名或密码不正确");
            //}
            //catch (Exception ex)
            //{
            //    return Error(ex);
            //}
            try
            {
                Pwd = DESCrypt.Encrypt(Pwd);
                Member_Info model = DB.Member_Info.FindEntity(q => q.Code == UserName && q.LoginPwd == Pwd);
                if (model != null)
                {
                    if (model.IsLock == "是")
                        throw new Exception("账户已经被锁定");
                    //保存信息到客户端
                    User_Shop.SetUser(model);

                    return Success("登录成功");
                }
                return Error("用户名或密码不正确");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        /// <summary>
        /// 注册添加用户
        /// </summary>
        /// <returns></returns>
        public JsonResult AddUser(string UserName, string Pwd, string Emial, string ValidateCode)
        {
            try
            {
                //判断验证码是否正确
                CheckValdateCode(ValidateCode);
                //用户是否存在
                if (DB.Member_Info.Any(q => q.Code == UserName))
                    throw new Exception("用户名已经被使用请更换");
                //添加账号
                Member_Info model = new Member_Info();
                model.MemberId = Guid.NewGuid().ToString();
                model.Code = UserName;
                model.LoginPwd = DESCrypt.Encrypt(Pwd);
                model.Email = Emial;
                model.CreateTime = DateTime.Now;
                var levle = DB.Sys_Level.Where(p => p.LevelId == 1).FirstOrDefault();
                model.MemberLevelId = levle.LevelId;
                model.MemberLevelName = levle.LevelName;
                model.ActiveAmount = levle.Investment;
                //收益等默认值
                model.Coins = 0;
                model.Scores = 0;
                model.CongXiao = 0;
                model.Position = "";
                //model.RPosition = DB.Member_Info.getRPosition(rem);
                model.Commission = 0;
                model.CommissionSum = 0;
                
                if (DB.Member_Info.Insert(model) == false)
                    throw new Exception("注册会员失败");

                return Success("注册成功");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        private void CheckValdateCode(string ValidateCode)
        {
            string code = Session["validate"] as string;
            if (string.IsNullOrEmpty(code))
                throw new Exception("验证码已经过期");
            if (code != ValidateCode)
                throw new Exception("验证码不正确");
        }


        /// <summary>
        /// 用户退出登录
        /// </summary>
        /// <returns></returns>
        public JsonResult LogOff()
        {
            try
            {
                User_Shop.Clear();
                return Success("退出成功");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
    }
}