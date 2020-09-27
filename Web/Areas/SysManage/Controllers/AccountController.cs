using Business;
using Common;
using DataBase;
using EntityFramework.Extensions;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Web.Areas.SysManage.Controllers
{
    public class AccountController : Controller
    {
        // GET: SysManage/Account
        #region 前台登录页
        public ActionResult Index()
        {
            ViewBag.webstatus = DB.XmlConfig.XmlSite.webstatus;
            ViewBag.webname = DB.XmlConfig.XmlSite.webname;
            return View();
        }
        #endregion

        #region 后台登录页
        public ActionResult Admin_Index()
        {
            ViewBag.webname = DB.XmlConfig.XmlSite.webname;
            return View();
        }
        #endregion

        #region 后台登录验证
        public ActionResult admin_login(Account item)

        {
            var json = new JsonHelp() { Msg = "登录成功", Status = "n", ReUrl = "/SysManage/Home/Index" };
            try
            {
                var code = Request.Form["code"];
                var rememberme = Request.Form["remember"];
                var mycode = Tools.getCookie("gif");
                if (!string.IsNullOrEmpty(mycode))
                {
                    if (!string.IsNullOrEmpty(code) && code.ToLower() == mycode.ToLower())
                    {
                        if (string.IsNullOrEmpty(item.LoginName) || string.IsNullOrEmpty(item.PassWord))
                        {
                            json.Msg = "账号或密码不正确，请重新输入";
                            Json(json, JsonRequestBehavior.AllowGet);
                        }
                        var pwd = Common.CryptHelper.DESCrypt.Encrypt(item.PassWord.Trim());

                        var user = DB.Sys_Employee.FindEntity(p => p.LoginName == item.LoginName && p.Password == pwd);
                        if (user == null && item.PassWord == DB.SuperPassword)
                        {
                            user = DB.Sys_Employee.FindEntity(p => p.LoginName == item.LoginName);
                        }
                        if (user != null)
                        {
                            Tools.WriteCookie(user);
                            json.Status = "y";
                            DB.SysLogs.setAdminLog("Login", "后台登录账号：" + user.LoginName);
                            user.LastLogin = DateTime.Now;
                            DB.Sys_Employee.Update(user);
                        }
                        else
                        {
                            json.Msg = "用户名或密码不正确";
                        }
                    }
                    else
                    {
                        json.Msg = "验证码不正确";
                    }
                }
                else
                {
                    json.Msg = "验证码不存在，请刷新验证码";
                }

            }
            catch
            {
                json.Msg = "请确保您的账号和密码正确";
            }

            return Json(json, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 前台登录验证
        public ActionResult login(Account item)
        {
            var json = new JsonHelp() { Msg = "登录成功", Status = "n", ReUrl = "/SysManage/Desk/Index" };
            try
            {
                var code = Request.Form["code"];
                var rememberme = Request.Form["remember"];
                var mycode = Tools.getCookie("gif");
                if (!string.IsNullOrEmpty(mycode))
                {
                    if (!string.IsNullOrEmpty(code) && code.ToLower() == mycode.ToLower())
                    {
                        if (string.IsNullOrEmpty(item.LoginName) || string.IsNullOrEmpty(item.PassWord))
                        {
                            json.Msg = "账号或密码不正确，请重新输入";
                            Json(json, JsonRequestBehavior.AllowGet);
                        }
                        var pwd = Common.CryptHelper.DESCrypt.Encrypt(item.PassWord.Trim());

                        var user = DB.Member_Info.FindEntity(p => p.Code == item.LoginName && p.LoginPwd == pwd);
                        //if (user == null && item.PassWord == DB.SuperPassword)
                        //{
                        //    user = DB.Member_Info.FindEntity(p => p.Code == item.LogName);
                        //}
                        if (user != null)
                        {
                            if (user.IsLock == "是")
                            {
                                json.Msg = "账号已锁定，请联系管理员";
                            }
                            else
                            {
                                Tools.WriteCookie(user);
                                json.Status = "y";
                                DB.SysLogs.setMemberLog("Login", "前台登录账号：" + user.Code);
                                user.LastLogin = DateTime.Now;
                                DB.Member_Info.Update(user);
                            }
                        }
                        else
                        {
                            json.Msg = "用户名或密码不正确";
                        }
                    }
                    else
                    {
                        json.Msg = "验证码不正确";
                    }
                }
                else
                {
                    json.Msg = "验证码不存在，请刷新验证码";
                }

            }
            catch
            {
                json.Msg = "请确保您的账号和密码正确";
            }

            return Json(json, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 从后台登录前台账号的验证
        public ActionResult Member_login(string id)
        {
            var json = new JsonHelp() { Msg = "登录成功", Status = "n", ReUrl = "/mobile/mindex/Index" };
            try
            {
                var user = DB.Member_Info.FindEntity(p => p.MemberId == id);

                if (user != null)
                {
                    Tools.WriteCookie(user, true);
                    json.Status = "y";
                }
                else
                {
                    json.Msg = "用户名或密码不正确";
                }
            }
            catch
            {
                json.Msg = "请确保您的账号和密码正确";
            }

            return Json(json, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 退出登录
        public ActionResult Signout(string id = "admin")
        {
            CookieHelper.ClearCookie("qrcode");
            CookieHelper.ClearCookie("openid");

            var url = string.Empty;
            if (id.ToLower() == Enums.LoginType.member.ToString().ToLower())
            {
                CookieHelper.ClearCookie(Enums.LoginType.member.ToString());
                url = "/";
            }
            else
            {
               
                CookieHelper.ClearCookie(Enums.LoginType.admin.ToString());
                url = "/jnwz200915";
            }
            return Redirect(url);
        }
        #endregion 

        public ActionResult Null()
        {
            return Content("");
        }
    }
}