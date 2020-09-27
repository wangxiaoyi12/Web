using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using DataBase;
using Business;
namespace System.Web
{
    /// <summary>
    /// 外部商城链接获取处理
    /// </summary>
    public class Url_Shop
    {
        public static string GetAction(string controller, string action, object id = null)
        {
            StringBuilder str = new StringBuilder(100);
            str.Append("/shop");
            if (string.IsNullOrEmpty(controller) == false)
                str.Append("/").Append(controller).Append("/");
            if (string.IsNullOrEmpty(action) == false)
                str.Append(action).Append("/");
            if (id != null)
                str.Append(id);
            return str.ToString();
        }
        public static string GetController(string controller)
        {
            return $"/shop/{controller}";
        }
        /// <summary>
        /// 获取首页
        /// </summary>
        /// <returns></returns>
        public static string GetIndex()
        {
            return "/shop";
        }
        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        public static string GetLogin()
        {
            return GetController("login");
        }
        /// <summary>
        /// 注册页面
        /// </summary>
        /// <returns></returns>
        public static string GetRegister()
        {
            return GetAction("login", "register");
        }
        /// <summary>
        /// 一元购首页
        /// </summary>
        /// <returns></returns>
        public static string GetOnebuy()
        {
            return GetController("onebuy");
        }

        #region 产品模块
        /// <summary>
        /// 产品首页
        /// </summary>
        /// <returns></returns>
        public static string GetProduct()
        {
            return GetController("product");
        }
        /// <summary>
        /// 产品首页,指定分类
        /// </summary>
        /// <returns></returns>
        public static string GetProduct(ShopProductCategory model)
        {
            return GetController("product") + "?classid=" + model.ID;
        }
        /// <summary>
        /// 产品首页,指定品牌
        /// </summary>
        /// <returns></returns>
        public static string GetProduct(ShopBrand model)
        {
            return GetController("product") + "?brandid=" + model.ID;
        }
        /// <summary>
        /// 产品详细页面
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetProduct(ShopProduct model)
        {
            return GetAction("product", "detail", model.ID);
        }
        /// <summary>
        /// 获取产品图片地址
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetProductImg(ShopProduct model, bool isBig = false)
        {
            var shopimg = DB.ShopProductImage.Where(a => a.ProductID == model.ID).ToList();
            if (shopimg.Count <= 0)
                return "/assets/shop/images/position.jpg";
            ShopProductImage first = shopimg.FirstOrDefault();
            if (isBig)
                return GetImg(first.URL);
            return GetImg(first.ThumURL);
        }
        /// <summary>
        /// 同一处理显示商品价格
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetProductPriceYuan(ShopProduct model)
        {
            //string curUserID = User_Shop.GetMemberID();
            //var member = DB.Member_Info.Where(p => p.MemberId == curUserID).FirstOrDefault();
            //if (member != null)
            //{
            //    var Level = DB.Sys_Level.Where(p => p.LevelId == member.MemberLevelId).FirstOrDefault();
            //    decimal? PriceShopping = Math.Round(Convert.ToDecimal(model.PriceShopping * Level.FindPointMoney / 10m), 2);
            //    //if (model.PriceShopping == 0)
            //    //{
            //    //    return model.PriceScore.ToString();
            //    //}
            //    return PriceShopping.ToString();
            //}
            //else
            //{
                return model.PriceOriginal.ToString();
            //}

        }
        public static string GetProductPrice(ShopProduct model)
        {
            string curUserID = User_Shop.GetMemberID();
            var member = DB.Member_Info.Where(p => p.MemberId == curUserID).FirstOrDefault();
            if (member.IsServiceCenter=="是")
            {
                return "零售价:￥" + model.PriceShopping + " 配货价:￥" + model.PriceScore;
            }else
            {
                return "零售价:￥" + model.PriceShopping + " 优惠价:￥" + model.PV;
            }
            //if (User_Shop.IsLogin())
            //{
            //    string curUserID = User_Shop.GetMemberID();
            //    var member = DB.Member_Info.Where(p => p.MemberId == curUserID).FirstOrDefault();
            //    if (member.MemberLevelId > 1)
            //    {
            //        var Level = DB.Sys_Level.Where(p => p.LevelId == member.MemberLevelId).FirstOrDefault();
            //        decimal? PriceShopping = Math.Round(Convert.ToDecimal(model.PriceShopping * Level.FindPointMoney / 10m), 2);
            //        //if (model.PriceShopping == 0)
            //        //{
            //        //    return model.PriceScore.ToString();
            //        //}
            //        return PriceShopping.ToString();
            //    }
            //    else
            //    {
            //        return model.PriceShopping.ToString();
            //    }

            //}
            //else
            //{
            //return model.PriceShopping.ToString();
            //}
        }
        public static string GetProductPriceJiaG(ShopProduct model)
        {
            //if (model.PriceShopping == 0)
            //{
            //    return model.PriceScore.ToString();
            //}
            return model.PriceShopping.ToString();
        }
        public static string GetProductPriceXianJin(ShopProduct model)
        {
            //if (model.PriceShopping == 0)
            //{
            //    return model.PriceScore.ToString();
            //}
            return model.PV.ToString();
        }
        public static string GetProductPriceGouWu(ShopProduct model)
        {
            //if (model.PriceShopping == 0)
            //{
            //    return model.PriceScore.ToString();
            //}
            return model.PriceScore.ToString();
        }


        public static string GetProductPriceStr1(ShopOrderProduct model, ShopProduct product)
        {
            return $"<span class='price' id='prodCash'>{model.MoneyCongXiao}</span>元; 运费:<span class='price' id='prodCash'>{product.Postage}</span>元";

        }
        public static string GetProductXianJin1(ShopOrderProduct model,ShopProduct product)
        {
            return $"<span class='price' id='prodCash'>{model.MoneyScore}</span>元; 运费:<span class='price' id='prodCash'>{product.Postage}</span>元";

        }
        public static string GetProductShopping(ShopOrderProduct model, ShopProduct product)
        {
            return $"<span class='price' id='prodCash'>{model.MoneyShopping}</span>积分; 运费:<span class='price' id='prodCash'>{product.Postage}</span>积分";

        }
        /// <summary>
        /// 同一处理显示商品价格字符串
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetProductPriceStr(ShopProduct model,string guige)
        {
            return $"<span class='price' id='prodCash'>{DB.ShopProduct.GetSPrice(model,guige).PeiHuo}</span>元; 运费:<span class='price' id='prodCash'>{model.Postage}</span>元";

        }
        public static string GetProductXianJin(ShopProduct model,string guige)
        {
            return $"<span class='price' id='prodCash'>{DB.ShopProduct.GetSPrice(model, guige).YouHui}</span>元; 运费:<span class='price' id='prodCash'>{model.Postage}</span>元";

        }
        public static string GetProductLingShou(ShopProduct model, string guige)
        {
            return $"<span class='price' id='prodCash'>{DB.ShopProduct.GetSPrice(model, guige).LingShou}</span>积分; 运费:<span class='price' id='prodCash'>{model.Postage}</span>积分";

        }
        public static string GetProductGouWuJ(ShopProduct model)
        {
            return $"<span class='price' id='prodCash'>{model.PriceScore}</span>配货价";

        }
        public static string GetProductPriceStrJiF(ShopProduct model)
        {
            return $"<span class='price' id='prodCash'>{model.PV}</span>积分";

        }
        ///// <summary>
        ///// 同一处理显示商品价格字符串2
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public static string GetProductPriceStr2(ShopProduct model)
        //{
        //    return $"<span class='price'>{(model.IsLvLiu() ? "¥" : "¥")}<span class='price' id='prodCash'>{GetProductPrice(model)}</span></span>";
        //}

        /// <summary>
        /// 同一处理显示商品价格
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static decimal GetMemberPrice(ShopProduct model)
        {
            //if (User_Shop.IsLogin())
            //{
            //    Member_Info curUser = User_Shop.GetMember_Info();
            //    if (curUser.IsVip())
            //    {
            //        return model.PriceShopping;
            //    }
            //}
            return model.PriceShopping;
        }
        #endregion

        #region 个人中心
        /// <summary>
        /// 个人中心首页
        /// </summary>
        /// <returns></returns>
        public static string GetUserCenter()
        {
            return GetController("usercenter");
        }
        /// <summary>
        /// 我的订单
        /// </summary>
        /// <returns></returns>
        public static string GetMyOrder()
        {
            return GetAction("usercenter", "orderform");
        }
        /// <summary>
        /// 我的购物车
        /// </summary>
        /// <returns></returns>
        public static string GetMyCat()
        {
            return GetAction("usercenter", "cat");
        }
        /// <summary>
        /// 确认订单页面
        /// </summary>
        /// <returns></returns>
        public static string GetMyPayment()
        {
            return GetAction("usercenter", "payment");
        }
        /// <summary>
        /// 我的足记
        /// </summary>
        /// <returns></returns>
        public static string GetMyFootprint()
        {
            return GetAction("usercenter", "footprint");
        }
        /// <summary>
        /// 我的收藏
        /// </summary>
        /// <returns></returns>
        public static string GetMyCollect()
        {
            return GetAction("usercenter", "collect");
        }
        /// <summary>
        /// 我的收货地址
        /// </summary>
        /// <returns></returns>
        public static string GetMyAddress()
        {
            return GetAction("usercenter", "address");
        }
        /// <summary>
        /// 一元购
        /// </summary>
        /// <returns></returns>
        public static string GetMyOnebuy()
        {
            return GetAction("usercenter", "onebuy");
        }
        #endregion

        #region 帮助中心
        /// <summary>
        /// 帮助首页
        /// </summary>
        /// <returns></returns>
        public static string GetHelp()
        {
            return GetController("help");
        }
        /// <summary>
        /// 帮助详细页
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetHelp(ShopArticleCategory model)
        {
            return GetAction("help", model.ID.ToString());
        }
        #endregion

        public static string GetImg(string url)
        {
            if (string.IsNullOrEmpty(url))
                return "/assets/shop/images/position.jpg";
            return url;
        }
        /// <summary>
        /// 获取商城配置参数
        /// </summary>
        /// <returns></returns>
        public static Xml_Shop GetConfig()
        {
            return DB.XmlConfig.XmlShop;
        }
        /// <summary>
        /// 获取商城logo图片
        /// </summary>
        /// <returns></returns>
        public static string GetLogo()
        {
            string logo = GetConfig().ShopLogo;
            if (string.IsNullOrEmpty(logo))
                return "/assets/shop/images/logo.png";
            return logo;
        }

        //获取跳转链接
        public static string GetRecirectUrl()
        {
            string url = ReqHelper.GetString("redirecturl");
            if (string.IsNullOrEmpty(url))
            {
                url = Common.CookieHelper.GetCookieValue("redirecturl");
            }
            return url;
        }
    }
}