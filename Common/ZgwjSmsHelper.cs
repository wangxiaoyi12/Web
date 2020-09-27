using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace System
{
    /// <summary>
    /// 中国网建 短信通，操作封装
    /// 注：api地址http://sms.webchinese.com.cn/api.shtml
    /// </summary>
    public class ZgwjSmsHelper
    {
       
        /// <summary>
        /// 登录账号
        /// </summary>
        public string Account { get; set; }
        public string UserID { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userid">申请appid</param>
        /// <param name="account">登录账号</param>
        /// <param name="pwd">登录密码</param>
        public ZgwjSmsHelper(string userid,string account, string pwd)
        {
            this.Account = account;
            this.Password = pwd;
            this.UserID = userid;
        }
        /// <summary>
        ///全站使用账号定义
        /// </summary>
        /// <returns></returns>
        public static ZgwjSmsHelper Create()
        {
            return new ZgwjSmsHelper("31359", "JNWZ200915", "JNWZ200915");
        }


        /// <summary>
        /// 发送成功,返回发送成功的条数+余额
        /// </summary>
        public Action<int> OnSuccess = null;
        /// <summary>
        /// 发送失败，失败相应内容
        /// </summary>
        public Action<string> OnError = null;
        /// <summary>
        /// 指定Url地址使用Get 方式获取全部字符串
        /// </summary>
        /// <param name="url">请求链接地址</param>
        /// <returns></returns>
        public static string Get(string url)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            try
            {
                //获取内容
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            finally
            {
                stream.Close();
            }
            return result;
        }
        /// <summary>
        /// 执行发送操作
        /// </summary>
        /// <param name="mobile">发信发送的目的号码.多个号码之间用半角逗号隔开</param>
        /// <param name="content">短信验证码内容</param>
        public void Send(string mobile, string content)
        {
            string sendUrl = "http://www.lcqxt.com/sms.aspx";
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("action", "send");
            param.Add("userid", this.UserID);
            param.Add("account", this.Account);
            param.Add("password", this.Password);
            param.Add("mobile", mobile);
            param.Add("content", content);
            //空表示立即发送
            //指定时间，则到点发送
            //param.Add("sendTime", DateTime.Now.ToString("yyyy-MM-dd hh:mmss"));


            string result = Post(sendUrl, param);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);
            XmlNode status = doc.DocumentElement.SelectSingleNode("returnstatus");
            if (status.InnerText == "Success")
            {
                XmlNode node = doc.DocumentElement.SelectSingleNode("successCounts");
                if (node == null)
                    throw new Exception("获取successCounts节点失败");
                int count = Convert.ToInt32(node.InnerText);

                XmlNode remain = doc.DocumentElement.SelectSingleNode("remainpoint");
                if (remain == null)
                    throw new Exception("获取remainpoint节点失败");

                decimal point = Convert.ToDecimal(remain.InnerText);
                OnSuccess?.Invoke(count);
            }
            else
            {
                XmlNode msg = doc.DocumentElement.SelectSingleNode("message");
                if (msg == null)
                    throw new Exception("获取message节点失败");
                OnError?.Invoke(msg.InnerText);
            }
        }
        public static string Post(string url, Dictionary<string, string> dic)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            #region 添加Post 参数  
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容  
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
        /// <summary>
        /// 获取剩余数量
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            string url = $"http://www.smschinese.cn/web_api/SMS/?Action=SMS_Num&Uid={this.Account}&Key={this.Password}";

            string result = Get(url);
            return Convert.ToInt32(result);
        }
    }
}
