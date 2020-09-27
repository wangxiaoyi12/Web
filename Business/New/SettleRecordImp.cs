using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataBase;
using EntityFramework.Extensions;

namespace Business
{
    /// <summary>
    /// 周结算，月计算处理
    /// </summary>
    public class SettleRecordImp : EFBase<DataBase.SettleRecord>
    {

        //获取周一  
        private DateTime getMonday()
        {
            DateTime now = DateTime.Now;
            DateTime temp = new DateTime(now.Year, now.Month, now.Day);
            int count = now.DayOfWeek - DayOfWeek.Monday;
            if (count == -1) count = 6;

            return temp.AddDays(-count);
        }
        //获取周天  
        private DateTime getSunday()
        {
            DateTime now = DateTime.Now;
            DateTime temp = new DateTime(now.Year, now.Month, now.Day);
            int count = now.DayOfWeek - DayOfWeek.Sunday;
            if (count != 0) count = 7 - count;
            return temp.AddDays(count);
        }

        /// <summary>
        /// 判断本周，是否结算
        /// </summary>
        /// <returns></returns>
        public bool IsSendWeek()
        {
            DateTime start = getMonday();
            DateTime end = getSunday();
            return Any(q => q.CreateTime >= start && q.CreateTime < end && q.Type == 1);
        }


        /// <summary>
        /// 判断本月是否发放过
        /// </summary>
        /// <returns></returns>
        public bool IsSendMonth()
        {
            DateTime now = DateTime.Now;
            return Any(q => q.CreateTime.Year == now.Year
            && q.CreateTime.Month == now.Month
            && q.Type == 2);
        }

        /// <summary>
        /// 周结算
        /// </summary>
        public void Send(int type)
        {
            using (var tran = BeginTransaction)
            {
                try
                {
                    Xml_Site config = DB.XmlConfig.XmlSite;
                    //周结算，收益
                    string[] typeName = new string[]{
                        "直推一代奖",
                        "直推二代奖",
                        "互助奖",
                    };
                    if (type == Enums.SettleType.月计算.GetHashCode())
                    {
                        typeName = new string[] {
                            "领导奖",
                            //"师长分红",
                            //"社区店分红",
                            "重消奖",
                        };
                    }


                    //1.添加记录
                    SettleRecord record = new SettleRecord();
                    record.CreateTime = DateTime.Now;
                    record.Type = type;
                    //2.处理到账
                    var list = DB.Fin_Info.Where(q => typeName.Contains(q.TypeName)
                    && q.IsSettlement != true);
                    foreach (var item in list)
                    {
                        //到账处理
                        Member_Info member = DB.Member_Info.FindEntity(item.MemberId);
                        member.Commission += item.RealAmount;
                        member.CommissionSum += item.RealAmount;
                        member.ShopCoins += item.Poundage ?? 0;
                        member.CommissionBan += item.Amount ?? 0;

                        //如果操作1万，赠送路由积分
                        if (member.CommissionBan >= config.TourScoresStep)
                        {
                            member.CommissionBan -= config.TourScoresStep;
                            member.TourScores += config.TourScores;
                        }
                        DB.Member_Info.Update(member);

                        //修改记录状态
                        item.IsSettlement = true;
                        item.SettlementTime = DateTime.Now;
                        DB.Fin_Info.Update(item);

                        record.AllAmount += item.Amount ?? 0;
                        record.AllCoins += item.Poundage ?? 0;
                        record.AllCommission += item.RealAmount ?? 0;
                        record.AllCount++;
                    }

                    if (base.Insert(record) == false)
                        throw new Exception("添加记录失败");

                    //提交处理
                    tran.Complete();
                }
                catch (Exception ex)
                {
                    DB.Rollback();
                    throw ex;
                }
            }
        }


        #region 查询
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="id">当前用户id</param>
        /// <param name="start">开始日期</param>
        /// <param name="end">结束日期</param>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        public List<SettleRecord> getDataSource(DateTime? start, DateTime? end, out int total, int _start, int pageSize)
        {
            var query = Where();

            if (start != null)
            {
                query = query.Where(a => a.CreateTime >= start);
            }
            if (end != null)
            {
                end = end.Value.AddDays(1);
                query = query.Where(a => a.CreateTime < end);
            }

            var data = query.OrderByDescending(p => p.CreateTime).Skip(_start).Take(pageSize).ToList();
            total = data.Count;
            if (data.Count >= pageSize)
            {
                total = query.Count();
            }
            return data;
        }
        #endregion
    }
}
