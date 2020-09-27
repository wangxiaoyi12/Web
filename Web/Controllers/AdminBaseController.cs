using Common;
using System;

namespace Web.Controllers
{
    /// <summary>
    /// 控制器基类，主要做登录用户、权限认证、日志记录等工作
    /// add dukang
    /// </summary>    
    public class AdminBaseController : BaseController
    {
        public override Enums.LoginType getLoginType()
        {
            return Enums.LoginType.admin;
        }
    }
}
