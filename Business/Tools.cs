using Common;
using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business
{
    public class Tools
    {
        #region cookie读写
        public static void WriteCookie(Sys_Employee user)
        {
            string cookievalue = "{";
            cookievalue += string.Format("\"id\":\"{0}\",\"loginname\":\"{1}\",\"username\":\"{2}\",\"password\":\"{3}\",\"logintype\":\"{4}\",\"roleid\":\"{5}\",\"token\":\"{6}\"", user.EmpId, user.LoginName, user.RealName, user.Password, Enums.LoginType.admin.ToString(), user.RoleId, DB.ValidCookieString);
            cookievalue += "}";
            CookieHelper.SetCookie(Enums.LoginType.admin.ToString(), Common.CryptHelper.DESCrypt.Encrypt(cookievalue), null);
        }
        public static void WriteCookie(Member_Info user, bool isAdmin = false)
        {
            var FuWu = 1;
            //判断是否是商家
            //var IsShop = DB.Shop.FindEntity(q => q.MemberID == user.MemberId);
            if (user.IsServiceCenter == "是")
            {
                FuWu = 35;
            }
            //else if (IsShop == null || user.IsServiceCenter == "否")
            //{
            //    FuWu = 1;
            //}
            //else if (user.IsServiceCenter == "是" || IsShop.IsCheck == true)
            //{
            //    FuWu = 35;
            //}
            string cookievalue = "{";
            cookievalue += string.Format("\"id\":\"{0}\",\"loginname\":\"{1}\",\"username\":\"{2}\",\"password\":\"{3}\",\"logintype\":\"{4}\",\"roleid\":\"{5}\",\"pwd2\":\"{6}\",\"token\":\"{7}\",\"isadmin\":\"{8}\"",
                user.MemberId,
                user.Code,
                user.NickName,
                user.LoginPwd,
                Enums.LoginType.member.ToString(), FuWu,
                user.Pwd2, DB.ValidCookieString,
                isAdmin);
            cookievalue += "}";
            CookieHelper.SetCookie(Enums.LoginType.member.ToString(), Common.CryptHelper.DESCrypt.Encrypt(cookievalue), null);
        }
        /// <summary>
        /// 设置cookie,自动加密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="days"></param>
        public static void setCookie(string key, string value, int? days = null)
        {
            CookieHelper.SetCookie(key, Common.CryptHelper.DESCrypt.Encrypt(value), days);
        }
        /// <summary>
        /// 获取加密cookie的自动解密后的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string getCookie(string key)
        {
            var cookie = CookieHelper.GetCookie(key);
            if (cookie != null)
            {
                return Common.CryptHelper.DESCrypt.Decrypt(cookie.Value);
            }
            return null;
        }
        #endregion

        #region 十进制与N进制转换
        /// <summary>
        /// N进制字符串转10进制
        /// </summary>
        /// <param name="s">n进制字符串</param>
        /// <param name="n">n进制</param>
        /// <returns></returns>
        public static int ConvertNTo10(string s, int n)
        {
            var list = s.ToCharArray();
            var r = 0;
            for (int i = list.Length - 1; i >= 0; i--)
            {
                var x = list[i].ToString();
                r += Convert.ToInt32(Math.Pow(n, list.Length - i - 1)) * Convert.ToInt32(x);
            }
            return r;
        }

        /// <summary>
        /// 10进制转n进制字符串
        /// </summary>
        /// <param name="value">10进制的值</param>
        /// <param name="n">n进制</param>
        /// <param name="layer">层数</param>
        /// <returns></returns>
        public static string Convert10ToN(int value, int n, int layer)
        {
            var r = string.Empty;
            var cur = value;
            List<int> list = new List<int>();
            if (value < n)
            {
                list.Add(value);
            }
            while (cur >= n)
            {
                var v = cur % n;
                cur = cur / n;
                list.Add(v);
                if (cur < n)
                {
                    list.Add(cur);
                }
            }
            //根据层数补全位数，高位补0，倒序输出list
            for (int i = list.Count; i < layer; i++)
            {
                list.Add(0);
            }
            list.Reverse();
            return list.Select(a => a.ToString()).Aggregate((x, y) => x + y);
        }
        #endregion

        #region 数据库查询like 时，需要替换的特殊字符
        public static string SQLFilterLike(string key)
        {
            key = key.Replace("[", "[[]");
            key = key.Replace("%", "[%]");
            key = key.Replace("_", "[_]");
            key = key.Replace("^", "[^]");
            return key;
        }
        #endregion
    }
}
