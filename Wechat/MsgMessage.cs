using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using DataBase;
using Newtonsoft.Json.Linq;

namespace Wechat
{
    /// <summary>
    /// 模板消息，发送定义
    /// </summary>
    public class MsgMessage:BaseManage
    {
        private string GetUrl()
        {
           string url = $"https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={this.config.Access_Token}";


            //string url = $"https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=7_r7sMIdzZZdBhyMGTxuw0Uc-0twwPL148vmZTDKrmhZuNnhvq05SK6npsIsdzioq8cMJpP6nRK0hZxiZtPmk92q15cm29RJnMxCj8Bnxr372j-nwhmxcYElefkw0QNFcAEAHEW";

            return url;
        }
        /// <summary>
        /// 提现成功消息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="openid"></param>
        public void Send_DrawSuccess(Fin_Draw model, string openid, string openUrl)
        {
            string json = new
            {
                touser = openid,
                template_id = "pkVbcnEmaGqMeowKl2y9SY8poWQjmF6xD2oUmw86igw",
                url = openUrl,
                topcolor = "#FF0000",
                data = new
                {
                    first = new
                    {
                        color = "#173177",
                        value = "您好，您有一条提现成功到账通知"
                    },
                    keyword1 = new
                    {
                        color = "#0000ff",
                        value = model.DrawAmount.Value.ToString("0.##")
                    },
                    keyword2 = new
                    {
                        color = "#173177",
                        value = model.CreateTime.Value.ToString("yyyy-MM-dd HH:ss")
                    },
                    keyword3 = new
                    {
                        color = "#0000ff",
                        value = model.Amount.Value.ToString("0.##")
                    },
                    keyword4 = new
                    {
                        color = "#173177",
                        value = model.BankCode
                    },
                    remark = new
                    {
                        color = "#173177",
                        value = "请及时核对银行账单"
                    }
                }
            }.ToJsonString();
            string result = NetHelper.Post(GetUrl(), json);
            JObject obj = JObject.Parse(result);
            if ((int)obj["errcode"] != 0)
                throw new Exception("发送提现消息失败：" + result);
        }
        /// <summary>
        /// 提现失败消息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="openid"></param>
        public void Send_DrawError(Fin_Draw model, string openid, string openUrl)
        {
            string json = new
            {
                touser = openid,
                template_id = "Lz04xXz3Lp5j7_U86mZo1ETRs9tF-Y_HhR7izDBIKso",
                url = openUrl,
                topcolor = "#D7D8DA",
                data = new
                {
                    first = new
                    {
                        color = "#173177",
                        value = "您好，您有一条提现失败通知"
                    },
                    keyword1 = new
                    {
                        color = "#173177",
                        value = model.DrawAmount.Value.ToString("0.##")
                    },
                    keyword2 = new
                    {
                        color = "#173177",
                        value = model.CreateTime.Value.ToString("yyyy-MM-dd HH:ss")
                    },
                    keyword3 = new
                    {
                        color = "#173177",
                        value = model.Summary
                    },
                    remark = new
                    {
                        color = "#173177",
                        value = "如有疑问请联系管理员"
                    }
                }
            }.ToJsonString();
            string result = NetHelper.Post(GetUrl(), json);
            JObject obj = JObject.Parse(result);
            if ((int)obj["errcode"] != 0)
                throw new Exception("发送提现消息失败：" + result);
        }
        /// <summary>
        /// 下单后，通知社区店
        /// </summary>
        /// <param name="model"></param>
        /// <param name="openid"></param>
        public void Send_Dian(ShopOrder model, string openid, string openUrl)
        {
            string json = new
            {
                touser = openid,
                template_id = "QAOj8o-jx54SKWi5ZLRx3czRYyQ3A-Nc0kD7J_gcfyg",
                url = openUrl,
                topcolor = "#00ff00",
                data = new
                {
                    first = new
                    {
                        color = "#173177",
                        value = "您好，您有待发货订单提醒"
                    },
                    keyword1 = new
                    {
                        color = "#173177",
                        value = model.OrderCode
                    },
                    keyword2 = new
                    {
                        color = "#173177",
                        value = model.MemberCode
                    },
                    keyword3 = new
                    {
                        color = "#173177",
                        value = model.SubmitTime.ToString("yyyy-MM-dd HH:mm")
                    },
                    remark = new
                    {
                        color = "#173177",
                        value = "请登录系统及时处理发货"
                    }
                }
            }.ToJsonString();
            string result = NetHelper.Post(GetUrl(), json);
            JObject obj = JObject.Parse(result);
            if ((int)obj["errcode"] != 0)
                throw new Exception("发送提现消息失败：" + result);
        }

    }
}
