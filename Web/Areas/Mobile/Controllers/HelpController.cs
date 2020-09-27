using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.Mobile.Controllers
{
    // 文章列表  文章详情
    public class HelpController : MobileBaseController
    {
        // GET: Mobile/Help
        public ActionResult Index()
        {
            return View();
        }
    }
}