using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DataBase;
using Business;
using Wechat;

namespace Web.Areas.Member_Finance.Controllers
{
    public class ScoreRecordController : Web.Controllers.MemberBaseController
    {
        // GET: Member_Finance/ScoreRecord
        public ActionResult Index()
        {
            return View();
        }

        #region 查询
        public string getDataSource(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.ScoreRecord.getDataSource(CurrentUser.Id, startTime, end, key, out total, start, length);
            return ToPage(list, total, start, length, draw);
        }
        #endregion

        /// <summary>
        /// 充值
        /// </summary>
        /// <returns></returns>
        public ActionResult Detail(int id=0)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult Save(decimal scores, decimal amount, int id = 0)
        {
            //1.添加充值记录
            ScoreRecord record = null;
            Member_Info curUser = DB.Member_Info.FindEntity(CurrentUser.Id);
            if (id == 0)
            {
                Xml_Site config = DB.XmlConfig.XmlSite;
                if (amount <= 0)
                    return Content("充值金额要大于0");
                if (scores % config.ScoreMultiple != 0)
                    return Content("充值数量要是" + config.ScoreMultiple + "的数量");
                record = new ScoreRecord();
                record.CreateTime = DateTime.Now;
                record.Scores = scores;
                record.Amount = amount;
                record.State = Enums.ScoreState.待付款.GetHashCode();
                record.MemberID = curUser.MemberId;
                record.MemberCode = curUser.Code;
                record.MemberName = curUser.NickName;
                DB.ScoreRecord.Insert(record);
            }
            else
            {
                record = DB.ScoreRecord.FindEntity(id);
            }


            //判断是否是微信端
            if (Url_Mobile.IsWechat() && string.IsNullOrEmpty(curUser.OpenID) == false)
            {
                Xml_Site config = DB.XmlConfig.XmlSite;
                JsonHelp json = new JsonHelp(true);
                try
                {
                    //使用微信api 下订单
                    PayManage _manage = new PayManage();
                    string tradetype = Url_Mobile.IsWechat() ? "JSAPI" : "NATIVE";
                    _manage.OnCreateOrderSuccess += (result) =>
                    {

                        json.Msg = result.Prepay_ID;
                        json.ReUrl = result.Code_Url;//二维码地址
                                                     //签名处理
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        int timeStamp = AccessTokenManage.TimeStamp;
                        dic.Add("appId", result.APPID);
                        dic.Add("timeStamp", timeStamp.ToString());
                        dic.Add("nonceStr", timeStamp.ToString());
                        dic.Add("package", $"prepay_id={result.Prepay_ID}");
                        dic.Add("signType", "MD5");
                        json.Data = new
                        {
                            appId = result.APPID,
                            timeStamp = timeStamp.ToString(),
                            nonceStr = timeStamp.ToString(),
                            package = $"prepay_id={result.Prepay_ID}",
                            signType = "MD5",
                            paySign = ConfigInfo.GetPaySign(dic)
                        };
                    };
                    _manage.BackHandleUrl = $"http://{Request.Url.Host}/ajax/ScoreWX";//指定回调地址
                    _manage.CreateOrder(DateTime.Now.ToString("MMddms") + "_" + record.ID, config.GetAmount(amount), curUser.OpenID, tradetype, "竹笛商城积分充值");
                }
                catch (Exception ex)
                {
                    json.IsSuccess = false;
                    json.Msg = ex.Message;

                    Common.LogHelper.Debug("积分充值微信端出错：" + ex.Message);
                    Common.LogHelper.Debug(ex.StackTrace);
                }

                return Json(json);
            }
            else
            {
                //2.获取充值地址
                //string url = PayHelper.GetPayUrl(DateTime.Now.ToString("MMddms") + "_" + record.ID, amount, "积分充值", true);
                //return Redirect(url);
                return Redirect("支付接口申请中....");
            }
        }
    }
}