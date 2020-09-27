using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    /// <summary>
    /// 局部视图控制器
    /// </summary>
    public class PartController : Controller
    {
        // GET: Part
        [OutputCache(Duration = 3600, VaryByParam = "id")]
        public PartialViewResult Menu(string id)
        {
            var x = id.Split('_');
            if (x[0] == Enums.LoginType.admin.ToString())
            {
                ViewBag.Url = "/SysManage/Home/Index";
            }
            else
            {
                ViewBag.Url = "/SysManage/Desk/Index";
            }
            ViewBag.RoleID = Convert.ToInt32(x[1]);
            return PartialView();
        }
    }
}