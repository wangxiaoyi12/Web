using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;
using Aop.Api.Util;
using Business;
using Common;
using DataBase;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WxPayAPI;

namespace Web.Areas.Member_Mall.Controllers
{
    public class PayController : Controller
    {
        public String srcMsg = null;
        public String signMsg = null;

        public String serverUrl;

        public String key;
        public String version;
        public String language;
        public String inputCharset;
        public String merchantId;
        public String pickupUrl;
        public String receiveUrl;
        public String signType;

        public String orderNo;
        public String orderAmount;
        public String orderDatetime;
        public String orderCurrency;
        public String orderExpireDatetime;

        public String payerTelephone;
        public String payerEmail;
        public String payerName;

        public String productName;
        public String productId;
        public String productNum;
        public String productPrice;
        public String productDesc;

        public String ext1;
        public String ext2;
        public String extTL;

        public String payType;
        public String issuerId;

   
        private void appendLastSignPara(System.Text.StringBuilder buf, String key,
            String value)
        {
            if (!isEmpty(value))
            {
                buf.Append(key).Append('=').Append(value);
            }
        }
        private void appendSignPara(System.Text.StringBuilder buf, String key, String value)
        {
            if (!isEmpty(value))
            {
                buf.Append(key).Append('=').Append(value).Append('&');
            }
        }
        private bool isEmpty(String src)
        {
            if (null == src || "".Equals(src) || "-1".Equals(src))
            {
                return true;
            }
            return false;
        }
     

        public ActionResult Recevie()
        {
           


            LogHelper.Error("回调");
            /* 实际验证过程建议商户添加以下校验。
            1、商户需要验证该通知数据中的out_trade_no是否为商户系统中创建的订单号，
            2、判断total_amount是否确实为该订单的实际金额（即商户订单创建时的金额），
            3、校验通知中的seller_id（或者seller_email) 是否为out_trade_no这笔单据的对应的操作方（有的时候，一个商户可能有多个seller_id/seller_email）
            4、验证app_id是否为该商户本身。
            */
            Dictionary<string, string> sArray = GetRequestPost();
            LogHelper.Error("回调" + sArray.Count);
            if (sArray.Count != 0)
            {

                bool flag = AlipaySignature.RSACheckV1(sArray, Common.Alipay.Config.alipay_public_key, Common.Alipay.Config.charset, Common.Alipay.Config.sign_type, false);
                LogHelper.Error("回调" + flag);
                if (flag)
                {
                    //交易状态
                    //判断该笔订单是否在商户网站中已经做过处理
                    //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                    //请务必判断请求时的total_amount与通知时获取的total_fee为一致的
                    //如果有做过处理，不执行商户的业务程序

                    //注意：
                    //退款日期超过可退款期限后（如三个月可退款），支付宝系统发送该交易状态通知


                    string trade_status = Request.Form["trade_status"];

                    string code = Request.Form["out_trade_no"];
                    LogHelper.Error("回调3" + code);
               

                    var order = DB.ShopOrder.Where(a => a.OrderCode == code).FirstOrDefault();
                    LogHelper.Error("回调4" + trade_status);
                    if (order.State == 1 && (trade_status== "TRADE_SUCCESS" || trade_status == "TRADE_FINISHED"))
                    {
                        order.PayState = ShopEnum.OrderPayState.Pay.GetHashCode();
                        order.PayTime = DateTime.Now;
                        order.State = ShopEnum.OrderState.Pay.GetHashCode();
                        DB.ShopOrder.Update(order);
                        var member = DB.Member_Info.FindEntity(order.MemberID);
                        DB.Jiang.GiveJiang(DB.Member_Info.FindEntity(order.MemberID), order);

                    }
                    Response.Write("<script language='JavaScript'>location.href='http://www.jst1314.cn/mobile/mobilecenter/bill';</script>");
                }
                else
                {
                    Response.Write("<script language='JavaScript'>window.open('http://www.jst1314.cn/mobile/mobilecenter/bill');</script>");
                }
            }
            return View();

        }

        private bool QueryOrder(string transaction_id)
        {
            WxPayData req = new WxPayData();
            req.SetValue("transaction_id", transaction_id);
            WxPayData res = WxPayApi.OrderQuery(req);
            if (res.GetValue("return_code").ToString() == "SUCCESS" &&
                res.GetValue("result_code").ToString() == "SUCCESS")
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        /// <summary>
        /// 接收从微信支付后台发送过来的数据并验证签名
        /// </summary>
        /// <returns>微信支付后台返回的数据</returns>
        public WxPayData GetNotifyData()
        {
            //接收从微信后台POST过来的数据
            System.IO.Stream s = Request.InputStream;
            int count = 0;
            byte[] buffer = new byte[1024];
            StringBuilder builder = new StringBuilder();
            while ((count = s.Read(buffer, 0, 1024)) > 0)
            {
                builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
            }
            s.Flush();
            s.Close();
            s.Dispose();

            Log.Info(this.GetType().ToString(), "Receive data from WeChat : " + builder.ToString());

            //转换数据格式并验证签名
            WxPayData data = new WxPayData();
            try
            {
                data.FromXml(builder.ToString());
            }
            catch (WxPayException ex)
            {
                //若签名错误，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", ex.Message);
                Log.Error(this.GetType().ToString(), "Sign check error : " + res.ToXml());
                //return res;
                Response.Write(res.ToXml());
                Response.End();
            }

            Log.Info(this.GetType().ToString(), "Check sign success");
            return data;
        }


        public ActionResult WXRecevie()
        {

            LogHelper.Error("回调");
            WxPayData notifyData = GetNotifyData();

            //检查支付结果中transaction_id是否存在
            if (!notifyData.IsSet("transaction_id"))
            {
                //若transaction_id不存在，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "支付结果中微信订单号不存在");
                Log.Error(this.GetType().ToString(), "The Pay result is error : " + res.ToXml());
                Response.Write(res.ToXml());
                Response.End();
                return View();
            }

            string transaction_id = notifyData.GetValue("transaction_id").ToString();
            LogHelper.Error("回调transaction_id-" + transaction_id);
            //查询订单，判断订单真实性
            if (!QueryOrder(transaction_id))
            {
                LogHelper.Error("回调1进来了true");
                //若订单查询失败，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "订单查询失败");
                Log.Error(this.GetType().ToString(), "Order query failure : " + res.ToXml());
                CookieHelper.ClearCookie("orderlist");
                Response.Write(res.ToXml());
                Response.End();
            }
            //查询订单成功
            else
            {
                LogHelper.Error("11111");
                string code = notifyData.GetValue("out_trade_no").ToString();
                LogHelper.Error("回调3-" + code);

                
                var order = DB.ShopOrder.Where(a => a.OrderCode == code).FirstOrDefault();
                LogHelper.Error("回调4" + order.State);
                if (order.State == 1 )
                {
                    LogHelper.Error("改前");
                    order.PayState = ShopEnum.OrderPayState.Pay.GetHashCode();
                    order.PayTime = DateTime.Now;
                    order.State = ShopEnum.OrderState.Pay.GetHashCode();
                    DB.ShopOrder.Update(order);
                    LogHelper.Error("改完");
                    var member = DB.Member_Info.FindEntity(order.MemberID);
                    DB.Jiang.GiveJiang(DB.Member_Info.FindEntity(order.MemberID), order);
                    LogHelper.Error("结束");

                }

             

                WxPayData res = new WxPayData();
                res.SetValue("return_code", "SUCCESS");
                res.SetValue("return_msg", "OK");
                Log.Info(this.GetType().ToString(), "order query success : " + res.ToXml());
                Response.Write(res.ToXml());
                Response.End();

            }
            //Response.Write("<script language='JavaScript'>location.href='http://www.jst1314.cn/mobile/mobilecenter/bill';</script>");
            return View();


        }

     
        public Dictionary<string, string> GetRequestPost()
        {
            int i = 0;
            Dictionary<string, string> sArray = new Dictionary<string, string>();
            NameValueCollection coll;
            //coll = Request.Form;

            coll = Request.Form;

            String[] requestItem = coll.AllKeys;
            for (i = 0; i < requestItem.Length; i++)
            {
                //LogHelper.Error(requestItem[i] + " " + Request.Form[requestItem[i]]);
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
            }
            return sArray;

        }


      
    
        // GET: Member_Mall/Pay
        public ActionResult Pay(string OrderId)
        {




            var orderModel = DB.ShopOrder.FindEntity(OrderId);

            DefaultAopClient client = new DefaultAopClient(Common.Alipay.Config.gatewayUrl, Common.Alipay.Config.app_id, Common.Alipay.Config.private_key, "json", "1.0", Common.Alipay.Config.sign_type, Common.Alipay.Config.alipay_public_key, Common.Alipay.Config.charset, false);
            ////////////////////支付宝支付////////////////////////请求参数////////////////////////////////////////////

            //商户订单号，+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++，必填
            string out_trade_no = orderModel.OrderCode;

            //订单名称，必填
            string subject = orderModel.OrderCode;

            //付款金额，必填
            string total_fee = (orderModel.RealAmount + orderModel.Postage).ToString();
            //商品描述，可空
            string body = "123";
            if (Convert.ToDecimal(total_fee) > 0)
            {
                //// 组装业务参数model
                AlipayTradeWapPayModel alipaymodel = new AlipayTradeWapPayModel();
                alipaymodel.Body = body;
                alipaymodel.Subject = subject;
                alipaymodel.TotalAmount = total_fee;
                alipaymodel.OutTradeNo = out_trade_no;
                alipaymodel.ProductCode = "QUICK_WAP_WAY";
                alipaymodel.QuitUrl = "/Mall/usercenter/Order";

                AlipayTradeWapPayRequest request = new AlipayTradeWapPayRequest();
                //设置支付完成同步回调地址
                //request.SetReturnUrl("http://www.jst1314.cn/Member_Mall/Pay/Recevie?ordercode=" + orderModel.BillCode);
                request.SetReturnUrl("http://www.jst1314.cn/mobile/mobilecenter/bill");
                //设置支付完成异步通知接收地址
                request.SetNotifyUrl("http://www.jst1314.cn/Member_Mall/Pay/Recevie");
                // 将业务model载入到request
                request.SetBizModel(alipaymodel);
                AlipayTradeWapPayResponse response = null;
                try
                {
                    response = client.pageExecute(request, null, "post");
                    //return response.Body;

                    //return Success("生成订单成功", new
                    //{
                    //    ordercode = orderModel.BillCode,
                    //    addressid = orderModel.PostAddress,
                    //    amount = orderModel.TotalPrice,
                    //    ordertype = orderModel.PaymentType,
                    //    body = response.Body
                    //});
                    ViewBag.body = response.Body;
                    return View();
                }
                catch (Exception exp)
                {
                    LogHelper.Error("支付接口错误:" + exp.Message);
                    throw exp;
                }

            }
            return View();
        }


        public ActionResult WXPay(string OrderId)
        {
            var orderModel = DB.ShopOrder.FindEntity(OrderId);
            H5Pay h5Pay = new H5Pay();
            var wxConfig = WxPayConfig.GetConfig();
            string clientIP = wxConfig.GetIp();//获取客户端真实IP     
            string url = h5Pay.GetPayUrl(clientIP, orderModel.OrderCode, (orderModel.RealAmount+orderModel.Postage.Value) * 100M);//通过统一下单接口进行H5支付    
                                                                                                         //Response.Redirect(url);//跳转到微信支付中间页     
            ViewBag.body = url;
            return View();
            

      
        }

        public ActionResult RemitPay(string RemitId)
        {




            var orderModel = DB.Fin_Remit.FindEntity(Convert.ToInt32(RemitId));

            DefaultAopClient client = new DefaultAopClient(Common.Alipay.Config.gatewayUrl, Common.Alipay.Config.app_id, Common.Alipay.Config.private_key, "json", "1.0", Common.Alipay.Config.sign_type, Common.Alipay.Config.alipay_public_key, Common.Alipay.Config.charset, false);
            ////////////////////支付宝支付////////////////////////请求参数////////////////////////////////////////////

            //商户订单号，+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++，必填
            string out_trade_no = orderModel.RemitId.ToString();

            //订单名称，必填
            string subject = "汇款申请";

            //付款金额，必填
            string total_fee = orderModel.Amount.ToString();
            //商品描述，可空
            string body = "123";
            if (Convert.ToDecimal(total_fee) > 0)
            {
                //// 组装业务参数model
                AlipayTradeWapPayModel alipaymodel = new AlipayTradeWapPayModel();
                alipaymodel.Body = body;
                alipaymodel.Subject = subject;
                alipaymodel.TotalAmount = total_fee;
                alipaymodel.OutTradeNo = out_trade_no;
                alipaymodel.ProductCode = "QUICK_WAP_WAY";
                alipaymodel.QuitUrl = "/Mall/usercenter/Order";

                AlipayTradeWapPayRequest request = new AlipayTradeWapPayRequest();
                //设置支付完成同步回调地址
                request.SetReturnUrl("http://www.jst1314.cn/member_finance/remit/index");
                //设置支付完成异步通知接收地址
                request.SetNotifyUrl("http://www.jst1314.cn/Member_Mall/Pay/RemitRecevie");
                // 将业务model载入到request
                request.SetBizModel(alipaymodel);
                AlipayTradeWapPayResponse response = null;
                try
                {
                    response = client.pageExecute(request, null, "post");
                    //return response.Body;

                    //return Success("生成订单成功", new
                    //{
                    //    ordercode = orderModel.BillCode,
                    //    addressid = orderModel.PostAddress,
                    //    amount = orderModel.TotalPrice,
                    //    ordertype = orderModel.PaymentType,
                    //    body = response.Body
                    //});
                    ViewBag.body = response.Body;
                    return View();
                }
                catch (Exception exp)
                {
                    LogHelper.Error("支付接口错误:" + exp.Message);
                    throw exp;
                }

            }
            return View();
        }

     

        public JsonResult Success(string content)
        {
            object obj = new
            {
                status = 1,
                msg = content
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
    }
}