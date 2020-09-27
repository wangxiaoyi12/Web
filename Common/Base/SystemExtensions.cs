using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class SystemExtensions
    {
        #region json序列化
        /// <summary>
        /// Json序列化
        /// </summary>
        /// <returns></returns>
        public static string ToJsonString(this object obj, string dateFormate = "yyyy-MM-dd")
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,//去除循环引用
                DateFormatString = dateFormate //设置日期格式化
            };
            JsonSerializer ser = JsonSerializer.Create(settings);
            using (var sw = new StringWriter())
            {
                ser.Serialize(sw, obj);
                return sw.ToString();
            }
        }
        /// <summary>
        /// Json序列化，长时间处理‘yyyy-MM-dd HH:mm:ss’
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string ToJsonLongDate(this object obj)
        {
            return ToJsonString(obj, "yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// Json反序列化
        /// </summary>
        /// <returns></returns>
        public static T JsonDeserializer<T>(this string str)
        {

            return JsonConvert.DeserializeObject<T>(str);
        }
        #endregion

        #region String 扩展,是否空串，加密，解密
        /// <summary>
        /// 字符串是否空
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNull(this string s)
        {
            return string.IsNullOrEmpty(s);
        }
        /// <summary>
        /// 字符串是否 不为空
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNotNull(this string s)
        {
            return string.IsNullOrEmpty(s) == false;
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ToEncrypt(this string obj)
        {
            if (obj.IsNull()) return string.Empty;
            return Common.CryptHelper.DESCrypt.Encrypt(obj);
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ToDecrypt(this string obj)
        {
            return Common.CryptHelper.DESCrypt.Decrypt(obj);
        }
        public static string ToMD5(this string obj)
        {
            return Common.MD5Provider.Hash(obj);
        }
        #endregion
    }
}
