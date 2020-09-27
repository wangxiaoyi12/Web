using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Common;
using DataBase;
using Business;
namespace System.Web
{
    public class PayHelper
    {
        public static string Domain
        {
            get
            {
                return HttpContext.Current.Request.Url.Host;
            }
        }

        /// <summary>
        /// 获取支付
        /// </summary>
        /// <returns></returns>
        //public static string GetPayUrl(string orderid, decimal amount,string remark,bool isScore=false)
        //{
        //    //金额处理
        //    Xml_Site config = DB.XmlConfig.XmlSite;
        //    if (config.TestAmount > 0)
        //    {
        //        amount = config.TestAmount;
        //    }
        //    amount *= 100;


        //    PayInfo info = new PayInfo();
        //    info.Order_ID = orderid;
        //    info.goods_inf = remark;
        //    info.Amount = Convert.ToInt64(amount);
        //    info.Service = "req_front_page_pay";
        //    info.notify_url = "http://" + Domain + "/ajax/" + (isScore ? "score" : "trade");
        //    return GetUrl(info.GetHashtable());
        //}
        ///// <summary>
        ///// 微信支付下单处理，返回微信二维码
        ///// </summary>
        ///// <param name="orderid"></param>
        ///// <param name="amount"></param>
        ///// <returns></returns>

        //public static string QrCodePay(string orderid, decimal amount, string remark)
        //{
        //    PayInfo info = new PayInfo();
        //    info.Order_ID = orderid;
        //    info.Amount = amount;

        //    //获取支付二维码
        //    info.Service = "active_scancode_order";
        //    info.goods_inf = remark;
        //    info.notify_url = "http://" + Domain + "/home/BackHandle";
        //    info.scancode_type = "WECHAT";

        //    //发送请求
        //    string url = GetUrl(info.GetHashtable());
        //    string reuslt = NetHelper.Get(url);
        //    Hashtable data = com.umpay.api.paygate.v40.Plat2Mer_v40.getResData(reuslt);
        //    string payurl = data["bank_payurl"] as string;
        //    if (string.IsNullOrEmpty(payurl))
        //        throw new Exception("获取微信url失败");
        //    return payurl;
        //}
        /// <summary>
        /// 微信公众号支付
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        //public static string WeiXinPay(string orderid, decimal amount, string remark)
        //{
        //    //金额处理
        //    Xml_Site config = DB.XmlConfig.XmlSite;
        //    if (config.TestAmount > 0)
        //    {
        //        amount = config.TestAmount;
        //    }
        //    amount *= 100;

        //    PayInfo info = new PayInfo();
        //    info.Order_ID = orderid;
        //    info.Amount = Convert.ToInt64(amount);

        //    //公众号支付
        //    info.Service = "publicnumber_and_verticalcode";
        //    info.goods_inf = remark;
        //    info.notify_url = "http://" + Domain + "/ajax/trade";
        //    info.is_public_number = "Y";

        //    return GetUrl(info.GetHashtable());
        //}
        /// <summary>
        /// 解析相应数据
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        //public static PayInfo GetBack(string html)
        //{
        //    Hashtable data = new Hashtable();
        //    data = com.umpay.api.paygate.v40.Plat2Mer_v40.getResData(html);
        //    PayInfo back = new PayInfo(data);
        //    return back;
        //}
        /// <summary>
        /// 响应成功
        /// </summary>
        /// <param name="info"></param>
        //public static void SetSuccess(PayInfo info)
        //{
        //    PayBackInfo back = PayBackInfo.GetSuccess(info);
        //    Hashtable ht = back.GetHashtable();
        //    com.umpay.api.common.ReqData reqDataGet = com.umpay.api.paygate.v40.Mer2Plat_v40.makeReqDataByGet(ht);
        //    string url = reqDataGet.Url;
        //    string result = NetHelper.Get(url);
        //}



        /// <summary>
        /// 获取请求地址
        /// </summary>
        /// <param name="ht"></param>
        /// <returns></returns>
        private static string GetUrl(Hashtable ht)
        {
            com.umpay.api.common.ReqData reqDataGet = com.umpay.api.paygate.v40.Mer2Plat_v40.makeReqDataByGet(ht);
            return reqDataGet.Url;
        }
    }


}