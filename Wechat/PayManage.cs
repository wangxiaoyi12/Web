using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Xml.Linq;
using Common;
using System.Reflection;
using System.Web;

namespace Wechat
{
    /// <summary>
    /// 微信支付，操作封装
    /// </summary>
    public class PayManage : BaseManage
    {
        LinkManage _link = new LinkManage();
        /// <summary>
        /// 下单成功后处理
        /// </summary>
        public Action<OrderResult> OnCreateOrderSuccess = null;
        /// <summary>
        /// 用户支付成功后出发
        /// </summary>
        public Action<PayBackResult> OnPaySuccess = null;


        public string BackHandleUrl = "http://www.zngheworld.com/part/back";
        /// <summary>
        /// 处理订单响应结果
        /// </summary>
        public void BackHandle(HttpRequestBase req, HttpResponseBase resp)
        {
            try
            {
                //1.获取响应内容
                XmlReader reader = XmlReader.Create(req.InputStream);
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);

                LogOperate.Write(doc.InnerXml);
                PayBackResult result = new PayBackResult();
                LoadXmlToObject(doc, result);
                //处理结果
                if (result.Result_Code.ToUpper() == "SUCCESS")
                {
                    resp.Write(@"<xml>
                      <return_code><![CDATA[SUCCESS]]></return_code>
                      <return_msg><![CDATA[OK]]></return_msg>
                    </xml>");
                    resp.End();

                    //处理支付成功结果
                    OnPaySuccess?.Invoke(result);
                }
            }
            catch (Exception ex)
            {
                LogOperate.Write(ex);
                throw ex;
            }
        }
        /// <summary>
        /// 创建订单
        /// </summary>
        public void CreateOrder(string orderid, decimal amount, string openid, string trade_type = "JSAPI", string body = "测试下单")
        {
            try
            {
                OrderInfo order = new OrderInfo();
                order.Out_Trade_No = orderid;
                order.Total_Fee = Convert.ToInt32(amount * 100);
                order.Spbill_create_ip = GetWebClientIp();
                order.OpenID = openid;
                //根据客户端确定是否类型
                order.Trade_Type = trade_type;
                //回调地址
                order.Notify_Url = BackHandleUrl;
                order.Attach = "竹笛舰店";
                order.Body = body;
                //签名处理
                order.Sign = ConfigInfo.GetSign(GetSendDic(order));
                string data = GetSendData(order);
                //LogOperate.Write("所有请求参数：" + data);
                //提交请求
                string result = NetHelper.Post(_link.GetPayUnifiedOrder(), data);
                //LogOperate.Write(result);
                if (string.IsNullOrEmpty(result))
                    throw new Exception("微信服务器响应结果为空");


                //读取返回的Xml数据结果
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result);
                OrderResult orderResult = new OrderResult();
                LoadXmlToObject(doc, orderResult);
                if (orderResult.Return_Code.ToUpper() == "SUCCESS" && orderResult.Result_Code.ToUpper() == "SUCCESS")
                {
                    LogHelper.Debug("下单成功后发出"+ orderResult.Code_Url+"_"+ orderResult.Return_Code);
                    //下单成功后出发
                    OnCreateOrderSuccess?.Invoke(orderResult);
                }
                else
                    throw new Exception($"统一下单失败{orderResult.Err_Code}：" + orderResult.Err_Code_Des);

            }
            catch (Exception ex)
            {
                LogOperate.Write(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 获取发送数据内容
        /// </summary>
        /// <returns></returns>
        private string GetSendData(OrderInfo order)
        {
            ConfigInfo config = ConfigInfo.GetInfo();
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("xml");

            Dictionary<string, string> dic = GetSendDic(order);
            foreach (var item in dic)
            {
                root.AppendChild(getElem(doc, item.Key, item.Value));
            }
            doc.AppendChild(root);
            return doc.InnerXml;
        }
        /// <summary>
        /// 获取发送测参数列表
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetSendDic(OrderInfo order)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("appid", config.AppID);
            dic.Add("mch_id", config.Mch_ID);
            dic.Add("nonce_str", order.Nonce_Str);


            dic.Add("body", order.Body);
            //dic.Add("detail", order.Detail);
            dic.Add("attach", order.Attach);
            dic.Add("out_trade_no", order.Out_Trade_No);
            dic.Add("fee_type", order.Fee_Type);
            dic.Add("total_fee", order.Total_Fee.ToString());
            dic.Add("spbill_create_ip", order.Spbill_create_ip);
            dic.Add("notify_url", order.Notify_Url);
            dic.Add("trade_type", order.Trade_Type);
            dic.Add("openid", order.OpenID);

            if (!string.IsNullOrEmpty(order.Sign_Type))
                dic.Add("sign_type", order.Sign_Type);
            if (!string.IsNullOrEmpty(order.Sign))
                dic.Add("sign", order.Sign);
            return dic;
        }
        /// <summary>
        /// 创建节点
        /// </summary>
        private XmlNode getElem(XmlDocument doc, string key, string value)
        {
            XmlNode elem = doc.CreateNode(XmlNodeType.Element, key, "");
            elem.InnerText = value;
            return elem;
        }

        /// <summary>
        /// 从Xml文档中转换到指定对象
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="result"></param>
        private void LoadXmlToObject(XmlDocument doc, object result)
        {
            PropertyInfo[] pros = result.GetType().GetProperties();
            foreach (PropertyInfo item in pros)
            {
                string name = item.Name;
                //从xml中获取值
                XmlNode node = doc.SelectSingleNode("//" + name.ToLower());
                if (node != null)
                {
                    object value = ChangeType(node.InnerText, item.PropertyType);
                    if (value != null)
                        item.SetValue(result, value);
                }
            }
        }

        #region 通用转换类型
        public static object ChangeType(object value, Type type)
        {
            try
            {
                if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
                if (value == null) return null;
                if (type == value.GetType()) return value;
                if (type.IsEnum)
                {
                    if (value is string)
                        return Enum.Parse(type, value as string);
                    else
                        return Enum.ToObject(type, value);
                }
                if (!type.IsInterface && type.IsGenericType)
                {
                    Type innerType = type.GetGenericArguments()[0];
                    object innerValue = ChangeType(value, innerType);
                    return Activator.CreateInstance(type, new object[] { innerValue });
                }
                if (value is string && type == typeof(Guid)) return new Guid(value as string);
                if (value is string && type == typeof(Version)) return new Version(value as string);
                if (!(value is IConvertible)) return value;
                return Convert.ChangeType(value, type);
            }
            catch
            {

            }
            return value;
        }
        #endregion

        ///// <summary>
        ///// 签名
        ///// </summary>
        ///// <param name="dic"></param>
        ///// <returns></returns>
        //private string GetSignString(Dictionary<string, string> dic)
        //{
        //    string key = Configs.GetValue("APIKey");//商户平台 API安全里面设置的KEY  32位长度
        //                                            //排序
        //    dic = dic.OrderBy(d => d.Key).ToDictionary(d => d.Key, d => d.Value);
        //    //连接字段
        //    var sign = dic.Aggregate("", (current, d) => current + (d.Key + "=" + d.Value + "&"));
        //    sign += "key=" + key;
        //    //MD5
        //    sign = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sign, "MD5").ToUpper();

        //    return sign;
        //}

        /// <summary>
        /// 获取客户端的IP
        /// </summary>
        /// <returns></returns>
        private static string GetWebClientIp()
        {
            string userIP = "IP";

            try
            {
                if (System.Web.HttpContext.Current == null
            || System.Web.HttpContext.Current.Request == null
            || System.Web.HttpContext.Current.Request.ServerVariables == null)
                    return "";

                string CustomerIP = "";

                //CDN加速后取到的IP   
                CustomerIP = System.Web.HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
                if (!string.IsNullOrEmpty(CustomerIP))
                {
                    return CustomerIP;
                }

                CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];


                if (!String.IsNullOrEmpty(CustomerIP))
                    return CustomerIP;

                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (CustomerIP == null)
                        CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                else
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                }

                if (string.Compare(CustomerIP, "unknown", true) == 0)
                    return System.Web.HttpContext.Current.Request.UserHostAddress;
                return CustomerIP;
            }
            catch { }

            return userIP;
        }
        /// <summary>
        /// 从字符串里随机得到，规定个数的字符串.
        /// </summary>
        /// <param name="CodeCount"></param>
        /// <returns></returns>
        private static string GetRandomString(int CodeCount)
        {
            string allChar = "1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            string[] allCharArray = allChar.Split(',');
            string RandomCode = "";
            int temp = -1;
            Random rand = new Random();
            for (int i = 0; i < CodeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(temp * i * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(allCharArray.Length - 1);
                while (temp == t)
                {
                    t = rand.Next(allCharArray.Length - 1);
                }
                temp = t;
                RandomCode += allCharArray[t];
            }
            return RandomCode;
        }



        /// <summary>
        /// 统一下单参数列表
        /// </summary>
        public class OrderInfo
        {
            public OrderInfo()
            {
                this.Nonce_Str = GetRandomString(32);
                // this.Sign = GetSign();
            }
            public string Nonce_Str { get; set; }
            public string Sign { get; set; }
            /// <summary>
            /// 商品描述
            /// </summary>
            public string Body { get; set; }
            /// <summary>
            /// 商户订单号
            /// </summary>
            public string Out_Trade_No { get; set; }
            /// <summary>
            /// 总金额，单位 分
            /// </summary>
            public int Total_Fee { get; set; }
            /// <summary>
            /// 终端IP，IPv4
            /// </summary>
            public string Spbill_create_ip { get; set; }
            /// <summary>
            /// 通知回调地址
            /// </summary>
            public string Notify_Url { get; set; }
            /// <summary>
            /// 交易类型
            /// </summary>
            public string Trade_Type { get; set; } = "JSAPI";
            /// <summary>
            /// 当使用JSAPI时，此参数必填
            /// </summary>
            public string OpenID { get; set; }

            #region 非必填字段
            /// <summary>
            /// 设备号
            /// </summary>
            public string Device_Info { get; set; } = "Web";
            /// <summary>
            /// 签名类型，默认为MD5
            /// </summary>
            public string Sign_Type { get; set; }
            /// <summary>
            /// 商品ID
            /// </summary>
            public string Product_ID { get; set; }
            /// <summary>
            /// 附件数据---店铺名称等
            /// </summary>
            public string Attach { get; set; }
            /// <summary>
            /// 商品标记
            /// </summary>
            public string Goods_Tag { get; set; } = "WXG";
            /// <summary>
            /// 商品详情
            /// </summary>
            public string Detail { get; set; }
            /// <summary>
            /// 标价币种，默认为人民币
            /// </summary>
            public string Fee_Type { get; set; } = "CNY";

            /// <summary>
            /// 交易开始时间
            /// </summary>
            public string Time_Start { get; set; }
            /// <summary>
            /// 交易结束时间
            /// </summary>
            public string Time_Expire { get; set; }

            /// <summary>
            /// 指定实付方式，上传此参数no_credit--可限制用户不能使用信用卡支付
            /// </summary>
            public string Limit_Pay { get; set; }
            #endregion


            ///// <summary>
            ///// 获取签名字符串
            ///// </summary>
            ///// <returns></returns>
            //public string GetSign(Dictionary<string, string> dic)
            //{
            //    //Dictionary<string, string> dic = new Dictionary<string, string>();
            //    ConfigInfo config = ConfigInfo.GetInfo();
            //    //dic.Add("appid", config.AppID);
            //    //dic.Add("mch_id", config.Mch_ID);
            //    //dic.Add("device_info", this.Device_Info);
            //    //dic.Add("body", this.Body);
            //    //dic.Add("nonce_str", this.Nonce_Str);

            //    string[] tempList = dic.OrderBy(q => q.Key).Select(q => q.Key.ToLower() + "=" + q.Value).ToArray();
            //    string signTemp = string.Join("&", tempList);
            //    //追加apikey
            //    signTemp += "&key=" + config.APIKey;
            //    return Common.SecurityHelper.MD5(signTemp);
            //}
        }


    }

    /// <summary>
    /// 下单结果数据
    /// </summary>
    public class OrderResult
    {
        public string Return_Code { get; set; }
        public string Return_Msg { get; set; }
        public string APPID { get; set; }

        public string Mch_ID { get; set; }
        public string Device_Info { get; set; }

        public string Nonce_Str { get; set; }
        public string Sign { get; set; }


        public string Result_Code { get; set; }
        public string Err_Code { get; set; }
        public string Err_Code_Des { get; set; }

        public string Trade_Type { get; set; }

        /// <summary>
        /// 微信生成的预支付会话标识，用于后续接口调用中使用，该值有效期为2小时
        /// </summary>
        public string Prepay_ID { get; set; }
        public string Code_Url { get; set; }
    }

    /// <summary>
    /// 支付结果响应数据
    /// </summary>
    public class PayBackResult
    {
        public string Return_Code { get; set; }
        public string Return_Msg { get; set; }
        public string APPID { get; set; }

        public string Mch_ID { get; set; }
        public string Device_Info { get; set; }

        public string Nonce_Str { get; set; }
        public string Sign { get; set; }
        public string Sign_Type { get; set; }

        public string Result_Code { get; set; }
        public string Err_Code { get; set; }
        public string Err_Code_Des { get; set; }

        public string Trade_Type { get; set; }

        public string OpenID { get; set; }
        public string is_subscribe { get; set; }
        public string Back_Type { get; set; }


        public int total_fee { get; set; }
        public int settlement_total_fee { get; set; }
        public string fee_type { get; set; }
        public int cash_fee { get; set; }
        public string cash_fee_type { get; set; }

        //public int coupon_fee { get; set; }
        //public int coupon_count { get; set; }
        //public string coupon_type_$n {get;set;}



        /// <summary>
        /// 微信支付订单号
        /// </summary>
        public string transaction_id { get; set; }
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string out_trade_no { get; set; }
        /// <summary>
        /// 商家数据包
        /// </summary>
        public string attach { get; set; }
        /// <summary>
        /// 支付完成时间
        /// </summary>
        public string time_end { get; set; }
    }
}
