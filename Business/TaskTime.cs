using Common;
using DataBase;
using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Business
{
    public class TaskTime
    {
        /// <summary>
        /// 定时任务执行的内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        public void TimerHanlder()
        {
            #region 这里写你的定时任务要执行的程序代码（数据库操作，不要使用 Business.DB,请重新自定义个上下文）
            using (var DB = new DbMallEntities())
            {
                var list = DB.ShopOrders.Where(p => p.Type == "未挂卖").ToList();
                foreach (var item in list)
                {
                    if ((int)Math.Floor((DateTime.Now - item.SubmitTime).TotalHours) >= 48)
                    {
                        item.State = ShopEnum.OrderState.Cancel.GetHashCode();
                        item.Type = "已超时";
                        var member = DB.Member_Info.Where(p => p.MemberId == item.MemberID).FirstOrDefault();
                        member.Commission = member.Commission + item.RealScore;
                        member.CommissionSum = member.CommissionSum + item.RealScore;
                        member.Scores = member.Scores + item.RealCongXiao;
                    }
                }
                var listMember = DB.ShopOrders.Where(p => p.Type == "挂卖中" && p.State == 40).ToList();
                foreach (var item in listMember)
                {
                    if ((int)Math.Floor((DateTime.Now - item.SubmitTime).TotalHours) >= 168)
                    {
                        item.State = ShopEnum.OrderState.Finish.GetHashCode();
                        item.Type = "挂卖成功";
                        item.FinishTime = DateTime.Now;
                        var member = DB.Member_Info.Where(p => p.MemberId == item.MemberID).FirstOrDefault();
                        //DB.Jiang.InsertFin(member, member, 800, "挂卖奖励积分", "挂卖奖励积分");//积分
                        InsertFin(DB, member, member, 2100, "委托奖励收益", "委托奖励收益");//收益
                        //分享消费奖
                        FenXiang(DB, member);
                        //辅导奖
                    }
                }
                DB.SaveChanges();
            }
            #endregion
        }
        public void UpdateOrder()
        {
            #region 这里写你的定时任务要执行的程序代码（数据库操作，不要使用 Business.DB,请重新自定义个上下文）

            var dt = DateTime.Now;
            var list = DB.ShopOrder.Where(p => p.State == 3 && SqlFunctions.DateDiff("day", p.PostTime, dt)>= 7).ToList();
            foreach (var item in list)
            {
                ShopOrder model = DB.ShopOrder.FindEntity(item.GUID);
                model.State = ShopEnum.OrderState.Finish.GetHashCode(); //确认收货
                model.FinishTime = DateTime.Now;
                DB.ShopOrder.Update(model);
                ////3.结算系统处理
                //DB.ShopOrder.OrderSettlement(guid);
                //if (model.OrderType != "积分优惠券")
                //{
                //    DB.Jiang.GiveJiang(DB.Member_Info.FindEntity(model.MemberID), model);
                //}
            }
 #endregion
        }
       
        #region 分红奖
        public void FenHong()
        {
            DateTime dt = DateTime.Now.Date;

            //if (DB.SysLogs.Any(a => a.Description == "分红奖发放" && a.CreateTime >= dt))
            //{
            //    LogHelper.Info(dt + "已经执行过分红奖发放功能");
            //    return;
            //}
            //if (DateTime.Now.Day != 1)
            //{
            //    LogHelper.Info(dt + "今天不是1号，不发放分红奖金");
            //    return;
            //}
            var list = DB.Member_Info.Where(p => p.FHSum>0 && p.MemberLevelId==3).ToList();
            foreach (var item in list)
            {
                Member_Info model = DB.Member_Info.FindEntity(item.MemberId);
                var amount = model.FHSum * DB.XmlConfig.XmlSite.FenHongBi / 100m;
                if(amount>0)
                {
                    DB.Jiang.InsertFin(model, model, amount.Value, "分红奖", "分红奖");
                }
            }
            DB.SysLogs.setSysLogsData1("00", "定时任务", "Edit", "分红奖发放");

        }
        #endregion


        #region 突出贡献奖
        public void TuChu()
        {
            DateTime dt = DateTime.Now.Date;

            //if (DB.SysLogs.Any(a => a.Description == "突出贡献奖发放" && a.CreateTime >= dt))
            //{
            //    LogHelper.Info(dt + "已经执行过突出贡献奖发放功能");
            //    return;
            //}
            //if (DateTime.Now.Day != 1)
            //{
            //    LogHelper.Info(dt + "今天不是1号，不发放突出贡献奖金");
            //    return;
            //}
            var list = DB.Member_Info.Where(p => p.LAmount > 0 && p.IsLockFen.Value).ToList();
            foreach (var item in list)
            {
                Member_Info model = DB.Member_Info.FindEntity(item.MemberId);
                var amount = model.LAmount * DB.XmlConfig.XmlSite.TuChuBiLi / 100m;
                if (amount > 0)
                {
                    DB.Jiang.InsertFin(model, model, amount.Value, "突出贡献奖", "突出贡献奖");
                }
            }
            DB.SysLogs.setSysLogsData1("00", "定时任务", "Edit", "突出贡献奖发放");

        }
        #endregion


        #region 奖金发放
        public void FaFang()
        {
            DateTime dt = DateTime.Now.Date;

            if (DB.SysLogs.Any(a => a.Description == "奖金发放" && a.CreateTime >= dt))
            {
                LogHelper.Info(dt + "已经执行过奖金发放功能");
                return;
            }
            //if (DateTime.Now.DayOfWeek == DayOfWeek.Monday )
            //{
            //     LogHelper.Info(dt + "今天不是周一不发放奖金");
            //    return;
            //}
            var list = DB.Fin_Info.Where(p => p.IsSettlement==false).ToList();
            foreach (var item in list)
            {
                Member_Info model = DB.Member_Info.FindEntity(item.MemberId);
                model.Coins += item.RealAmount;
                model.CommissionSum += item.RealAmount;
                DB.Fin_LiuShui.AddLS(model.MemberId, item.RealAmount.Value, item.TypeName,"奖金");
                DB.Member_Info.Update(model);
                Fin_Info finmodel = DB.Fin_Info.FindEntity(item.FinId);
                finmodel.IsSettlement = true;
                DB.Fin_Info.Update(finmodel);

             
            }
            DB.SysLogs.setSysLogsData1("00", "定时任务", "Edit", "奖金发放");

        }
        #endregion

        #region 分享消费奖
        public JsonHelp FenXiang(DbMallEntities db, Member_Info member)
        {
            JsonHelp json = new JsonHelp() { IsSuccess = true, Msg = "" };
            var Recommend = db.Member_Info.Where(p => p.MemberId == member.RecommendId).FirstOrDefault();
            for (int i = 1; i <= 2; i++)
            {
                if (Recommend == null)
                {
                    return json;
                }
                decimal? amount = 0;
                if (i == 1)
                {
                    amount = System.DB.XmlConfig.XmlSite.FenXiang1;
                }
                else if (i == 2)
                {
                    amount = System.DB.XmlConfig.XmlSite.FenXiang1;
                }
                if (amount > 0)
                {
                    InsertFin(db, Recommend, member, amount.Value, "分享消费奖", "分享消费奖");
                    FuDaoJ(db, Recommend, amount.Value);
                }
                Recommend = db.Member_Info.Where(p => p.MemberId == Recommend.RecommendId).FirstOrDefault();
            }
            return json;
        }
        #endregion

        #region 辅导奖
        public JsonHelp FuDaoJ(DbMallEntities db, Member_Info member, decimal Money)
        {
            JsonHelp json = new JsonHelp() { IsSuccess = true, Msg = "" };
            var Recommend = DB.Member_Info.FindEntity(member.RecommendId);
            if (Recommend != null)
            {
                var count = DB.Member_Info.Count(p => p.RecommendId == Recommend.MemberId && p.IsActive == "已激活");
                if (count >= 3)
                {
                    decimal? amount = Money * System.DB.XmlConfig.XmlSite.FuDao / 100m;
                    if (amount > 0)
                    {
                        InsertFin(db, Recommend, member, amount.Value, "辅导奖", "辅导奖");
                    }
                }

            }
            return json;
        }
        #endregion

        #region 增加奖金
        /// <summary>
        /// 增加奖金
        /// </summary>
        /// <param name="member">要增加奖金的会员,</param>
        /// <param name="refMember">相关的会员,</param>
        /// <param name="amount">金额</param>
        /// <param name="db">数据库连接实例</param>
        /// <param name="typeName">奖金类型名称</param>
        /// <param name="Comment">奖金描述</param>
        /// <returns></returns>
        public JsonHelp InsertFin(DbMallEntities db, Member_Info member, Member_Info refMember, decimal amount, string typeName, string Comment)
        {
            JsonHelp json = new JsonHelp(true);
            if (amount < 0)
            {
                json.Msg = "奖金不能小于0";
                return json;
            }
            else if (amount == 0)
            {
                //奖金为0，直接返回，不向数据库增加数据                
                json.Msg = "奖金为0";
                return json;
            }
            //检查是否锁定此会员不得奖
            if (member.IsLockCommission == true)
            {
                json.Msg = "此会员已锁定得奖，奖金不能分配";
                return json;
            }
            var Poundage = System.DB.XmlConfig.XmlSite.Poundage;
            var ChongXiao = System.DB.XmlConfig.XmlSite.ChongXiao;


            //插入费用明细                        
            var mFin = new Fin_Info();
            mFin.MemberId = member.MemberId;
            mFin.MemberCode = member.Code;
            mFin.NickName = member.NickName;
            mFin.Amount = amount;
            if (typeName == "挂卖奖励收益")
            {
                mFin.Poundage = 0;
                mFin.CongXiao = 0;
                if (typeName == "挂卖奖励收益")
                {
                    mFin.RealAmount = 2100;
                }
            }
            else
            {
                mFin.Poundage = 0;
                mFin.CongXiao = 0;
                mFin.RealAmount = mFin.Amount;
            }
            mFin.TypeName = typeName;
            mFin.Comment = string.IsNullOrEmpty(Comment) ? typeName : Comment;
            mFin.RefMemberId = refMember.MemberId;
            mFin.RefMemberCode = refMember.Code;
            mFin.RefNickName = refMember.NickName;
            mFin.CreateTime = DateTime.Now;
            mFin.IsSettlement = true;
            mFin.SettlementTime = mFin.CreateTime;
            db.Fin_Info.Add(mFin);
            if (typeName == "挂卖奖励收益")
            {
                if (typeName == "挂卖奖励收益")
                {
                    member.Commission += Convert.ToDecimal(mFin.RealAmount);
                    member.CommissionSum += Convert.ToDecimal(mFin.RealAmount);
                    Fin_LiuShui _liushui = new Fin_LiuShui();
                    //收益
                    _liushui.MemberId = member.MemberId;
                    _liushui.Code = member.Code;
                    _liushui.NickName = member.NickName;
                    _liushui.Type = "流水账单";
                    _liushui.Comment = "收益(+)";
                    _liushui.Amount = mFin.RealAmount;
                    _liushui.CreateTime = DateTime.Now;
                    DB.Fin_LiuShui.Insert(_liushui);
                    //累计收益
                    _liushui.MemberId = member.MemberId;
                    _liushui.Code = member.Code;
                    _liushui.NickName = member.NickName;
                    _liushui.Type = "流水账单";
                    _liushui.Comment = "累计收益(+)";
                    _liushui.Amount = mFin.RealAmount;
                    _liushui.CreateTime = DateTime.Now;
                }
            }
            else
            {
                member.Commission += mFin.RealAmount;
                member.CommissionSum += mFin.RealAmount;
                //收益
                Fin_LiuShui _liushui = new Fin_LiuShui();
                _liushui.MemberId = member.MemberId;
                _liushui.Code = member.Code;
                _liushui.NickName = member.NickName;
                _liushui.Type = "流水账单";
                _liushui.Comment = "收益(+)";
                _liushui.Amount = mFin.RealAmount;
                _liushui.CreateTime = DateTime.Now;
                //累计收益
                _liushui.MemberId = member.MemberId;
                _liushui.Code = member.Code;
                _liushui.NickName = member.NickName;
                _liushui.Type = "流水账单";
                _liushui.Comment = "累计收益(+)";
                _liushui.Amount = mFin.RealAmount;
                _liushui.CreateTime = DateTime.Now;
            }
            json.Msg = "插入奖金成功";
            return json;
        }
        #endregion
    }
}
