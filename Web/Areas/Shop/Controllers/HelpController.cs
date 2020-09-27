using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using Common;
using DataBase;
using Business;
namespace Web.Areas.Shop.Controllers
{
    /// <summary>
    /// 帮助中心模块
    /// </summary>
    public class HelpController : ShopBaseController
    {
        // GET: Shop/Help
        public ActionResult Index(int? ID)
        {
            if (ID == null)
            {
                ID = DB.ShopArticleCategory.Where(q => q.PID != null).FirstOrDefault().ID;
            }
            return View(ID.Value);
        }
    }
}