using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DataBase;
using Business;
using Common;
namespace System.Web
{
    /// <summary>
    /// 商城当前用户操作
    /// </summary>
    public class User_Shop
    {

        #region 获取当前用户信息
        /// <summary>
        /// 判断是否已经登录
        /// </summary>
        /// <returns></returns>
        public static bool IsLogin()
        {
            dynamic value = GetAccount();
            if (value == null)
                return false;
            return true;
        }
        /// <summary>
        /// 判断登录用户是否是已经收藏过指定产品
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        public static bool IsCollect(int ProductID)
        {
            if (IsLogin())
            {
                string id = GetMemberID();
                if (DB.ShopCollectionProduct.Any(q => q.ProductID == ProductID && q.MemberID == id))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 获取登录用户对象
        /// </summary>
        /// <returns></returns>
        public static Member_Info GetMember_Info()
        {
            if (IsLogin() == false)
                return null;
            string id = GetMemberID();
            return DB.Member_Info.FindEntity(q => q.MemberId == id);
        }
        /// <summary>
        /// 获取会员ID
        /// </summary>
        /// <returns></returns>
        public static string GetMemberID()
        {
            try
            {
                //string[] temp = GetCookie().Split('&');
                //return temp[0];
                return GetAccount().Id;
            }
            catch (Exception ex)
            {
                throw new HttpException(599, "获取商城用户失败," + ex.Message);
            }
        }
        /// <summary>
        /// 获取登录名
        /// </summary>
        /// <returns></returns>
        public static string GetLoginName()
        {
            try
            {
                return GetAccount().LoginName;
            }
            catch (Exception ex)
            {
                throw new HttpException(599, "获取商城用户失败," + ex.Message);
            }
        }
        ///// <summary>
        ///// 获取登录名
        ///// </summary>
        ///// <returns></returns>
        //public static int GetRoleID()
        //{
        //    try
        //    {
        //        string[] temp = GetCookie().Split('&');
        //        return Convert.ToInt32(temp[3]);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw new HttpException(599, "获取商城用户失败," + ex.Message);
        //    }
        //}


        /// <summary>
        /// 获取当前用户的cookie
        /// </summary>
        /// <returns></returns>
        public static string GetCookie()
        {
            return Tools.getCookie(Enums.LoginType.member.ToString());
        }
        public static Account GetAccount()
        {
            string value = GetCookie();
            if (string.IsNullOrEmpty(value))
                return null;
            return value.JsonDeserializer<Account>();
        }
        //public static dynamic GetAccount()
        //{
        //    string value = GetCookie();
        //    if (string.IsNullOrEmpty(value))
        //        return null;
        //   return Common.JsonConverter.ConvertJson(value)
        //    //return value.JsonDeserializer<Account>();
        //}
        #endregion


        #region 客户端信息操作
        /// <summary>
        /// 保存用户信息到客户端
        /// </summary>
        /// <param name="model"></param>
        public static void SetUser(Member_Info model)
        {

            Tools.WriteCookie(model, false);

            //保存微信客户端信息
            if (string.IsNullOrEmpty(model.OpenID) == false)
                CookieHelper.SetCookie("_token_", model.OpenID, 365 * 3);
        }
        /// <summary>
        /// 删除商城会员
        /// </summary>
        public static void Clear()
        {
            CookieHelper.ClearCookie("_token_");
            CookieHelper.ClearCookie(Enums.LoginType.member.ToString());
            CookieHelper.ClearCookie("cart");
            CookieHelper.ClearCookie("total");
            CookieHelper.ClearCookie("openid");
            CookieHelper.ClearCookie("recirecturl");
            CookieHelper.ClearCookie("headimgurl");
            CookieHelper.ClearCookie("nickname");
        }
        #endregion


        #region 购物相关
        ///// <summary>
        ///// 判断登录用户是否是已经收藏过指定产品
        ///// </summary>
        ///// <param name="ProductID"></param>
        ///// <returns></returns>
        //public static bool IsCollect(int ProductID)
        //{
        //    if (IsLogin())
        //    {
        //        string id = GetMemberID();
        //        if (DB.ShopCollectionProduct.Any(q => q.ProductID == ProductID && q.MemberID == id))
        //            return true;
        //    }
        //    return false;
        //}
        /// <summary>
        /// 获取当前登录会员的购物车数量
        /// </summary>
        /// <returns></returns>
        public static int GetCartCount()
        {
            if (IsLogin() == false)
                return 0;
            Member_Info model = GetMember_Info();
            return DB.ShopCat.Where(q => q.MemberID == model.MemberId).Count();
        }

        #region 添加OpenID 相关
        /// <summary>
        /// 获取openid
        /// </summary>
        /// <returns></returns>
        public static string GetOpenID()
        {
            return CookieHelper.GetCookieValue("_token_");
        }
        #endregion
        #endregion
    }
}