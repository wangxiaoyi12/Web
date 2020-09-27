using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.Shop.Controllers
{
    /// <summary>
    /// 商城控制器基类
    /// </summary>
    public class ShopBaseController : Controller
    {


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
            return Json(obj,JsonRequestBehavior.AllowGet);
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
            return Json(obj, JsonRequestBehavior.AllowGet);
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

    }
}