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
    /// 用户中心模块
    /// </summary>
    [ShopUserCheck()]
    public class UserCenterController : ShopBaseController
    {
        public string CurrentUserID
        {
            get
            {
                return User_Shop.GetMemberID();
            }
        }
        public Member_Info CurrentUser
        {
            get
            {
                return User_Shop.GetMember_Info();
            }
        }
        /// <summary>
        /// 个人中心首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 购物车
        /// </summary>
        /// <returns></returns>
        public ActionResult Cat()
        {
            return View();
        }

        /// <summary>
        /// 订单确认页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Payment()
        {
            return View();
        }
        /// <summary>
        /// 购物结算处理
        /// </summary>
        /// <returns></returns>
        public ActionResult Calculate()
        {
            return View();
        }
        /// <summary>
        /// 订单列表
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderForm()
        {
            return View();
        }
        /// <summary>
        /// 一元购记录
        /// </summary>
        /// <returns></returns>
        public ActionResult OneBuy()
        {
            return View();
        }
        /// <summary>
        /// 订单地址
        /// </summary>
        /// <returns></returns>
        public ActionResult Address(int? id)
        {
            ViewBag.id = id;
            return View();
        }
        /// <summary>
        /// 浏览记录
        /// </summary>
        /// <returns></returns>
        public ActionResult Footprint()
        {
            return View();
        }
        /// <summary>
        /// 用户收藏
        /// </summary>
        /// <returns></returns>
        public ActionResult Collect()
        {
            return View();
        }


        #region 用户流程操作
        //删除订单
        public JsonResult DeleteForm(string orderid)
        {
            try
            {
                ShopOrder order = DB.ShopOrder.FindEntity(orderid);
                if (order.MemberID != CurrentUser.MemberId)
                    throw new Exception("权限验证失败");
                order.State = ShopEnum.OrderState.Cancel.GetHashCode();
                DB.ShopOrder.Update(order);
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        /// <summary>
        /// 用户提交订单处理
        /// </summary>
        /// <returns></returns>
        public JsonResult SubmitForm(int AddressID, string remark)
        {
            try
            {
                if (DB.ShopOrder.GenerateOrder(CurrentUser, AddressID, remark))
                    return Success("提交成功");
                return Error("提交失败");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        /// <summary>
        /// 结算用户的为付款订单
        /// </summary>
        /// <returns></returns>
        public ActionResult CalcuteForm(string orderid)
        {
            try
            {
                if (DB.ShopOrder.Calcute(CurrentUser, orderid,""))
                {
                    return Success("结算成功");
                }
                return Error("结算失败");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        public ActionResult PayCenter(string orderid, int paytype)
        {
            string url = null;
            //if (DB.ShopOrder.Calcute(CurrentUser, orderid, paytype, (order) =>
            //{
            //    decimal amount = paytype == ShopEnum.PayType.全额现金.GetHashCode() ? order.RealShopping : order.RealCongXiao;
            //    ShopProduct product = DB.ShopOrderProduct.Where(q => q.OrderID == order.GUID).First().ShopProduct;
            //    url = PayHelper.GetPayUrl(order.GUID, amount, product.Title);
            //}))
            //{
            //    return Redirect(url);
            //}
            return Content("支付接口待完善");
        }
        /// <summary>
        /// 标记订单，已收货
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public JsonResult FirmOrder(string OrderID)
        {
            try
            {
                ShopOrder model = DB.ShopOrder.FindEntity(q => q.GUID == OrderID);
                model.State = ShopEnum.OrderState.Finish.GetHashCode();
                if (DB.ShopOrder.Update(model))
                {
                    return Success("操作成功");
                }
                return Error("操作失败");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        /// <summary>
        /// 标记订单，提货
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public JsonResult TiHuoOrder(string OrderID)
        {
            try
            {
                ShopOrder model = DB.ShopOrder.FindEntity(q => q.GUID == OrderID);
                model.State = ShopEnum.OrderState.Pay.GetHashCode();
                model.Type = "提货";
                if (DB.ShopOrder.Update(model))
                {
                    return Success("操作成功");
                }
                return Error("操作失败");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        /// <summary>
        /// 标记订单，委托
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public JsonResult WeiTuoOrder(string OrderID)
        {
            try
            {
                ShopOrder model = DB.ShopOrder.FindEntity(q => q.GUID == OrderID);
                //model.State = ShopEnum.OrderState.WeiTuo.GetHashCode();
                #region 委托
                //给零售区加库存
                //var lingshouqu = DB.ShopOrderProduct.Where(p => p.OrderID == model.GUID).FirstOrDefault();
                //var shopOrderProudct = DB.ShopProduct.Where(p => p.Title == lingshouqu.Name && p.CategoryID1 == 15).FirstOrDefault();
                //shopOrderProudct.Inventory = Convert.ToInt32(shopOrderProudct.Inventory + model.ZongDay);
                //DB.ShopProduct.Update(shopOrderProudct);
                model.State = ShopEnum.OrderState.GuaM.GetHashCode();
                model.Type = "挂卖中";
                model.SubmitTime = DateTime.Now;//委托时间
                //DB.ShopOrder.Update(model);
                #endregion
                if (DB.ShopOrder.Update(model))
                {
                    return Success("操作成功");
                }
                return Error("操作失败");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        #endregion


        #region 用户操作统一处理
        /// <summary>
        /// 添加购物车
        /// </summary>
        /// <param name="ProductID">产品ID</param>
        /// <param name="Count">产品数量</param>
        /// <returns></returns>
        public JsonResult InsertCat(int ProductID, int Count,string guige)
        {
            try
            {
                ShopProduct product = DB.ShopProduct.FindEntity(ProductID);
                var category = DB.ShopProduct.GetCategoryId2(product.CategoryID.Value);
                var guigeproduct = DB.GuiGeName.Where(a => a.CName == category.Name).Count();

                if(product.CategoryID1.Value==DB.XmlConfig.XmlSite.Scores && Count!=1)
                {
                    throw new Exception("拼团专区只能买一个");
                }


                string[] strlist = guige.Split('_');

                if (strlist.Length < guigeproduct)
                {
                    throw new Exception("请选择完整规格");
                }

                var gproduct = DB.ShopProduct.GetSPrice(product, guige);
                if (gproduct == null)
                {
                    throw new Exception("库存不足下单失败");
                }

                //判断购物车中是否都是同一个大类中的产品
                ShopCat fistPro = DB.ShopCat.Where(q => q.MemberID == CurrentUserID).FirstOrDefault();
                if (fistPro != null)
                {
                    //if (fistPro.ShopProduct.CategoryID1 != product.CategoryID1)
                    //    return Error("在一次下单中，只能结算同一类商品");
                    if (fistPro.ShopProduct.ID != product.ID && fistPro.ShopProduct.CategoryID1==DB.XmlConfig.XmlSite.Scores)
                        return Error("在一次下单中，拼团专区只能加入同一种购物车商品");
                }

                //如果是订单商品，判断用户是否是注册30天之内，否则不允许再次下单
                //if (product.IsDingDan())
                //{
                //    if (CurrentUser.MemberLevelId > 0)//会员用户
                //    {
                //        //规定等级的订单商品不能
                //        var levels = DB.Sys_Level.ToList();
                //        if (levels.Any(q => q.Investment == product.PriceShopping))
                //            return Error("会员用户只能购买升级商品");
                //    }
                //    else
                //    {//粉丝用户
                //        var levels = DB.Sys_Level.ToList();
                //        if (levels.Any(q => q.Investment == product.PriceShopping) == false)
                //            return Error("粉丝用户只能购买订单商品");
                //    }
                //}


                if (Count > gproduct.KuCun)
                    throw new Exception("库存不足");
                //判断用户的产品是否存在，存在购买数量++
                if (DB.ShopCat.Any(q => q.ProductID == ProductID && q.GuiGe == guige && q.MemberID == CurrentUserID))
                {
                    ShopCat model = DB.ShopCat.FindEntity(q => q.ProductID == ProductID && q.MemberID == CurrentUserID);
                    if (product.CategoryID1 == DB.XmlConfig.XmlSite.Scores)
                    {
                        model.Count = 1;
                    }
                    else
                    {
                        model.Count += Count;
                    }
                    if (model.Count > gproduct.KuCun)
                        throw new Exception("库存不足");
                    if (DB.ShopCat.Update(model))
                    {
                        return Success("添加购物车成功");
                    }
                    return Error("添加购物车失败");
                }
                else
                {
                    ShopCat cat = new ShopCat();
                    cat.ProductID = ProductID;
                    cat.Count = Count;
                    cat.GuiGe = guige;
                    cat.MemberID = CurrentUserID;
                    cat.CreateTime = DateTime.Now;
                    if (DB.ShopCat.Insert(cat))
                    {
                        return Success("添加购物车成功");
                    }
                    return Error("添加购物车失败");
                }


            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        /// <summary>
        /// 删除购物车的商品
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        public JsonResult DeleteCat(int ProductID)
        {
            try
            {
                if (DB.ShopCat.Delete(q => q.ID == ProductID) > 0)
                    return Success("删除成功");
                return Error("删除失败");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }


        /// <summary>
        /// 添加收藏产品
        /// </summary>
        /// <param name="ProductID">产品ID</param>
        /// <returns></returns>
        public JsonResult InsertCollect(int ProductID)
        {
            try
            {
                //判断是否已经收藏
                if (DB.ShopCollectionProduct.Any(q => q.ProductID == ProductID))
                    return Error("已经收藏");
                //判断当前用户搜藏总数
                if (DB.ShopCollectionProduct.Count(q => q.MemberID == CurrentUserID) > 100)
                    return Error("您的收藏总数已经超过100个商品，如果想继续收藏，请删除旧的收藏记录");
                ShopCollectionProduct model = new ShopCollectionProduct();
                model.ProductID = ProductID;
                model.CreateTime = DateTime.Now;
                model.MemberID = CurrentUserID;
                if (DB.ShopCollectionProduct.Insert(model))
                    return Success("收藏成功");
                return Error("收藏失败");
            }
            catch (Exception ex)
            {
                return Error(ex); ;
            }
        }
        /// <summary>
        /// 删除收藏记录
        /// </summary>
        /// <param name="ID">记录ID</param>
        /// <returns></returns>
        public JsonResult DeleteCollect(int ID)
        {
            try
            {
                if (DB.ShopCollectionProduct.Delete(q => q.ID == ID) > 0)
                    return Success("删除成功");
                return Error("删除失败");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        /// <summary>
        /// 添加或修改收货地址
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult AddAddress(ShopMyAddress model)
        {
            try
            {
                //修改省市县内容
                if (model.ProvId > 0)
                    model.ProvinceName = DB.Area_Province.FindEntity(q => q.id == model.ProvId).name;
                if (model.CityID > 0)
                    model.CityName = DB.Area_City.FindEntity(q => q.id == model.CityID.Value).name;
                if (model.CountyID > 0)
                    model.CountyName = DB.Area_County.FindEntity(q => q.id == model.CountyID).name;
                if (model.ID > 0)
                {
                    //修改
                    ShopMyAddress oldModel = DB.ShopMyAddress.FindEntity(q => q.ID == model.ID);
                    oldModel.Name = model.Name;
                    oldModel.Tel = model.Tel;
                    oldModel.PostCode = model.PostCode;
                    oldModel.Address = model.Address;
                    oldModel.ProvId = model.ProvId;
                    oldModel.ProvinceName = model.ProvinceName;
                    oldModel.CityID = model.CityID;
                    oldModel.CityName = model.CityName;
                    oldModel.CountyID = model.CountyID;
                    oldModel.CountyName = model.CountyName;
                    if (DB.ShopMyAddress.Update(oldModel) == false)
                        throw new Exception("修改失败");
                    return Success("修改成功");
                }
                else
                {
                    //添加
                    model.CreateTime = DateTime.Now;
                    model.MemberID = CurrentUserID;
                    if (DB.ShopMyAddress.Insert(model) == false)
                        throw new Exception("添加收货地址失败");
                    return Success("添加成功");
                }
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        /// <summary>
        /// 删除收货地址
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public JsonResult DeleteAddress(int ID)
        {
            try
            {
                if (DB.ShopMyAddress.Delete(q => q.ID == ID) > 0)
                    return Success("删除成功");
                return Error("删除失败");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        /// <summary>
        /// 设置默认地址
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public JsonResult DefaultAddress(int ID)
        {
            try
            {
                DB.ShopMyAddress.SetDefault(ID, CurrentUserID);
                return Success("设置成功");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        #endregion

        #region 用户一元购操作
        /// <summary>
        /// 添加一元购记录
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public JsonResult AddOneBuy(int ID, int Count)
        {
            try
            {
                Member_Info curUser = CurrentUser;
                ShopOneBuy model = DB.ShopOneBuy.FindEntity(q => q.ID == ID);
                //没人购买数量不能超过10%
                if (model.TotalCount > 10)
                {
                    //1.获取当前人的购买数量
                    int buyCount = DB.ShopOneBuyDetail.Where(q => q.MemberID == CurrentUser.MemberId && q.OneBuyID == ID).Select(q => q.Count)
                        .ToList().Sum();
                    double max = model.TotalCount * 0.1;
                    if (buyCount > max)
                        throw new Exception("当前商品一个人最多购买总数的10%");
                }
                //1.数量验证
                int span = model.TotalCount - model.Count;
                if (Count > span)
                {
                    throw new Exception("购买数量超出");
                }
                if (curUser.Coins == null || curUser.Coins < Count)
                    throw new Exception("余额不足");
                //如果购买超过总数量标记已完成
                if ((model.Count + Count) >= model.TotalCount)
                    model.State = ShopEnum.OneBuyState.Finish.GetHashCode();

                //修改数量
                using (var tran = DB.ShopOneBuy.BeginTransaction)
                {
                    try
                    {
                        model.Count += Count;
                        DB.ShopOneBuy.Update(model);
                        //2.添加记录
                        ShopOneBuyDetail detail = new ShopOneBuyDetail();
                        detail.OneBuyID = ID;
                        detail.Count = Count;
                        detail.CreateTime = DateTime.Now;
                        detail.MemberID = curUser.MemberId;
                        detail.MemberCode = curUser.Code;
                        detail.NickName = curUser.NickName;

                        if (DB.ShopOneBuyDetail.Insert(detail))
                        {
                            //扣除用户的钱
                            curUser.Coins -= Count;
                            DB.Member_Info.Update(curUser);

                            tran.Complete();
                            return Success("购买成功");
                        }
                        return Error("购买失败");
                    }
                    catch (Exception ex)
                    {
                        DB.Rollback();
                        throw ex;
                    }
                }

            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        #endregion

        /// <summary>
        /// 左侧菜单
        /// </summary>
        /// <returns></returns>
        public PartialViewResult LeftMenu()
        {
            return PartialView();
        }
    }
}