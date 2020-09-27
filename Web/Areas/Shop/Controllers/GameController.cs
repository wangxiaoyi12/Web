using Business;
using Common;
using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.Shop.Controllers
{
    public class GameController : ShopBaseController
    {
        // GET: Shop/Game
        #region 大转盘
        public ActionResult Wheel()
        {
            return View();
        }
        public ActionResult ToWheel(int id)
        {
            //var memberid = User_Shop.GetMemberID();
            try
            {
                var memberid = User_Shop.GetMemberID();
                var member = DB.Member_Info.FindEntity(memberid);
                #region 验证会员是否有抽奖机会
                // 1.比如 条件 ，每天免费抽3次
                var today = DateTime.Now.Date;
                var count = DB.ShopBigWheelLog.Count(a => a.MemberID == memberid && a.BID == id && a.CreateTime >= today);
                if (count >= 3)
                {
                    return Error("每天免费3次抽奖机会已用完，不能再次抽奖");
                }
                #endregion
                #region 抽奖
                var model = new ShopBigWheelLog()
                {
                    BID = id,
                    CreateTime = DateTime.Now,
                    IP = WebTools.GetHostAddress(),
                    MemberCode = member.Code,
                    NickName = member.NickName,
                    MemberID = member.MemberId,
                };
                var r = DB.ShopBigWheelLog.Insert(model);
                if (r)
                {
                    if (model != null)
                    {
                        #region 抽奖过程
                        var details = DB.ShopBigWheelDetail.Where(a => a.BID == model.BID);
                        ShopBigWheelDetail curResult = null;
                        foreach (var item in details)
                        {
                            if (item.Probability <= 0)
                            {
                                continue;
                            }
                            else if (item.Probability >= 1)
                            {
                                curResult = item;
                                break;
                            }
                            else
                            {
                                var big = 1000;
                                var n = item.Probability * big; //先放大1000倍
                                var random = DB.Random.Next(0, big + 1);  //1-1000之间随机一个数，如果这个数<=n ,中奖
                                if (random <= n)
                                {
                                    curResult = item;
                                    break;
                                }
                            }
                        }
                        #endregion
                        #region 奖结果赋于model

                        if (curResult == null)
                        {
                            model.ResultID = null;
                            model.Result = "未中奖";
                            model.Desc = "谢谢参与，再接再厉";
                        }
                        else
                        {
                            model.ResultID = curResult.ID;
                            model.Result = curResult.Name;
                            model.Desc = curResult.Desc;
                        }
                        var re = DB.ShopBigWheelLog.Update(model);
                        if (re)
                        {
                            switch (model.Result)
                            {
                                case "一等奖":
                                    return Success("3037,恭喜您中" + model.Result);
                                case "二等奖":
                                    return Success("2945,恭喜您中" + model.Result);
                                case "三等奖":
                                    return Success("3215,恭喜您中" + model.Result);
                                case "四等奖":
                                    return Success("3127,恭喜您中" + model.Result);
                                default:
                                    break;
                            }
                        }
                        #endregion
                    }
                    //var obj = new{angle = "2994",prize = "谢谢参与，请再接再厉",prizename = "谢谢参与"};
                    return Success("2994,谢谢参与");
                }
                else
                {
                    return Error("抽奖失败");
                }
                #endregion

            }
            catch (Exception e)
            {
                return Error(e);
            }
        }

        #region 抽奖记录
        public PartialViewResult Log(int id)
        {
            ViewBag.ID = id;
            return PartialView();
        }
        #endregion
        #endregion

    }
}