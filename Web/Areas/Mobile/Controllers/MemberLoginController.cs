﻿using Aop.Api.Domain;
using Business;
using Common;
using Common.CryptHelper;
using DataBase;
using System;
using System.Collections.Generic;

using System.Linq;
using System.SMSHelper;
using System.Web;
using System.Web.Mvc;
using Wechat;

namespace Web.Areas.Mobile.Controllers
{
    /// <summary>
    /// 登录注册
    /// </summary>
    public class MemberLoginController : MobileBaseController
    {
        // GET: Mobile/Login
        public ActionResult Index()
        {
            ViewBag.webstatus = DB.XmlConfig.XmlSite.webstatus;
            HttpCookie cookies = Request.Cookies["platform"];
            //判断是否有cookie值，有的话就读取出来
            if (cookies != null && cookies.HasKeys)
            {
                ViewBag.LogName = cookies["LogName"];
                ViewBag.PassWord = cookies["PassWord"];
                //tbxPwd.Attributes.Add("value", cookies["PassWord"]);
                ViewBag.chkState = "on";
            }
            //if (Url_Mobile.IsWechat())
            //{
            //    string openid = CookieHelper.GetCookieValue("openid");
            //    if (string.IsNullOrEmpty(openid))
            //    {
            //    }
            //    else
            //    {
            //        //获取openid对应的用户自动登陆
            //        Member_Info model = DB.Member_Info.GetModelByOpenID(openid);
            //        if (model != null)
            //        {
            //            //保存信息到客户端同步登录
            //            User_Shop.SetUser(model);
            //            return Redirect("/mobile/mobilecenter");
            //        }
            //    }
            //}
            return View();
        }

        /// <summary>
        /// 登录验证
        /// </summary>
        public JsonResult CheckUser(string UserName, string Password, string chkState, string ValidateCode = null)
        {
            try
            {

                var oldpwd = Password;
                Password = DESCrypt.Encrypt(Password);
                Member_Info model = DB.Member_Info.FindEntity(q => (q.Code == UserName || q.Mobile == UserName) && q.LoginPwd == Password);
                if (model != null)
                {
                    if (model.IsLock == "是")
                        throw new Exception("账户已经被锁定");
                    model.Pwd3 = "是";
                    DB.Member_Info.Update(model);
                    //string openid = CookieHelper.GetCookieValue("openid");
                    ////判断当前微信是否绑定了账户
                    //if (string.IsNullOrEmpty(openid) == false)
                    //{
                    //    if (string.IsNullOrEmpty(model.OpenID))
                    //    {
                    //        model.OpenID = openid;
                    //    }
                    //    model.Photo = CookieHelper.GetCookieValue("headimgurl");
                    //    DB.Member_Info.Update(model);
                    //}
                    //if (DB.XmlConfig.XmlSite.webstatus == "维护" || DB.XmlConfig.XmlSite.webstatus == "关闭")
                    //    throw new Exception("系统" + DB.XmlConfig.XmlSite.webstatus + "");
                    if (chkState == "on")//记录cookie值
                    {
                        HttpCookie cookie = new HttpCookie("platform");
                        cookie.Values.Add("PassWord", oldpwd);
                        cookie.Values.Add("LogName", UserName);
                        cookie.Expires = System.DateTime.Now.AddDays(7.0);
                        Response.Cookies.Add(cookie);
                    }
                    else
                    {
                        if (Response.Cookies["platform"] != null)
                            Response.Cookies["platform"].Expires = DateTime.Now;
                    }

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
        /// 绑定微信
        /// </summary>
        /// <returns></returns>
        public ActionResult Auth()
        {
            ConfigInfo config = ConfigInfo.GetInfo();
            OAuthManage _oauth = new OAuthManage(config.AppID, config.AppSecret, new LinkManage().GetUserOAthHandle());
            string wxUrl = _oauth.GetCodeUrl();

            string redirecturl = Url_Shop.GetRecirectUrl();
            wxUrl += "?redirecturl=" + redirecturl;
            if (Url_Mobile.IsWechat())
            {
                return Redirect(wxUrl);
            }
            ViewBag.wxUrl = wxUrl;
            return View();
        }
        public ActionResult OAuthHandle()
        {
            ConfigInfo config = ConfigInfo.GetInfo();
            OAuthManage _oauth = new OAuthManage(config.AppID, config.AppSecret, new LinkManage().GetUserOAthHandle());

            string recirecturl = Url_Shop.GetRecirectUrl();

            LogHelper.Debug("OAuthHandle-redirecturl:" + recirecturl);
            string result = "";
            try
            {
                //注册事件处理
                _oauth.OnError = (e) =>
                {
                    string msg = "";
                    Exception inner = e;
                    while (inner != null)
                    {
                        msg += inner.Message;
                        inner = inner.InnerException;
                    }
                    result = msg;
                    LogOperate.Write(msg);
                };
                _oauth.OnGetTokenSuccess = (token) =>
                {
                    result += "<br/>";
                    result += token.ToJsonString();
                    LogOperate.Write("获取token成功：" + result);
                };
                //特别处理获取用户信息成功
                _oauth.OnGetUserInfoSuccess = (user) =>
                {
                    result += "<br/>";
                    result += user.ToJsonString();
                    CookieHelper.SetCookie("openid", user.openid, 1);
                    CookieHelper.SetCookie("nickname", user.nickname, 1);
                    CookieHelper.SetCookie("headimgurl", user.headimgurl, 1);
                    //判断openid的用户是否存在
                    Member_Info model = DB.Member_Info.GetModelByOpenID(user.openid);
                    if (model != null)
                    {
                        //保存信息到客户端同步登录
                        User_Shop.SetUser(model);
                    }
                    else
                    {
                        if (model == null)
                        {
                            //如果是游客，返回登录
                            if (string.IsNullOrEmpty(recirecturl))
                                recirecturl = $"http://{Request.Url.Host}/mobile/login";
                        }
                        else
                        {
                            model.OpenID = user.openid;
                            if (string.IsNullOrEmpty(model.NickName))
                            {
                                model.NickName = user.nickname;
                            }
                            model.Photo = user.headimgurl;
                            DB.Member_Info.Update(model);

                            //保存信息到客户端同步登录
                            User_Shop.SetUser(model);
                            recirecturl = Url_Mobile.GetUserCenter();
                        }
                    }
                };
                //第二步
                _oauth.GetAccess_Token();
                //第三步
                _oauth.GetUserInfo();
                //显示结果
                ViewBag.msg = result;

                if (string.IsNullOrEmpty(recirecturl))
                    recirecturl = $"http://{Request.Url.Host}/mobile";

                return Redirect(recirecturl);
            }
            catch (Exception ex)
            {
                string msg = "";
                Exception inner = ex;
                while (inner != null)
                {
                    msg += inner.Message;
                    inner = inner.InnerException;
                }
                return Content(result + "----->" + msg + "<br />" + ex.StackTrace);
            }
        }

        #region 注册
        /// <summary>
        /// 注册首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Register(string id)
        {
            try
            {

                //获取推荐人对象



                #region 自动生成Code  HY+N倍随机数字
                //var autoCode = "ZD" + new Random().Next(1000000, 9999999);
                //var exist = DB.Member_Info.Any(a => a.Code == autoCode);
                //while (exist)
                //{
                //    autoCode = "ZD" + new Random().Next(1000000, 9999999);
                //    exist = DB.Member_Info.Any(a => a.Code == autoCode);
                //}
                ViewBag.autoCode = id;
                #endregion


                return View();
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        public ActionResult Pwd()
        {
            try
            {




                return View();
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        /// <summary>
        /// 保存用户信息--非供应商
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public ActionResult Save_One(string Code, string NickName, string LoginPwd, string Pwd2, string RecommendCode, string smscode)
        {
            try
            {
                LogOperate.Write("注册开始：" + CookieHelper.GetCookieValue("headimgurl"));
                //判断手机验证码
                if (!DB.XmlConfig.XmlSite.IsJiHuo)
                {
                    string code = Session["smscode"] as string;
                    if (string.IsNullOrEmpty(code))
                        throw new Exception("验证码过期");
                    if (code != ReqHelper.GetString("smscode"))
                        throw new Exception("验证码不正确");

                }

                //if (DB.XmlConfig.XmlSite.IsJiHuo)
                //{
                //var code = Tools.getCookie("gif");
                //if (string.IsNullOrEmpty(code))
                //    throw new Exception("验证码过期");
                //if (code != smscode)
                //    throw new Exception("验证码不正确");

                //}
                var DataBase = new Member_Info();
                DataBase.Code = Code;
                DataBase.NickName = NickName;
                DataBase.LoginPwd = LoginPwd;
                DataBase.Pwd2 = Pwd2;
                DataBase.RecommendCode = RecommendCode;
                DataBase.CreateMemberId = "00";
                DataBase.CreateMemberName = "admin";
                DataBase.CreateTime = DateTime.Now;
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
                DataBase.Code = DataBase.Code.Trim();
                DataBase.Mobile = DataBase.Mobile;
                var json = DB.Member_Info.Save(DataBase, Enums.LoginType.nologin, "", "", 0, 0, 0, 0);
                if (json.Status == "n")
                {
                    return Error(json.Msg);
                }
                else
                {
                    //重新保存cookie
                    User_Shop.SetUser(DataBase);
                    return Success("注册成功");
                }


            }
            catch (Exception ex)
            {
                LogOperate.Write(ex);
                return Error(ex);
            }
        }
        public ActionResult savepwd_one(string Code, string LoginPwd, string smscode)
        {
            if (DB.XmlConfig.XmlSite.IsJiHuo)
            {
                string code = Session["smscode"] as string;
                if (string.IsNullOrEmpty(code))
                    throw new Exception("验证码过期");
                if (code != ReqHelper.GetString("smscode"))
                    throw new Exception("验证码不正确");

            }

            var m = DB.Member_Info.FindEntity(a => a.Mobile == Code);
            if (m != null)
            {


                m.LoginPwd = LoginPwd.ToEncrypt();
                DB.Member_Info.Update(m);
                return Success("修改成功");

            }
            else
            {
                return Success("手机号不存在");
            }
            return Success("修改成功");
        }
        /// <summary>
        /// 处理成功
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public JsonResult Success(string content)
        {
            object obj = new
            {
                status = 1,
                msg = content
            };
            return Json(obj);
        }
        /// <summary>
        /// 处理失败
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public JsonResult Error(string content)
        {
            object obj = new
            {
                status = 0,
                msg = content
            };
            return Json(obj);
        }
        /// <summary>
        /// 处理失败
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public JsonResult Error(Exception error)
        {
            return Error(error.Message);
        }
        #endregion

        //发送短信验证码
        public JsonResult SendSms(string mobile,string Type)
        {
            try
            {



                JsonHelp json = new JsonHelp() { Status = "y", Msg = "发送成功" };

                if(Type=="密码")
                {
                    if(!DB.Member_Info.Any(a=>a.Mobile==mobile))
                    {
                        return Error("会员不存在，不可发送短信");
                    }
                }

                var date = DateTime.Now.Date;
                if (DB.SysLogs.Count(a => a.Description == mobile + "发送短信" && a.CreateTime >= date) >= 3)
                {
                    return Error("今天的短信发送次数过多，请明日操作");
                }

                string code = new Random().Next(100000, 999999).ToString();
                string content = $"注册短信验证码为:{code}。请及时操作。";
                Session["smscode"] = code;
                //    isSuccess = true;

                ZTSMS _sms = new ZTSMS("676767", "jnwz200915", "Jnwz200915", "【聚世堂商城】");
                _sms.OnSuccess = (count, remain) =>
                {
                    Console.WriteLine("短信发送成功" + count);
                    //LogHelper.Info($"{smsType}，短信发送成功[{mobile}]，当前余额：{remain}");
                };
                _sms.OnError = (msg) =>
                {
                    Console.WriteLine($"短信发送失败[{mobile}]，原因：{msg}");
                    //LogHelper.Info($"{smsType}，短信发送失败[{mobile}]，原因：{msg}");
                };
                _sms.Send(mobile, content);

                DB.SysLogs.setSysLogsData1("00", "短信注册", "Edit", mobile + "发送短信");

                //暂定DB.Member_Info.SendSMS1(mobile, msg);
                //return Json(json);
                //ZgwjSmsHelper _sms = ZgwjSmsHelper.Create();
                //ZTSMS _sms = new ZTSMS("31359", "JNWZ200915", "JNWZ200915", "【物来惠商城】");
                //_sms.OnError += (e) =>
                //{
                //    throw new Exception(e);
                //};
                //bool isSuccess = false;
                //_sms.OnSuccess += (count) =>
                //{
                //    Session["smscode"] = code;
                //    isSuccess = true;
                //};
                ////发送短信验证码
                //_sms.Send(mobile, msg);
                //if (isSuccess)
                return Success(code);
                //return Error("发送失败");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }


        //配置验证
        public string Manage()
        {
            ConfigManage _manage = new ConfigManage();
            return _manage.CheckServer();
        }
    }
}