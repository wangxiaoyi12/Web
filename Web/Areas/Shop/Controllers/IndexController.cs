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
    /// 首页
    /// </summary>
    public class IndexController : ShopBaseController
    {
        /// <summary>
        /// 商城首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //跳转处理
            //if (Url_Mobile.IsMobile())
            //{
                return Redirect(Url_Mobile.GetIndex());
            //}

            //return View();
        }
        /// <summary>
        /// 品牌输出
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Brand()
        {
            return PartialView();
        }
        /// <summary>
        /// 获取各大分类的产品板块
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Plate()
        {
            return PartialView();
        }
        /// <summary>
        /// 猜你喜欢
        /// </summary>
        /// <returns></returns>
        public PartialViewResult GuessLike()
        {
            //1.获取列表
            List<ShopProduct> list = DB.ShopProduct.Where(q => q.IsHot && q.IsEnable)
                .OrderByDescending(q => q.CreateTime)
                .Take(8)
                .ToList();
            return PartialView(list);
        }
        /// <summary>
        /// 获取头部购物车
        /// </summary>
        /// <returns></returns>
        public PartialViewResult TopCart()
        {
            return PartialView();
        }
    }
}