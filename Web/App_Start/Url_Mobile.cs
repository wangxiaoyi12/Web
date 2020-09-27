using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace System.Web
{
    /// <summary>
    /// 手机版商城链接处理
    /// </summary>
    public class Url_Mobile
    {
        private static HttpRequest req
        {
            get
            {
                if (HttpContext.Current != null)
                    return HttpContext.Current.Request;
                return null;
            }
        }
        /// <summary>
        /// 判断是否是自营商城
        /// </summary>
        /// <returns></returns>
        public static bool IsSelf()
        {
            //判断请求链接中是否含有self
            //string path = ReqHelper.req.Url.AbsolutePath.ToLower();
            //return path.Contains("self");
            string controller = ReqHelper.GetStringRoute("controller");
            return controller.ToLower().Contains("self");
        }

        /// <summary>
        /// 判断是否是微信客户端
        /// </summary>
        /// <returns></returns>
        public static bool IsWechat()
        {
            string param = req.ServerVariables["HTTP_USER_AGENT"];
            if (string.IsNullOrEmpty(param) == false)
            {
                return param.ToLower().Contains("MicroMessenger".ToLower());
            }
            return false;
        }
        /// <summary>
        /// 判断是否是手机浏览器
        /// </summary>
        /// <returns></returns>
        public static bool IsMobile()
        {
            string userAgent = req.Headers["User-Agent"];
            if (string.IsNullOrEmpty(userAgent))
                return false;
            if (userAgent.IndexOf("Noki") > -1 || // Nokia phones and emulators     
                     userAgent.IndexOf("Android") > -1 ||   //Android 手机
                       userAgent.IndexOf("iPhone") > -1 ||  //iPhone  手机
                     userAgent.IndexOf("Eric") > -1 || // Ericsson WAP phones and emulators     
                     userAgent.IndexOf("WapI") > -1 || // Ericsson WapIDE 2.0     
                     userAgent.IndexOf("MC21") > -1 || // Ericsson MC218     
                     userAgent.IndexOf("AUR") > -1 || // Ericsson R320     
                     userAgent.IndexOf("R380") > -1 || // Ericsson R380     
                     userAgent.IndexOf("UP.B") > -1 || // UP.Browser     
                     userAgent.IndexOf("WinW") > -1 || // WinWAP browser     
                     userAgent.IndexOf("UPG1") > -1 || // UP.SDK 4.0     
                     userAgent.IndexOf("upsi") > -1 || //another kind of UP.Browser     
                     userAgent.IndexOf("QWAP") > -1 || // unknown QWAPPER browser     
                     userAgent.IndexOf("Jigs") > -1 || // unknown JigSaw browser     
                     userAgent.IndexOf("Java") > -1 || // unknown Java based browser     
                     userAgent.IndexOf("Alca") > -1 || // unknown Alcatel-BE3 browser (UP based)    


                     userAgent.IndexOf("MITS") > -1 || // unknown Mitsubishi browser     
                     userAgent.IndexOf("MOT-") > -1 || // unknown browser (UP based)     
                     userAgent.IndexOf("My S") > -1 ||//  unknown Ericsson devkit browser      
                     userAgent.IndexOf("WAPJ") > -1 ||//Virtual WAPJAG www.wapjag.de     
                     userAgent.IndexOf("fetc") > -1 ||//fetchpage.cgi Perl script from www.wapcab.de 


                     userAgent.IndexOf("ALAV") > -1 || //yet another unknown UP based browser     
                     userAgent.IndexOf("Wapa") > -1 || //another unknown browser (Web based "Wapalyzer")    
                     userAgent.IndexOf("UCWEB") > -1 || //another unknown browser (Web based "Wapalyzer")    
                     userAgent.IndexOf("BlackBerry") > -1 || //another unknown browser (Web based "Wapalyzer")                     
                     userAgent.IndexOf("J2ME") > -1 || //another unknown browser (Web based "Wapalyzer")              
                     userAgent.IndexOf("Oper") > -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GetAction(string controller, string action, object id = null)
        {
            StringBuilder str = new StringBuilder(100);
            str.Append("/mobile");
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
            return $"/mobile/{controller}";
        }
        public static string GetAction2(string controller, string action, object id = null)
        {
            //if (IsSelf())
            //{
            //    controller = "self" + controller;
            //}
            //else
            //{
            //    controller = "union" + controller;
            //}
            controller = "self" + controller;
            return GetAction(controller, action, id);
        }
        public static string GetController2(string controller)
        {
            //if (IsSelf())
            //{
            //    controller = "self" + controller;
            //}
            //else
            //{
            //    controller = "union" + controller;
            //}
            controller = "self" + controller;
            return GetController(controller);
        }

        #region 首页
        /// <summary>
        /// 获取首页
        /// </summary>
        /// <returns></returns>
        public static string GetIndex()
        {
            return "/mobile";
        }

        #endregion

        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        public static string GetLogin()
        {
            return GetController("memberlogin");
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
        /// 获取js，css版本
        /// </summary>
        /// <returns></returns>
        public static string GetVersion()
        {
            return "1.3";
        }
        #region 产品模块
        /// <summary>
        /// 产品首页
        /// </summary>
        /// <returns></returns>
        public static string GetProduct()
        {
            return GetController2("product");
        }
        /// <summary>
        /// 产品首页,指定分类
        /// </summary>
        /// <returns></returns>
        public static string GetProduct(ShopProductCategory model)
        {
            return GetController2("product") + "?classid=" + model.ID;
        }
        /// <summary>
        /// 产品首页,指定品牌
        /// </summary>
        /// <returns></returns>
        public static string GetProduct(ShopBrand model)
        {
            return GetController2("product") + "?brandid=" + model.ID;
        }
        /// <summary>
        /// 产品详细页面
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetProduct(ShopProduct model)
        {
            return GetAction("selfproduct", "detail", model.ID);

            //判断商品是否是自营商城
            //if (model.ShopID == 1)
            //{
            //    return GetAction("selfproduct", "selfdetail", model.ID);
            //}
            //return GetAction("unionproduct", "uniondetail", model.ID);
        }

        #endregion


        #region 分类，品牌
        /// <summary>
        /// 品牌首页
        /// </summary>
        /// <returns></returns>
        public static string GetBrand()
        {
            return GetController2("brand");
        }
        public static string GetShangJ(int id)
        {
            return GetController2("cate") + "?ShopID=" + id;
        }
        /// <summary>
        /// 分类首页
        /// </summary>
        /// <returns></returns>
        public static string GetCategory()
        {
            return GetController2("cate");
        }
        #endregion

        #region 个人中心
        /// <summary>
        /// 个人中心首页
        /// </summary>
        /// <returns></returns>
        public static string GetUserCenter()
        {
            return GetController("mobilecenter");
        }
        /// <summary>
        /// 我的订单
        /// </summary>
        /// <returns></returns>
        public static string GetMyOrder()
        {
            return GetAction("mobilecenter", "orderform");
        }
        /// <summary>
        /// 我的购物车
        /// </summary>
        /// <returns></returns>
        public static string GetMyCart()
        {
            return GetAction("mobilecenter", "cart");
        }
        /// <summary>
        /// 确认订单页面
        /// </summary>
        /// <returns></returns>
        public static string GetMyPayment()
        {
            return GetAction("mobilecenter", "payment", new { q = "t" });
        }
        /// <summary>
        /// 我的足记
        /// </summary>
        /// <returns></returns>
        public static string GetMyFootprint()
        {
            return GetAction("mobilecenter", "footprint");
        }
        /// <summary>
        /// 我的收藏
        /// </summary>
        /// <returns></returns>
        public static string GetMyCollect()
        {
            return GetAction("mobilecenter", "collect");
        }
        /// <summary>
        /// 我的收货地址
        /// </summary>
        /// <returns></returns>
        public static string GetMyAddress()
        {
            return GetAction("mobilecenter", "address");
        }
        ///// <summary>
        ///// 一元购
        ///// </summary>
        ///// <returns></returns>
        //public static string GetMyOnebuy()
        //{
        //    return GetAction("mobilecenter", "onebuy");
        //}
        #endregion

        public static string GetImg(string url)
        {
            if (string.IsNullOrEmpty(url))
                //return "/assets/shop/images/position.jpg";
                return "/assets/mobile/images/noavatar.png";
                return url;
        }

        public static string GetTouXiang(string url)
        {
            if (string.IsNullOrEmpty(url))
                return "/assets/mobile/images/position.png";
            return url;
        }
        //获取会员的推广链接
        public static string GetQrCodeLink(Member_Info model)
        {
            string code = Common.CryptHelper.DESCrypt.Encrypt(model.MemberId);
            string path = GetRegister() + $"?code={code}";
            return $"http://" + req.Url.Host + path;
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
    }
}