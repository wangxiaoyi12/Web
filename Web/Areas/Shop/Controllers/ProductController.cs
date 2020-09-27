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
    /// 产品模块
    /// </summary>
    public class ProductController : ShopBaseController
    {
        /// <summary>
        /// 产品列表页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取今日推荐内容
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Today()
        {
            List<ShopProduct> list = DB.ShopProduct.Where(q => q.IsRecommend == true && q.IsEnable)
                .OrderBy(q => q.Sort).OrderByDescending(q => q.CreateTime)
                .Take(10)
                .ToList();
            return PartialView(list);
        }

        /// <summary>
        /// 浏览记录
        /// </summary>
        /// <returns></returns>
        public PartialViewResult History()
        {
            List<ShopProduct> list = new List<ShopProduct>();
            //判断用户是否登录
            if (User_Shop.IsLogin())
            {
                string curUserID = User_Shop.GetMemberID();
                list = DB.ShopBrowsingHistory.Where(q => q.MemberID == curUserID && q.ShopProduct.IsEnable)
                    .OrderByDescending(q => q.CreateTime)
                    .Select(q => q.ShopProduct)
                    .Take(3)
                    .ToList();
                ViewBag.title = "浏览记录";
            }
            else
            {
                list = DB.ShopProduct.Where(q => q.IsNew&&q.IsEnable)
                    .OrderByDescending(q => q.CreateTime)
                    .Take(3)
                    .ToList();
                ViewBag.title = "最新商品";
            }
            return PartialView(list);
        }


        /// <summary>
        /// 获取销售排行
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Rank()
        {
            List<ShopProduct> list = DB.ShopProduct.Where(q => q.IsEnable)
                .OrderByDescending(q => q.Sales)
                .Take(10)
                .ToList();
            return PartialView(list);
        }
        /// <summary>
        /// 产品详细页面
        /// </summary>
        /// <returns></returns>
        //public ActionResult Detail(int id)
        //{
        //   string curUserID = User_Shop.GetMemberID();
        //    ViewBag.member = DB.Member_Info.Where(p => p.MemberId == curUserID).FirstOrDefault();
        //   ShopProduct model = DB.ShopProduct.FindEntity(q => q.ID == id);
        //    if (model == null)
        //        throw new HttpException("您要查看的商品不存在或已经删除");
        //    //添加用户的浏览记录
        //    if (User_Shop.IsLogin())
        //    {
        //        DB.ShopBrowsingHistory.Add(id, User_Shop.GetMemberID());
        //    }
        //    return View(model);
        //}
        public ActionResult Detail(int id)
        {
            if (User_Shop.IsLogin())
            {
                string curUserID = User_Shop.GetMemberID();
                ViewBag.member = DB.Member_Info.FindEntity(p => p.MemberId == curUserID);
                DB.ShopBrowsingHistory.Add(id, User_Shop.GetMemberID());
            }
            ShopProduct model = DB.ShopProduct.FindEntity(q => q.ID == id);
            if (model == null)
                throw new HttpException("您要查看的商品不存在或已经删除");
            return View(model);
        }
        /// <summary>
        /// 浏览记录
        /// </summary>
        /// <returns></returns>
        public PartialViewResult History_Detail()
        {
            List<ShopProduct> list = new List<ShopProduct>();
            //判断用户是否登录
            if (User_Shop.IsLogin())
            {
                string curUserID = User_Shop.GetMemberID();
                list = DB.ShopBrowsingHistory.Where(q => q.MemberID == curUserID && q.ShopProduct.IsEnable)
                    .OrderByDescending(q => q.CreateTime)
                    .Select(q => q.ShopProduct)
                    .Take(3)
                    .ToList();
                ViewBag.title = "浏览记录";
            }
            else
            {
                list = DB.ShopProduct.Where(q => q.IsNew && q.IsEnable)
                    .OrderByDescending(q => q.CreateTime)
                    .Take(3)
                    .ToList();
                ViewBag.title = "最新商品";
            }
            return PartialView(list);
        }
        /// <summary>
        /// 同类产品推荐
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Recommend_Detail(int classID)
        {
            //获取同类产品推荐内容
            List<ShopProduct> list = DB.ShopProduct.Where(q => q.IsEnable == true && q.CategoryID == classID)
                .OrderByDescending(q => q.IsRecommend)
                .ThenByDescending(q => q.CreateTime)
                .Take(4)
                .ToList();
            return PartialView(list);
        }
        /// <summary>
        /// 同类销售排行
        /// </summary>
        /// <param name="classID"></param>
        /// <returns></returns>
        public PartialViewResult Range_Detail(int classID)
        {
            List<ShopProduct> list = DB.ShopProduct.Where(q => q.IsEnable == true && q.CategoryID == classID)
                    .OrderByDescending(q => q.Sales)
                    .ThenByDescending(q => q.CreateTime)
                    .Take(5)
                    .ToList();
            return PartialView(list);
        }


    }
}