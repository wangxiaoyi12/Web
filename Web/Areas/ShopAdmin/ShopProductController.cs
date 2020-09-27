using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.ShopAdmin
{
    /// <summary>
    /// 商品管理
    /// </summary>
    public class ShopProductController : Controller
    {
        // GET: ShopAdmin/ShopProduct
        public ActionResult Index()
        {
            return View();
        }
    }
}