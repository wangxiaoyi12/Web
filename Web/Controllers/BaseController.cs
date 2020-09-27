using Business;
using Common;
using DataBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Web.Controllers
{
    //[TimingActionFilter]
    public abstract class BaseController : Controller
    {
        // GET: Base
        #region 属性
        protected Enums.LoginType LoginType
        {
            get
            {
                return getLoginType();
            }
        }
        private string controllerPath;
        protected string ControllerPath
        {
            get
            {
                if (string.IsNullOrEmpty(controllerPath))
                {
                    controllerPath = string.Format("/{0}/{1}", ControllerContext.RouteData.DataTokens["area"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
                }
                return controllerPath;
            }
        }
        /// <summary>
        /// 设置登录类型，也就是前台登录还是后台登录
        /// </summary>
        /// <returns></returns>
        public abstract Enums.LoginType getLoginType();

        #endregion

        #region 记录操作日志
        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="Event">操作事件</param>
        /// <param name="Description">描述</param>
        /// <returns></returns>
        public bool Log(Enums.EventType Event, string Description)
        {
            if (LoginType == Enums.LoginType.admin)  //后台写操作日志
            {
                return DB.SysLogs.setAdminLog(Event, Description);
            }
            else if (LoginType == Enums.LoginType.member) //前台写操作日志
            {
                return DB.SysLogs.setMemberLog(Event, Description);
            }
            return false;
        }
        #endregion

        #region 获取当前用户对象
        private Account currentUser = null;
        /// <summary>
        /// 获取当前用户对象
        /// </summary>
        public Account CurrentUser
        {
            get
            {
                //return GetAccountByCookie();
                if (currentUser == null) currentUser = GetAccountByCookie();
                return currentUser;
            }
        }

        /// <summary>
        /// 从Cookie中获取用户信息
        /// </summary>
        public Account GetAccountByCookie()
        {
            var cookie = CookieHelper.GetCookie(LoginType.ToString());
            if (cookie != null)
            {
                //验证json的有效性
                if (!string.IsNullOrEmpty(cookie.Value))
                {
                    //解密
                    var cookievalue = Common.CryptHelper.DESCrypt.Decrypt(cookie.Value);
                    if (!string.IsNullOrEmpty(cookievalue))
                    {
                        //是否为json
                        if (!JsonSplit.IsJson(cookievalue)) return null;
                        try
                        {
                            var jsonFormat = Common.JsonConverter.ConvertJson(cookievalue);
                            if (jsonFormat != null && jsonFormat.token == DB.ValidCookieString)
                            {
                                string id = jsonFormat.id;
                                string username = jsonFormat.username;
                                string loginname = jsonFormat.loginname;
                                string userpwd = jsonFormat.password;
                                string pwd2 = DB.ValidCookieString;
                                string logintype = jsonFormat.logintype;
                                string roleId = jsonFormat.roleid;
                                string isadmin = "false";
                                if (logintype == Enums.LoginType.member.ToString())
                                {
                                    isadmin = jsonFormat.isadmin;
                                }
                                int roleid = 1;
                                int.TryParse(roleId, out roleid);
                                if (LoginType == Enums.LoginType.member)
                                {
                                    pwd2 = jsonFormat.pwd2;
                                }

                                Account account = new Account()
                                {
                                    Id = id,
                                    Name = username,
                                    LoginName = loginname,
                                    PassWord = userpwd,
                                    PassWord2 = pwd2,
                                    LoginType = LoginType.ToString(),
                                    IsAdmin = Convert.ToBoolean(isadmin),
                                    RoleID = roleid
                                    //Role_Nav = DB.Sys_Role_Nav.Where(a => a.role_id == roleid).ToList()
                                };
                                return account;
                            }
                        }
                        catch { return null; }
                    }
                }
            }
            return null;
        }
        #endregion

        #region 重写Initialize  OnActionExecuting(不需要) 方法
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            var url = "/";
            if (LoginType == Enums.LoginType.admin)
            {
                url = "/JNWZ200915";
            }
            if (CurrentUser == null)
            {
                requestContext.HttpContext.Response.Write("<script type='text/javascript'> alert('登录已过期，请重新登录'); window.top.location='" + url + "';</script>");
                requestContext.HttpContext.Response.End();
                return;
            }
            if (LoginType == Enums.LoginType.member)
            {
                if (CookieHelper.GetCookie("admin") == null && (DB.XmlConfig.XmlSite.webstatus == "关闭" || DB.XmlConfig.XmlSite.webstatus == "维护"))
                {
                    requestContext.HttpContext.Response.Write("<script type='text/javascript'> alert('系统【" + DB.XmlConfig.XmlSite.webstatus + "】中,请联系管理员'); window.top.location='" + url + "';</script>");
                    requestContext.HttpContext.Response.End();
                }
            }

            base.Initialize(requestContext);
            ViewData["CurrentUser"] = CurrentUser;
            ViewBag.Path = ControllerPath;
        }
        #endregion

        #region 查询列表并分页
        public string ToPage(IEnumerable data, int total, int start, int length, int draw, string dataFormat = "yyyy-MM-dd HH:mm")
        {
            var result = new
            {
                draw = draw,
                recordsTotal = total,
                recordsFiltered = total,
                data = data
            }; 
            return result.ToJsonString(dataFormat);
        }
        public string ToPage(IEnumerable data, string dataFormat = "yyyy-MM-dd")
        {
            var result = new
            {
                data = data
            };
            return result.ToJsonString(dataFormat);
        }
        #endregion

        #region 导出Excel
        /// <summary>
        /// 导出excel
        /// </summary>
        /// <typeparam name="T">集合列表里对象的类型</typeparam>
        /// <param name="list">数据列表</param>
        /// <returns></returns>
        protected FileResult ToExcel(IEnumerable list)
        {
            #region 获取XML
            XMLHelp xmlhelp = new Common.XMLHelp(string.Format("/XmlConfig/excel/{0}-{1}.xml", ControllerContext.RouteData.DataTokens["area"].ToString(), ControllerContext.RouteData.Values["controller"].ToString()));
            var fields = xmlhelp.GetList<Xml_Field>().Select(a => new KeyValuePair<string, string>(a.code, a.name)).ToList();
            #endregion

            var dirPath = Server.MapPath("/upload/toexcel/");
            if (System.IO.Directory.Exists(dirPath) == false)
            {
                System.IO.Directory.CreateDirectory(dirPath);
            }
            var filePath = string.Format("{0}{1}.xls", dirPath, System.Guid.NewGuid().ToString());
            ExcelHelp.ToExcelByNPOI(list, filePath, fields);
            return File(filePath, "application/vnd.ms-excel");
        }
        /// <summary>
        /// 导出excel
        /// </summary>
        /// <typeparam name="T">集合列表里对象的类型</typeparam>
        /// <param name="list">数据列表</param>
        /// <returns></returns>
        protected FileResult ToExcel(DataTable list)
        {
            #region 获取XML
            XMLHelp xmlhelp = new Common.XMLHelp(string.Format("/XmlConfig/excel/{0}-{1}.xml", ControllerContext.RouteData.DataTokens["area"].ToString(), ControllerContext.RouteData.Values["controller"].ToString()));
            var fields = xmlhelp.GetList<Xml_Field>().Select(a => new KeyValuePair<string, string>(a.code, a.name)).ToList();
            #endregion
            var dirPath = Server.MapPath("/upload/toexcel/");
            if (System.IO.Directory.Exists(dirPath) == false)
            {
                System.IO.Directory.CreateDirectory(dirPath);
            }
            var filePath = string.Format("{0}{1}.xls", dirPath, System.Guid.NewGuid().ToString());
            ExcelHelp.ToExcelByNPOI(list, filePath, fields);
            return File(filePath, "application/vnd.ms-excel");
        }
        #endregion
    }
}