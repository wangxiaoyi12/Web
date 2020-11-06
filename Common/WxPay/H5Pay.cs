using Common;
using System;
using System.Web;
namespace WxPayAPI
{
    public class H5Pay
    {
        public string GetPayUrl(string clientip,string orderCode,decimal totalAmount)
        {
            Log.Info(this.GetType().ToString(), "H5 pay url is producing...");
            WxPayData data = new WxPayData();
            data.SetValue("body", "商品描述");//这里替换成你的数据       
            data.SetValue("attach", "详见我的订单");//这里替换成你的数据       
            data.SetValue("out_trade_no", orderCode);//这里替换成你的数据  "商户订单号"       
            data.SetValue("total_fee", ((int)totalAmount).ToString());//这里替换成你的数据  "总金额"     
            data.SetValue("spbill_create_ip", clientip);//终端IP    
            data.SetValue("trade_type", "MWEB");//交易类型        
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));
            //data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));
            //data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));
            data.SetValue("scene_info", "{'h5_info':{'type':'Wap','wap_url':'www.jst1314.cn','wap_name':'会员商城'}}");//场景信息   
            WxPayData result = WxPayApi.UnifiedOrder(data);//调用统一下单接口    
            string url = result.GetValue("mweb_url").ToString();//获得统一下单接口返回的链接   
            Log.Info(this.GetType().ToString(), "Get H5 pay url : " + url);
            Log.Info(this.GetType().ToString(), url + "&redirect_url=http%3A%2F%2Fwww.jst1314.cn/mobile/mobilecenter/bill");
            url = url + "&redirect_url=http%3A%2F%2Fwww.jst1314.cn/mobile/mobilecenter/bill";
            return url;
        }


        public string GetPayUrl1(string clientip, string orderCode, decimal totalAmount)
        {
            Log.Info(this.GetType().ToString(), "H5 pay url is producing...");
            WxPayData data = new WxPayData();
            data.SetValue("body", "汇款申请");//这里替换成你的数据       
            data.SetValue("attach", "详见我的汇款订单");//这里替换成你的数据       
            data.SetValue("out_trade_no", orderCode);//这里替换成你的数据  "商户订单号"       
            data.SetValue("total_fee", ((int)totalAmount).ToString());//这里替换成你的数据  "总金额"     
            data.SetValue("spbill_create_ip", clientip);//终端IP    
            data.SetValue("trade_type", "MWEB");//交易类型         
            data.SetValue("scene_info", "{'h5_info':{'type':'Wap','wap_url':'tjyy.fabeisha.cn','wap_name':'法贝莎总代订货系统'}}");//场景信息   
            WxPayData result = WxPayApi.UnifiedOrder(data);//调用统一下单接口    
            LogHelper.Error("支付接口错误:" + result);

            string url = result.GetValue("mweb_url").ToString();//获得统一下单接口返回的链接   
            Log.Info(this.GetType().ToString(), "Get H5 pay url : " + url);
            return url;
        }
    }
}

