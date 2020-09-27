using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.Mobile.Controllers
{
    /// <summary>
    /// 个人中心，订单管理，收货地址管理等
    /// </summary>
    public class MIndexController : MobileBaseController
    {
        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    //判断是否登录，如果没有登录跳转登录页
        //    //if (User_Shop.IsLogin() == false)
        //    //{
        //    //    string url = "";
        //    //    //如果是微信登录跳转到授权页面
        //    //    if (Url_Mobile.IsWechat())
        //    //    {
        //    //        url = Url_Mobile.GetLogin() + "/auth";
        //    //    }
        //    //    else
        //    //    {
        //    //        url = Url_Mobile.GetLogin();
        //    //    }
        //    //    url += "?redirecturl=" + ReqHelper.req.Url.AbsolutePath;
        //    //    if (string.IsNullOrEmpty(url) == false)
        //    //        filterContext.Result = new RedirectResult(url);
        //    //}
        //    if (User_Shop.IsLogin() == false)
        //    {
        //        string url = "";
        //        url = Url_Mobile.GetLogin();
        //        url += "?redirecturl=" + ReqHelper.req.Url.AbsolutePath;
        //        if (string.IsNullOrEmpty(url) == false)
        //            filterContext.Result = new RedirectResult(url);
        //    }
        //}
        // GET: Mobile/MHome
        public ActionResult Index()
        {
            //判断是否是手机浏览器打开
            //if (Url_Mobile.IsMobile() == false)
            //{
            //    return Redirect(Url_Shop.GetIndex());
            //}
            return View();
        }
        public ActionResult Cash()
        {
            //判断是否是手机浏览器打开
            //if (Url_Mobile.IsMobile() == false)
            //{
            //    return Redirect(Url_Shop.GetIndex());
            //}
            return View();
        }
        /// <summary>
        /// 获取首页列表部分
        /// </summary>
        /// <returns></returns>
        public PartialViewResult List()
        {
            return PartialView();
        }
        /// <summary>
        /// 获取首页Banner
        /// </summary>
        /// <returns></returns>
        public PartialViewResult GetBanner(int id)
        {
            ViewBag.List = DB.ShopSlide.Where(q=>q.Type==2).Take(10).ToList();
            return PartialView();
        }
        /// <summary>
        /// 获取分类
        /// </summary>
        /// <returns></returns>
        public PartialViewResult GetCategory()
        {
            return PartialView();
        }
        public ActionResult Address()
        {
            var list = DB.ShopMyAddress.Where(a => a.MemberID == MemberID).ToList();
            ViewBag.List = list;
            return View();
        }
    }
}