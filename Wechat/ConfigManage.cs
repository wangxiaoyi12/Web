using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using Common;
using System.IO;

namespace Wechat
{
    /// <summary>
    /// 配置部分管理
    /// </summary>
    public class ConfigManage : BaseManage
    {
        /// <summary>
        /// 服务器配置验证
        /// </summary>
        /// <returns></returns>
        public string CheckServer()
        {
            try
            {
                string signature = ReqHelper.GetStringQuery("signature");
                string timestamp = ReqHelper.GetStringQuery("timestamp");
                string nonce = ReqHelper.GetStringQuery("nonce");
                string echostr = ReqHelper.GetStringQuery("echostr");
                string result = string.Join("", new[] { config.Token, timestamp, nonce }.OrderBy(q => q));
                result = Common.SecurityHelper.SHA1(result);
                if (result.ToLower() == signature.ToLower())
                {
                    //验证消息成功
                    //判断是否是Post提交，并获取Post数据
                    //ReceiveData();
                    return echostr;
                }
                return "参数解析失败:" + echostr;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        ///// <summary>
        ///// 接收服务器消息
        ///// </summary>
        //public void ReceiveData()
        //{
        //    HttpRequest req = HttpContext.Current.Request;
        //    if (req.HttpMethod == "POST")
        //    {
        //        Stream stream = req.InputStream;
        //        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
        //        try
        //        {
        //            string result = reader.ReadToEnd();
        //            //1，保存当前消息
        //            ReceiveMessage _receive = new ReceiveMessage(result);
        //            MsgRecord record = _receive.GetCurrent();

        //            //2.根据输入内容相应消息
        //            SendMessage _send = new SendMessage(record);
        //            _send.SendText("您输入发送的内容是：" + record.Content);
        //            LogOperate.Write(result);
        //        }
        //        catch (Exception ex)
        //        {
        //            LogOperate.Write("响应出错：" + ex.Message);
        //        }
        //        finally
        //        {
        //            reader.Close();
        //            stream.Close();
        //        }
        //    }
        //}
        /// <summary>
        /// 获取微信服务器ip列表
        /// </summary>
        /// <returns></returns>
        public WXIPData GetIPList()
        {
            try
            {
                string url = new LinkManage().GetWXIP();
                string result = NetHelper.Get(url);
                WXIPData _data = result.JsonDeserialezer<WXIPData>();
                if (_data == null)
                    throw new Exception(result);
                return _data;
            }
            catch (Exception ex)
            {
                throw new Exception("获取微信服务器ip列表异常，" + ex.Message);
            }
        }


        #region 实体类定义
        /// <summary>
        /// 微信IP地址返回列表
        /// </summary>
        public class WXIPData
        {
            public List<string> ip_list { get; set; }
        }
        #endregion
    }
}
