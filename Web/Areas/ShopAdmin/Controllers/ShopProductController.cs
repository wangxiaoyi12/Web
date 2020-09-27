using Business;
using Common;
using DataBase;
using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.ShopAdmin.Controllers
{
    /// <summary>
    /// 商品管理
    /// </summary>
    public class ShopProductController : Web.Controllers.AdminBaseController
    {
        // GET: ShopAdmin/ShopProduct
        public ActionResult Index()
        {
            ViewBag.List1 = DB.ShopProductCategory.getFrom1Layer();
            return View();
        }
        #region 查询
        public string getDataSource(string key, int start, int length, int draw, int? category)
        {
            //var shopid = DB.Shop.getIDByMemberID(CurrentUser.Id);
            var query = DB.ShopProduct
                .Where(a => a.IsNew == false)
                .Where(a => string.IsNullOrEmpty(key) ? true : a.Title.Contains(key))
                .Where(a => category > 0 ? a.CategoryID1 == category : true)
                 .Select(a => new
                 {
                     a.ID,
                     IsEnable = a.IsEnable == true ? "上架" : "下架",
                     IsHot = a.IsHot == true ? "是" : "",
                     IsNew = a.IsNew == true ? "是" : "",
                     IsRecommend = a.IsRecommend == true ? "是" : "",
                   
                     CategoryID = a.ShopProductCategory.Name,
                     CategoryID1 = a.ShopProductCategory.Name,
                     a.CreateTime,
                     a.UpdateTime,
                     a.Image,
                     a.Inventory,
                     a.Postage,
                     a.PriceCongXiao,
                     a.PriceOriginal,
                     a.PV,
                     a.PriceScore,
                     a.PriceShopping,
                     a.PriceVip,
                     a.Sales,
                     
                     a.Sort,
                     a.Title
                 });
            var total = query.Count();
            var list = query.OrderBy(a => a.Sort)
                .ThenByDescending(q => q.CreateTime).Skip(start).Take(length).ToList();

            return ToPage(list, total, start, length, draw);
        }
        #endregion

        #region 编辑视图
        public ActionResult Detail(int? id)
        {
            //var admin = DB.Member_Info.FindEntity(p => p.MemberId == CurrentUser.Id);
            //ViewBag.code = admin.Code;
            ShopProduct entity = null;
            ViewBag.List1 = DB.ShopProductCategory.getFrom1Layer();
            ViewBag.List8 = DB.ShopProductCategory.getFrom8Layer();
            ViewBag.Brand = DB.ShopBrand.Where().Select(a => new { a.ID, a.Name })
                .ToList().Select(a => new KeyValuePair<int, string>(a.ID, a.Name));
            if (id == null)
            {
                /* var shop = DB.Shop.getIDByMemberID(CurrentUser.Id);*/  //测试时，先把第一个商家赋值给商品，后面商品增加
                entity = new ShopProduct() { CreateTime = DateTime.Now, ShopID = 1 };
            }
            else
            {
                entity = DB.ShopProduct.Where(a => a.ID == id).FirstOrDefault();
                if (entity.ShopProductCategory != null)
                {
                    if (entity.ShopProductCategory.Layer == 1)
                    {
                        ViewBag.C1 = entity.ShopProductCategory.ID;
                    }
                    if (entity.ShopProductCategory.Layer == 2)
                    {
                        ViewBag.C1 = entity.ShopProductCategory.ShopProductCategory2.ID;
                    }
                    if (entity.ShopProductCategory.Layer == 3)
                    {
                        ViewBag.C1 = entity.ShopProductCategory.ShopProductCategory2.ShopProductCategory2.ID;
                    }
                }
                var imgs = DB.ShopProductImage.Where(a => a.ProductID == id).Select(a => new { a.URL, a.ThumURL }).ToList();
                if (imgs.Count > 0)
                {
                    ViewBag.imgpath1 = imgs.Select(a => a.URL).Aggregate((m, n) => m + "&" + n) + "&";
                    ViewBag.imgpath2 = imgs.Select(a => a.ThumURL).Aggregate((m, n) => m + "&" + n) + "&";
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in imgs)
                    {
                        sb.AppendFormat("<div class=\"myImg divimg{0}\"><div style=\"position:absolute\"><a href=\"javascript:deleteimg('{1}','{2}',{0})\" class=\"btn red\"><i class=\"fa fa-remove\"></i></a></div><div><img src=\"{2}\" /></div></div>", imgs.IndexOf(item), item.URL, item.ThumURL);
                    }
                    ViewBag.SB = sb.ToString();
                }
                if (entity != null)
                {
                    var pid = DB.ShopProduct.GetCategoryId2(entity.CategoryID.Value);
                    var guiges = DB.GuiGeProduct_Info.Where(a => a.ProductId == id && a.SName == pid.ID).ToList();
                    if (guiges.Count > 0)
                    {

                        ViewBag.GG = guiges;
                    }
                    else
                    {
                        ViewBag.GG = null;
                    }
                }
                else
                {
                    ViewBag.GG = null;
                }

            }
            ViewBag.tiao = Request["tiao"];
            return View(entity);
        }
        #endregion

        public JsonResult addguige(decimal LingShou, decimal YouHui, decimal PeiHuo, int KuCun, decimal YiShou,
             string str, int productid, int SName)
        {
            JsonHelp json = new JsonHelp() { Msg = "增加数据失败" };
            GuiGeProduct_Info gg = new GuiGeProduct_Info();

            var entity = DB.ShopProduct.FindEntity(productid);
            if (entity == null)
            {
                json.Msg = "请先上传产品在选在规格";
                return Json(json);
            }
            else
            {
                if (entity.CategoryID != SName)
                {
                    json.Msg = "产品类型不一致，上传失败";
                    return Json(json);
                }
            }

            gg.LingShou = LingShou;
            gg.YouHui = YouHui;
            gg.PeiHuo = PeiHuo;
            gg.KuCun = Convert.ToInt32(KuCun);
            gg.YiShou = Convert.ToInt32(YiShou);

         

            gg.SName = DB.ShopProduct.GetCategoryId2(SName).ID;

            gg.ProductId = productid;
            string[] strlist = str.Split(',');
            string SComment = "";
            for (int i = 0; i < strlist.Length; i++)
            {
                if (i == strlist.Length - 1)
                {
                    SComment += strlist[i];
                }
                else
                {
                    SComment += strlist[i] + "_";
                }

            }
            gg.SComment = SComment;
            if (DB.GuiGeProduct_Info.Any(a => a.ProductId == productid && a.SName == SName && a.SComment == SComment))
            {
                json.Msg = "商品规格已存在";
                return Json(json);
            }
            DB.GuiGeProduct_Info.Insert(gg);
            json.Msg = "商品规格添加成功";
            json.IsSuccess = true;
            return Json(json);
        }
        public void SaveData(string id, decimal LingShou, decimal YouHui, decimal PeiHuo, int KuCun, decimal YiShou)
        {
            GuiGeProduct_Info gg = DB.GuiGeProduct_Info.FindEntity(Convert.ToInt32(id));
            gg.LingShou = LingShou;
            gg.YouHui = YouHui;
            gg.PeiHuo = PeiHuo;
            gg.KuCun = Convert.ToInt32(KuCun);
            gg.YiShou = Convert.ToInt32(YiShou);


         
            DB.GuiGeProduct_Info.Update(gg);
        }
        public JsonResult GGDelete(string idList)
        {
            JsonHelp json = new JsonHelp() { Msg = "删除数据失败" };
            //是否为空
            if (string.IsNullOrEmpty(idList))
            {
                json.Msg = "未找到要删除的数据";
                return Json(json);
            }
            var ids = idList.TrimEnd(',').Split(',').Select(a => Convert.ToInt32(a)).ToList();
            if (DB.GuiGeProduct_Info.Any(a => ids.Contains(a.GGId)))
            {
                var names = DB.GuiGeProduct_Info.Where(a => ids.Contains(a.GGId)).Select(a => a.SComment)
                    .ToList().Aggregate((m, n) => m + "," + n);
                if (DB.GuiGeProduct_Info.Delete(a => ids.Contains(a.GGId)) > 0)
                {
                    json.IsSuccess = true;
                    json.Msg = "删除数据成功";
                }
                //添加操作日志
                DB.SysLogs.setAdminLog("Delete", "删除名字为[" + names + "]的规格 ");
            }
            else
            {
                json.Msg = "未找到要删除的对象，请刷新页面重试";
            }
            return Json(json);
        }

        #region 保存
        [ValidateInput(false)]
        public ActionResult Save(ShopProduct entity)
        {
            var json = new JsonHelp();
            //var shangpin = DB.ShopProductCategory.FindEntity(p => p.ID == entity.CategoryID1);
            var shop = DB.Shop.FindEntity(p => p.ID == 1);
            entity.UpdateTime = DateTime.Now;
            var imgs1 = Request["imgpath1"];
            var imgs2 = Request["imgpath2"];
            try
            {
                using (var db = new DbMallEntities())
                {
                    if (entity.ID == 0)
                    {
                        // entity.PriceCongXiao = 0;
                        //if (entity.CategoryID1 == ShopEnum.ShopType.订单商品.GetHashCode())
                        //{
                        //    entity.PriceScore = 0;
                        //    entity.PriceVip = 0;
                        //}
                        if (shop == null)
                        {
                            json.IsSuccess = false;
                            json.Msg = "添加失败，您暂无商家权限";
                            return Json(json);
                        }
                        else if (shop.IsCheck != true)
                        {
                            json.IsSuccess = false;
                            json.Msg = "添加失败，您暂无商家权限";
                            return Json(json);
                        }
                        //if (shangpin.Name == "优选区")
                        //{
                        //    if (entity.PriceShopping < 1)
                        //    {
                        //        json.IsSuccess = false;
                        //        json.Msg = "添加失败，现金价不准为0";
                        //        return Json(json);
                        //    }
                        //    if (entity.PV < 1)
                        //    {
                        //        json.IsSuccess = false;
                        //        json.Msg = "添加失败，积分价不准为0";
                        //        return Json(json);
                        //    }
                        //}
                        //else if (shangpin.Name == "消费区")
                        //{
                        //    if (entity.PriceShopping < 1)
                        //    {
                        //        json.IsSuccess = false;
                        //        json.Msg = "添加失败，现金价不准为0";
                        //        return Json(json);
                        //    }
                        //    if (entity.PV > 1)
                        //    {
                        //        json.IsSuccess = false;
                        //        json.Msg = "添加失败，积分不需要填写";
                        //        return Json(json);
                        //    }
                        //}
                        db.ShopProducts.Add(entity);
                        db.SaveChanges();
                        #region 添加商品相册图片
                        if (!string.IsNullOrEmpty(imgs1) && imgs1.EndsWith("&"))
                        {
                            var list1 = imgs1.Substring(0, imgs1.Length - 1).Split('&');
                            var list2 = imgs2.Substring(0, imgs2.Length - 1).Split('&');
                            if (list1.Length == list2.Length)
                            {
                                var xlist = new List<ShopProductImage>();
                                for (int i = 0; i < list1.Length; i++)
                                {
                                    var img = new ShopProductImage()
                                    {
                                        CreateTime = DateTime.Now,
                                        ProductID = entity.ID,
                                        Sort = i + 1,
                                        URL = list1[i],
                                        ThumURL = list2[i]
                                    };
                                    db.ShopProductImages.Add(img);
                                }
                                var first = db.ShopProducts.FirstOrDefault(a => a.ID == entity.ID);
                                first.Image = list1[0];
                            }
                            else
                            {
                                json.IsSuccess = false;
                                json.Msg = "添加失败，请重试商品图片";
                                return Json(json);
                            }
                        }
                        #endregion
                        json.Msg = "添加";
                    }
                    else
                    {
                        var first = db.ShopProducts.FirstOrDefault(a => a.ID == entity.ID);
                        WebTools.CopyToObject(entity, first);
                        //json.IsSuccess = DB.ShopProduct.Update(first);
                        #region 修改商品相册图片
                        //if (json.IsSuccess)
                        {
                            if (!string.IsNullOrEmpty(imgs1) && imgs1.EndsWith("&"))
                            {
                                var list1 = imgs1.Substring(0, imgs1.Length - 1).Split('&');
                                var list2 = imgs2.Substring(0, imgs2.Length - 1).Split('&');
                                if (list1.Length == list2.Length)
                                {
                                    var olds = db.ShopProductImages.Where(a => a.ProductID == entity.ID);
                                    #region 先删除多余的图片
                                    foreach (var item in olds)
                                    {
                                        var exsit = list1.Any(a => a == item.URL);
                                        if (exsit == false)
                                        {
                                            db.ShopProductImages.Remove(item);
                                        }
                                    }
                                    #endregion
                                    #region 再添加原来没有的
                                    for (int i = 0; i < list1.Length; i++)
                                    {
                                        var cururl = list1[i];
                                        var exsit = olds.FirstOrDefault(a => a.URL == cururl);
                                        if (exsit == null)
                                        {
                                            var img = new ShopProductImage()
                                            {
                                                CreateTime = DateTime.Now,
                                                ProductID = entity.ID,
                                                Sort = i + 1,
                                                URL = list1[i],
                                                ThumURL = list2[i]
                                            };
                                            db.ShopProductImages.Add(img);
                                        }
                                    }
                                    #endregion
                                    if (list1.Length > 0)
                                    {
                                        first.Image = list1[0];
                                    }
                                }
                                else
                                {
                                    json.IsSuccess = false;
                                }
                            }
                        }
                        #endregion
                        json.Msg = "修改";
                    }
                    db.SaveChanges();
                    json.IsSuccess = true;
                }
            }
            catch (Exception e)
            {
                json.IsSuccess = false;
                LogHelper.Error("商品添加失败：" + WebTools.getFinalException(e));
            }
            if (json.IsSuccess)
            {
                json.ReUrl = ControllerPath + "/Index";   //注册成功就跳转到 激活页
                json.Msg += "成功";
            }
            else
            {
                json.Msg += "失败";
            }
            return Json(json);
        }
        #endregion

        #region 删除
        public JsonResult Delete(string idList)
        {
            JsonHelp json = new JsonHelp() { Msg = "删除数据失败" };
            //是否为空
            if (string.IsNullOrEmpty(idList))
            {
                json.Msg = "未找到要删除的数据";
                return Json(json);
            }
            var ids = idList.TrimEnd(',').Split(',').Select(a => Convert.ToInt32(a)).ToList();
            if (DB.ShopProduct.Any(a => ids.Contains(a.ID)))
            {
                var names = DB.ShopProduct.Where(a => ids.Contains(a.ID)).Select(a => a.Title)
                    .ToList().Aggregate((m, n) => m + "," + n);
                var list = DB.ShopProduct.Where(a => ids.Contains(a.ID)).ToList();
                foreach (var item in list)
                {
                    item.IsNew = true;
                    DB.ShopProduct.Update(item);
                }

                json.IsSuccess = true;
                json.Msg = "删除数据成功";

                //添加操作日志
                DB.SysLogs.setMemberLog("Delete", "删除名字为[" + names + "]的产品 ");
            }
            else
            {
                json.Msg = "未找到要删除的对象，请刷新页面重试";
            }
            return Json(json);
        }
        #endregion

        #region 上架 下架商品
        public ActionResult SetState(string id, bool state)
        {
            JsonHelp json = new JsonHelp() { Msg = "操作失败" };
            var flag = "上架";
            if (state == false)
            {
                flag = "下架";
            }
            //是否为空
            if (string.IsNullOrEmpty(id))
            {
                json.Msg = "未找到要操作的数据";
                return Json(json);
            }
            var ids = id.TrimEnd(',').Split(',').Select(a => Convert.ToInt32(a)).ToList();
            if (DB.ShopProduct.Any(a => ids.Contains(a.ID)))
            {
                var names = DB.ShopProduct.Where(a => ids.Contains(a.ID)).Select(a => a.Title)
                    .ToList().Aggregate((m, x) => m + "," + x);
                if (DB.ShopProduct.Where(a => ids.Contains(a.ID)).Update(a => new ShopProduct() { IsEnable = state }) > 0)
                {
                    json.IsSuccess = true;
                    json.Msg = flag + " 商品成功";
                }
                //添加操作日志
                DB.SysLogs.setMemberLog("Edit", flag + "标题为[" + names + "]的商品 ");
            }
            else
            {
                json.Msg = "未找到要操作的商品，请刷新页面重试";
            }
            return Json(json);
        }
        #endregion
    }
}