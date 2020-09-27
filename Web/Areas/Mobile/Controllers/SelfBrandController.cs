using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Common;
using Business;
using DataBase;
namespace Web.Areas.Mobile.Controllers
{
    public class SelfBrandController : MobileBaseController
    {
        // GET: Mobile/SelfBrand
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult GetBrand()
        {
            bool IsMySelf = Url_Mobile.IsSelf();
            //国际大品牌
            List<ShopBrand> brand1 = DB.ShopBrand.Where(q => q.IsWorld).OrderByDescending(q => q.Sort)
                .ToList();
            //推荐品牌
            List<ShopBrand> brand2 = DB.ShopBrand.Where(q => q.IsRecommend).OrderByDescending(q => q.IsRecommend)
                .ToList();
            //国货精品
            List<ShopBrand> brand3 = DB.ShopBrand.Where(q => q.IsWorld == false).OrderByDescending(q => q.IsWorld == false)
                .ToList();

            ViewBag.brand1 = brand1;
            ViewBag.brand2 = brand2;
            ViewBag.brand3 = brand3;
            return PartialView();
        }
    }
}