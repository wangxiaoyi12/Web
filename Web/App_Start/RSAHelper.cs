using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.IO;

namespace System.Web
{
    /// <summary>
    /// 参数加密处理
    /// </summary>
    public class RSAHelper
    {
        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="publickey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string Encrypt(string content)
        {
            //指定url参数签名
            return com.umpay.api.util.SignUtil.RSAEncrypt(content);
        }
    }
}