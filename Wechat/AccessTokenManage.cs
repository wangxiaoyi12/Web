using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using Common;
using Newtonsoft.Json.Linq;

namespace Wechat
{
    /// <summary>
    /// AccessToken 状态值维护
    /// </summary>
    public class AccessTokenManage
    {
        /// <summary>
        /// 当前状态值
        /// </summary>
        public static string AccessToken = null;
        /// <summary>
        /// jsapi_ticket用于 生成签名
        /// </summary>
        public static string Jsapi_Ticket = null;
        /// <summary>
        /// 当前公众号的AppID
        /// </summary>
        public static string AppID = null;
        /// <summary>
        /// 下一次获取access_token 的时间点
        /// </summary>
        public static DateTime NextTime { get; set; }
        /// <summary>
        /// 指定当前线程是否可运行
        /// </summary>
        private static bool EnableRun = true;
        /// <summary>
        /// 运行维护操作
        /// </summary>
        public static void Run()
        {
            if (AccessToken == null)
            {
                Task.Factory.StartNew(() =>
                {
                    while (EnableRun)
                    {
                        //超过时间，重新获取
                        if (DateTime.Now >= NextTime)
                        {
                           //1.先获取access_token
                            RequestAccessToken();
                            //2.再获取jsapi_ticket
                            RequestJsApi_Ticket();
                        }
                        //每分中轮询
                        Thread.Sleep(60000);
                    }
                });
            }
     
        }
        static AccessTokenManage()
        {
            //1.初始化AppID
            if (AppID == null)
                AppID = ConfigInfo.GetInfo().AppID;
        }
        /*
        access_token是公众号的全局唯一票据，公众号调用各接口时都需使用access_token。开发者需要进行妥善保存。
        access_token的存储至少要保留512个字符空间。
        access_token的有效期目前为2个小时，需定时刷新，重复获取将导致上次获取的access_token失效。
        */
        /// <summary>
        /// 调用接口，获取access_token
        /// </summary>
        public static void RequestAccessToken()
        {
            string url = new LinkManage().GetAccessToken();
            try
            {
                string result = NetHelper.Get(url);
                Access_TokenData _data = result.JsonDeserialezer<Access_TokenData>();
                if (string.IsNullOrEmpty(_data.access_token) == false)
                {
                    //获取成功
                    AccessToken = _data.access_token;
                    NextTime = DateTime.Now.AddSeconds(_data.expires_in - 10);
                    //测试代码
                    LogOperate.Write("获取access_token成功：" + AccessToken);
                }
                else
                {
                    throw new Exception("反序列化Access_TokenData对象失败：" + result);
                }
            }
            catch (Exception ex)
            {
                LogOperate.Write("获取AccessToken失败," + ex.Message);
                if ((DateTime.Now - NextTime).TotalMinutes >= 10)
                {
                    EnableRun = false;
                }
                NextTime = DateTime.Now;
            }
        }
        /// <summary>
        /// 获取jsapi_ticket
        /// 1.有效期为7200秒
        /// </summary>
        public static void RequestJsApi_Ticket()
        {
            try
            {
                string url = new LinkManage().GetJsApi_Ticket();
                string result = NetHelper.Get(url);
                JObject obj = JObject.Parse(result);
                try
                {
                    string ticket = (string)obj["ticket"];
                    if (string.IsNullOrEmpty(ticket) == false)
                    {
                        Jsapi_Ticket = ticket;
                        return;
                    }
                    throw new Exception("获取ticket为空");
                }
                catch (Exception ex)
                {
                    throw new Exception("反序列化获取jsapi_ticket失败，" + ex.Message + ",返回内容：" + result);
                }
            }
            catch (Exception ex)
            {
                LogOperate.Write("获取jsapi_ticket失败，" + ex.Message);
                if ((DateTime.Now - NextTime).TotalMinutes >= 10)
                {
                    EnableRun = false;
                }
                NextTime = DateTime.Now;
            }
        }
        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        public static int TimeStamp
        {
            get
            {
                DateTime start = new DateTime(1970, 1, 1);
                return (int)(DateTime.Now - start).TotalSeconds;
            }
        }
        /// <summary>
        /// 获取签名
        /// </summary>
        /// <param name="timestamp">时间戳和noncestr</param>
        /// <param name="url">当前链接地址</param>
        /// <returns></returns>
        public static string GetSignture(int timestamp, string url)
        {
            if (url.Contains("#"))
            {
                url = url.Substring(0, url.IndexOf("#"));
            }
            string[] temp = new string[] {
                "jsapi_ticket="+Jsapi_Ticket,
                "noncestr="+timestamp,
                "timestamp="+timestamp,
                "url="+url
            };
            string result = string.Join("&", temp);
            LogHelper.Debug("签名url:" + url);
            // return SecurityHelper.SHA1(result).ToLower();
            return result;
        }
        /// <summary>
        /// 请求返回的结果对象
        /// </summary>
        public class Access_TokenData
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string errcode { get; set; }
            public string errmsg { get; set; }
        }
    }
}
