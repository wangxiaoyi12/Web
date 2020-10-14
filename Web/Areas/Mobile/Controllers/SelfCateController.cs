using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.Mobile.Controllers
{
    public class SelfCateController : MobileBaseController
    {
        // GET: Mobile/SelfCate
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AllIndex()
        {
            return View();
        }
        public PartialViewResult GetClass()
        {
            ViewBag.ShopID = Request["ShopID"];
            return PartialView();
        }
        public PartialViewResult List(string keywords, int? brandid, int? classid,
          int praise = 0,
          int order = 0,
          int ordertype = 0,
          int skipCount = 0
          )
        {
            //筛选
            var query = DB.ShopProduct.Where();
            if (classid != null)
            {
                ShopProductCategory cate = DB.ShopProductCategory.FindEntity(classid.Value);
                List<int> childID = DB.ShopProductCategory.GetChildIDList(cate);
                childID.Add(classid.Value);
                query = query.Where(q => q.CategoryID != null && childID.Contains(q.CategoryID.Value));
            }
            if (brandid != null)
                query = query.Where(q => q.BrandID == brandid);

            if (string.IsNullOrEmpty(keywords) == false)
                query = query.Where(q => q.Title.Contains(keywords));

            if (praise == 1)
            {
                query = query.Where(q => q.IsHot == true);
            }
            ViewBag.praise = praise;
            query = query.Where(q => q.IsEnable == true);
            ViewBag.allCount = query.Count();
            //排序
            #region 排序
            if (order == 0)
            {
                if (praise == 1)
                {
                    if (ordertype == 0)
                    {
                        query = query.OrderByDescending(q => q.PraiseCount)
                      .ThenBy(q => q.CreateTime);
                    }
                    else
                    {
                        query = query.OrderBy(q => q.PraiseCount)
                      .ThenBy(q => q.CreateTime);
                    }
                }
                else
                {
                    if (ordertype == 0)
                    {
                        query = query.OrderBy(q => q.Sort)
                            .ThenByDescending(q => q.IsHot)
                     .ThenByDescending(q => q.CreateTime);
                    }
                    else
                    {
                        query = query.OrderByDescending(q => q.Sort)
                         .ThenByDescending(q => q.IsHot)
                         .ThenByDescending(q => q.CreateTime);
                    }
                }
            }
            else if (order == 1)
            {
                if (ordertype == 0)
                {
                    query = query.OrderBy(q => q.ShopOrderProducts.Count());
                }
                else
                {
                    query = query.OrderByDescending(q => q.ShopOrderProducts.Count());
                }
            }
            else if (order == 2)
            {
                if (ordertype == 0)
                {
                    query = query.OrderBy(q => q.PriceShopping);
                }
                else
                {
                    query = query.OrderByDescending(q => q.PriceShopping);
                }
            }

            #endregion
            //分页
            List<ShopProduct> list = query.Skip(skipCount)
                .Take(8)
                .ToList();
            ViewBag.skipCount = skipCount + list.Count;
            return PartialView(list);
        }

    }
}