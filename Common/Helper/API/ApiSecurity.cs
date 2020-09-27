using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// api传输安全性验证
    /// </summary>
    public class ApiSecurity
    {
        private readonly static string HelperString = "mxbig";
        /// <summary>
        /// 根据传入字符串设置token
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string setToken(string id)
        {
            return (id + HelperString).ToEncrypt();
        }
        /// <summary>
        /// 根据传入字符串获取token
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string getToken(string str)
        {
            var s = str.ToDecrypt();
            if (s.IsNotNull() && s.Length > HelperString.Length)
            {
                return s.Substring(0, s.Length - HelperString.Length);
            }
            else
            {
                return string.Empty;
            }          
        }
    }
}
