using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Text;
using System.Collections;

using Common;
namespace System.Web
{
    /// <summary>
    /// 基本信息定义
    /// </summary>
    public class PayBackInfo
    {

        public PayBackInfo()
        {
        }
        //public PayBackInfo(Hashtable ht)
        //{
        //    foreach (var item in ht.Keys)
        //    {
        //        object value = ht[item];
        //        PropertyInfo info = this.GetType().GetProperty(item.ToString(), BindingFlags.IgnoreCase);
        //        if (info != null)
        //        {
        //            info.SetValue(this, value);
        //        }
        //    }
        //}

        /// <summary>
        /// 订单号
        /// </summary>
        public string Order_ID { get; set; }
        /// <summary>
        /// 支付金额，单位：分
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 签名，参数
        /// </summary>
        public string Sign { get; set; }
        /// <summary>
        /// 返回码
        /// </summary>
        public string Ret_Code { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Ret_Msg { get; set; }
        /// <summary>
        /// 联动交易号
        /// </summary>
        public string Trade_No { get; set; }


        #region 固定参数
        /// <summary>
        /// 账户ID
        /// </summary>
        public string Mer_ID { get; set; } = "50234";
        /// <summary>
        /// 签名方式
        /// </summary>
        public string Sign_Type { get; set; } = "RSA";
        /// <summary>
        /// 编码类型
        /// </summary>
        public string CharSet { get; set; } = "UTF-8";
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; } = "4.0";
  
        /// <summary>
        /// 下单日期
        /// </summary>
        public string Mer_Date { get; set; } = DateTime.Now.ToString("yyyyMMdd");
        #endregion

        ///// <summary>
        ///// 判断是否支付成功
        ///// </summary>
        ///// <returns></returns>
        //public bool IsSuccess()
        //{
        //    if (this.Ret_Code == "0000")
        //        return true;
        //    return false;
        //}


        public Hashtable GetHashtable()
        {
            Hashtable table = new Hashtable();

            PropertyInfo[] infoList = this.GetType().GetProperties();
            StringBuilder str = new StringBuilder();
            foreach (var item in infoList.OrderBy(q => q.Name))
            {
                if (item.GetValue(this) != null)
                {

                    table.Add(item.Name.ToLower(), item.GetValue(this));
                }
            }
            return table;
        }
        /// <summary>
        /// 获取支付成功消息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        //public static PayBackInfo GetSuccess(PayInfo info)
        //{
        //    PayBackInfo back = new Web.PayBackInfo();
        //    back.Mer_ID = info.Mer_ID;
        //    back.Sign_Type = info.Sign_Type;
        //    back.Version = info.Version;
        //    back.Order_ID = info.Order_ID;
        //    back.Mer_Date = DateTime.Now.ToString("yyyyMMdd");
        //    back.Ret_Code = "0000";
        //    back.Ret_Msg = "支付成功";

        //    return back;
        //}
    }
}