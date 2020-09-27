using Common;
using DataBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementation
{
    public class Fin_InfoImp : EFBase<DataBase.Fin_Info>
    {
        #region 获取收益明细列表
        /// <summary>
        /// 获取收益明细列表
        /// </summary>
        /// <param name="id">当前用户id</param>
        /// <param name="start">开始日期</param>
        /// <param name="end">结束日期</param>
        /// <param name="key">关键字</param>
        public List<Fin_Info> getDataSource(string id, DateTime? start, DateTime? end, string key, out int total, int _start, int pageSize)
        {
            var query = Where(a => true);
            if (!string.IsNullOrEmpty(id))
            {
                query = query.Where(a => a.MemberId == id);
            }
            if (start != null)
            {
                query = query.Where(a => a.CreateTime >= start);
            }
            if (end != null)
            {
                end = end.Value.AddDays(1);
                query = query.Where(a => a.CreateTime < end);
            }
            if (!string.IsNullOrEmpty(key))
            {
                key = key.Trim();
                if (!string.IsNullOrEmpty(id))
                {
                    query = query.Where(a => a.RefMemberCode.Contains(key) || a.RefNickName.Contains(key) || a.Comment.Contains(key) || a.TypeName.Contains(key) || a.MemberCode.Contains(key) || a.NickName.Contains(key));
                }
                else
                {
                    query = query.Where(a => a.MemberCode.Contains(key) || a.NickName.Contains(key) || a.Comment.Contains(key) || a.TypeName.Contains(key) || a.RefMemberCode.Contains(key) || a.RefNickName.Contains(key));
                }
            }

            var data = query.OrderByDescending(p => p.CreateTime).ThenByDescending(a => a.FinId).Skip(_start).Take(pageSize).ToList();
            total = data.Count;
            if (data.Count >= pageSize)
            {
                total = query.Count();
            }
            return data;
        }
        #endregion

        #region 前台获取收益总表的列表
        /// <summary>
        /// 前台获取收益总表的列表
        /// </summary>
        /// <param name="id">当前用户id</param>
        /// <param name="start">开始日期</param>
        /// <param name="end">结束日期</param>
        /// <param name="key">关键字</param>
        public IEnumerable getSummaryDataSource(string id, DateTime? start, DateTime? end, string key, out int total, int _start, int pageSize)
        {
            if (end != null)
            {
                end = end.Value.AddDays(1);
            }
            var query = DB.Fin_Info.Where(a => true
            && (string.IsNullOrEmpty(id) ? true : a.MemberId == id)
            && (start == null ? true : a.CreateTime >= start)
            && (end == null ? true : a.CreateTime < end)
            && (string.IsNullOrEmpty(key) ? true : (a.MemberCode.IndexOf(key) > -1 || a.NickName.IndexOf(key) > -1)))
                .GroupBy(a => new { a.MemberId, a.MemberCode, a.NickName, GroupDate = DbFunctions.TruncateTime(a.CreateTime) })
                .Select(a => new
                {
                    a.Key.MemberCode,
                    a.Key.MemberId,
                    a.Key.NickName,
                    a.Key.GroupDate,



                    推广奖 = a.Where(p => p.TypeName == "推广奖").Sum(p => p.Amount) ?? 0,
                    一代平级奖 = a.Where(p => p.TypeName == "一代平级奖").Sum(p => p.Amount) ?? 0,
                    超越奖 = a.Where(p => p.TypeName == "超越奖").Sum(p => p.Amount) ?? 0,
                    平级奖 = a.Where(p => p.TypeName == "平级奖").Sum(p => p.Amount) ?? 0,
                    分红奖 = a.Where(p => p.TypeName == "分红奖").Sum(p => p.Amount) ?? 0,
                    突出贡献奖 = a.Where(p => p.TypeName == "突出贡献奖").Sum(p => p.Amount) ?? 0,
                    代理推荐奖 = a.Where(p => p.TypeName == "代理推荐奖").Sum(p => p.Amount) ?? 0,
                    分红 = a.Where(p => p.TypeName == "分红").Sum(p => p.Amount) ?? 0,
                    代理津贴 = a.Where(p => p.TypeName == "代理津贴").Sum(p => p.Amount) ?? 0,
                    拼团奖 = a.Where(p => p.TypeName == "拼团奖").Sum(p => p.Amount) ?? 0,
                    购物返利 = a.Where(p => p.TypeName == "购物返利").Sum(p => p.Amount) ?? 0,
                    //委托奖励积分 = a.Where(p => p.TypeName == "委托奖励积分").Sum(p => p.Amount) ?? 0,
                    委托奖励收益 = a.Where(p => p.TypeName == "委托奖励收益").Sum(p => p.Amount) ?? 0,
                    分享消费奖 = a.Where(p => p.TypeName == "分享消费奖").Sum(p => p.Amount) ?? 0,
                    辅导奖 = a.Where(p => p.TypeName == "辅导奖").Sum(p => p.Amount) ?? 0,
                    小计 = a.Sum(p => p.Amount) ?? 0,
                    税 = a.Sum(p => p.Poundage) ?? 0,
                    重消 = a.Sum(p => p.CongXiao) ?? 0,
                    实际金额 = a.Sum(p => p.RealAmount) ?? 0
                });

            var data = query.OrderByDescending(a => a.GroupDate).Skip(_start).Take(pageSize).ToList();
            total = data.Count;
            if (data.Count >= pageSize)
            {
                total = query.Count();
            }
            return data;
        }
        #endregion

        #region 后台获取收益总表的列表（根据日期统计）
        /// <summary>
        /// 后台获取收益总表的列表
        /// </summary>
        /// <param name="id">当前用户id</param>
        /// <param name="start">开始日期</param>
        /// <param name="end">结束日期</param>
        /// <param name="key">关键字</param>
        public IEnumerable getSummaryTotalDataSource(DateTime? start, DateTime? end, out int total, int _start, int pageSize)
        {
            if (end != null)
            {
                end = end.Value.AddDays(1);
            }
            //1.分组查询日期并获取分页数据
            var query = DB.Fin_Info.Where();
            if (start != null)
                query = query.Where(q => q.CreateTime >= start);
            if (end != null)
            {
                end.Value.AddDays(1);
                query = query.Where(q => q.CreateTime < end);
            }
            var group = query.GroupBy(q => q.GroupDate);
            total = group.Count();

            //分页
            var pageData = group.OrderByDescending(q => q.Key)
                     .Skip(_start)
                     .Take(pageSize)
                     .Select(q => new
                     {
                         GroupDate = q.Key
                     }).ToList();

            //2.根据日期汇总收益
            List<object> list = new List<object>();
            foreach (var item in pageData)
            {
                var a = DB.Fin_Info.Where(q => q.GroupDate == item.GroupDate);
                object result = new
                {

                    GroupDate = new DateTime(1900, 1, 1).AddDays(item.GroupDate).ToString("yyyy-MM-dd"),
                    一代平级奖 = a.Where(p => p.TypeName == "一代平级奖").Sum(p => p.Amount) ?? 0,
                    超越奖 = a.Where(p => p.TypeName == "超越奖").Sum(p => p.Amount) ?? 0,
                    推广奖 = a.Where(p => p.TypeName == "推广奖").Sum(p => p.Amount) ?? 0,
                    平级奖 = a.Where(p => p.TypeName == "平级奖").Sum(p => p.Amount) ?? 0,
                    分红奖 = a.Where(p => p.TypeName == "分红奖").Sum(p => p.Amount) ?? 0,
                    突出贡献奖 = a.Where(p => p.TypeName == "突出贡献奖").Sum(p => p.Amount) ?? 0,
                    代理推荐奖 = a.Where(p => p.TypeName == "代理推荐奖").Sum(p => p.Amount) ?? 0,
                    分红 = a.Where(p => p.TypeName == "分红").Sum(p => p.Amount) ?? 0,
                    代理津贴 = a.Where(p => p.TypeName == "代理津贴").Sum(p => p.Amount) ?? 0,
                    拼团奖 = a.Where(p => p.TypeName == "拼团奖").Sum(p => p.Amount) ?? 0,
                    购物返利 = a.Where(p => p.TypeName == "购物返利").Sum(p => p.Amount) ?? 0,
                    //委托奖励积分 = a.Where(p => p.TypeName == "委托奖励积分").Sum(p => p.Amount) ?? 0,
                    委托奖励收益 = a.Where(p => p.TypeName == "委托奖励收益").Sum(p => p.Amount) ?? 0,
                    分享消费奖 = a.Where(p => p.TypeName == "分享消费奖").Sum(p => p.Amount) ?? 0,
                    辅导奖 = a.Where(p => p.TypeName == "辅导奖").Sum(p => p.Amount) ?? 0,
                    小计 = a.Sum(p => p.Amount) ?? 0,
                    税 = a.Sum(p => p.Poundage) ?? 0,
                    重消 = a.Sum(p => p.CongXiao) ?? 0,
                    实际金额 = a.Sum(p => p.RealAmount) ?? 0
                };
                list.Add(result);
            }
            return list;

            #region old
            //           var query = DB.Fin_Info.Where(a => a.IsSettlement == true && (start == null ? true : a.CreateTime >= start)
            //&& (end == null ? true : a.CreateTime < end))
            //    .GroupBy(a => new { GroupDate = DbFunctions.TruncateTime(a.CreateTime) })
            //    .Select(a => new
            //    {
            //        a.Key.GroupDate,
            //        直推一代奖 = a.Where(p => p.TypeName == "直推一代奖").Sum(p => p.Amount) ?? 0,
            //        直推二代奖 = a.Where(p => p.TypeName == "直推二代奖").Sum(p => p.Amount) ?? 0,
            //        互助奖 = a.Where(p => p.TypeName == "互助奖").Sum(p => p.Amount) ?? 0,
            //        拼团奖 = a.Where(p => p.TypeName == "拼团奖").Sum(p => p.Amount) ?? 0,

            //        师长分红 = a.Where(p => p.TypeName == "师长分红").Sum(p => p.Amount) ?? 0,
            //        重消奖 = a.Where(p => p.TypeName == "重消奖").Sum(p => p.Amount) ?? 0,

            //        推店奖 = a.Where(p => p.TypeName == "推店奖").Sum(p => p.Amount) ?? 0,
            //        服务费 = a.Where(p => p.TypeName == "服务费").Sum(p => p.Amount) ?? 0,

            //        小计 = a.Sum(p => p.Amount),
            //        手续费 = a.Sum(p => p.Poundage),
            //        实际金额 = a.Sum(p => p.RealAmount)
            //    });

            //           var data = query.OrderByDescending(a => a.GroupDate)
            //               .Skip(_start)
            //               .Take(pageSize).ToList();
            //           total = data.Count;
            //           if (data.Count >= pageSize)
            //           {
            //               total = query.Count();
            //           }
            //           return data;
            #endregion

        }
        #endregion

        #region 后台获取收益总表的列表(根据会员统计)
        /// <summary>
        /// 后台获取收益总表的列表2
        /// </summary>
        /// <param name="total"></param>
        /// <param name="_start"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable getSummaryTotalDataSourceByCode(out int total, int _start, int pageSize)
        {
            //1.会员表获取会员分页
            List<Member_Info> memberList = null;
            var memberQuery = DB.Member_Info.Where();
            memberQuery = memberQuery.Where(q => q.IsActive == "已激活");
            //分页
            total = memberQuery.Count();
            memberList = memberQuery.OrderByDescending(q => q.CreateTime)
                .Skip(_start)
                .Take(pageSize)
                .ToList();
            //2.汇总会员的收益
            List<object> list = new List<object>();
            var query = DB.Fin_Info.Where();
            query = query.Where(q => true);
            foreach (var item in memberList)
            {
                var a = query.Where(q => q.MemberId == item.MemberId);
                list.Add(new
                {
                    MemberCode = item.Code,
                    MemberId = item.MemberId,
                    NickName = item.NickName,
                    推广奖 = a.Where(p => p.TypeName == "推广奖").Sum(p => p.Amount) ?? 0,
                    平级奖 = a.Where(p => p.TypeName == "平级奖").Sum(p => p.Amount) ?? 0,
                    一代平级奖 = a.Where(p => p.TypeName == "一代平级奖").Sum(p => p.Amount) ?? 0,
                    超越奖 = a.Where(p => p.TypeName == "超越奖").Sum(p => p.Amount) ?? 0,
                    分红奖 = a.Where(p => p.TypeName == "分红奖").Sum(p => p.Amount) ?? 0,
                    突出贡献奖 = a.Where(p => p.TypeName == "突出贡献奖").Sum(p => p.Amount) ?? 0,
                    代理推荐奖 = a.Where(p => p.TypeName == "代理推荐奖").Sum(p => p.Amount) ?? 0,
                    分红 = a.Where(p => p.TypeName == "分红").Sum(p => p.Amount) ?? 0,
                    代理津贴 = a.Where(p => p.TypeName == "代理津贴").Sum(p => p.Amount) ?? 0,
                    拼团奖 = a.Where(p => p.TypeName == "拼团奖").Sum(p => p.Amount) ?? 0,
                    购物返利 = a.Where(p => p.TypeName == "购物返利").Sum(p => p.Amount) ?? 0,
                    //委托奖励积分 = a.Where(p => p.TypeName == "委托奖励积分").Sum(p => p.Amount) ?? 0,
                    委托奖励收益 = a.Where(p => p.TypeName == "委托奖励收益").Sum(p => p.Amount) ?? 0,
                    分享消费奖 = a.Where(p => p.TypeName == "分享消费奖").Sum(p => p.Amount) ?? 0,
                    辅导奖 = a.Where(p => p.TypeName == "辅导奖").Sum(p => p.Amount) ?? 0,
                    小计 = a.Sum(p => p.Amount) ?? 0,
                    税 = a.Sum(p => p.Poundage) ?? 0,
                    重消 = a.Sum(p => p.CongXiao) ?? 0,
                    实际金额 = a.Sum(p => p.RealAmount) ?? 0
                });
            }
            return list;

            #region old
            //var query = DB.Fin_Info
            //          .Where(a => a.IsSettlement == true)
            //          .GroupBy(a => new { a.MemberId, a.MemberCode, a.NickName })
            //          .Select(a => new
            //          {
            //              a.Key.MemberCode,
            //              a.Key.MemberId,
            //              a.Key.NickName,

            //              直推一代奖 = a.Where(p => p.TypeName == "直推一代奖").Sum(p => p.Amount) ?? 0,
            //              直推二代奖 = a.Where(p => p.TypeName == "直推二代奖").Sum(p => p.Amount) ?? 0,
            //              互助奖 = a.Where(p => p.TypeName == "互助奖").Sum(p => p.Amount) ?? 0,
            //              拼团奖 = a.Where(p => p.TypeName == "拼团奖").Sum(p => p.Amount) ?? 0,

            //              师长分红 = a.Where(p => p.TypeName == "师长分红").Sum(p => p.Amount) ?? 0,
            //              重消奖 = a.Where(p => p.TypeName == "重消奖").Sum(p => p.Amount) ?? 0,

            //              推店奖 = a.Where(p => p.TypeName == "推店奖").Sum(p => p.Amount) ?? 0,
            //              服务费 = a.Where(p => p.TypeName == "服务费").Sum(p => p.Amount) ?? 0,


            //              小计 = a.Sum(p => p.Amount),
            //              手续费 = a.Sum(p => p.Poundage),
            //              实际金额 = a.Sum(p => p.RealAmount)
            //          });

            //var data = query.OrderByDescending(a => a.MemberCode)
            //    .Skip(_start)
            //    .Take(pageSize).ToList();
            //total = data.Count;
            //if (data.Count >= pageSize)
            //{
            //    total = query.Count();
            //}
            //return data;
            #endregion

        }
        #endregion

        #region 本周收益走势图
        public string RealAmountForWeekToLine(string id)
        {
            DataSet ds = new DataSet();
            var data = FindSqlToJson(string.Format(@"SET DATEFIRST 1 SELECT datename(weekday, SettlementTime) as SettlementTime,SUM(RealAmount) as count FROM Fin_Info
                                            WHERE IsSettlement=1 and MemberId='{0}' and (DATEPART(week, SettlementTime) = DATEPART(week, GETDATE())) AND(DATEPART(yy, SettlementTime) = DATEPART(yy, GETDATE()))
                                            group by datename(weekday, SettlementTime) order by SettlementTime desc", id));
            DataTable dt = Common.JsonConverter.JsonToDataTable(data);
            if (dt != null)
            {
                dt.TableName = "本周收益统计图";
                ds.Tables.Add(dt.Copy());
                return EchartsHelp.EchartJsonToMultipleBar(ds, SysDictionary.GetWeek(), "SettlementTime", "count");
            }
            DataColumn dc3 = new DataColumn("count", Type.GetType("System.Int16"));
            DataColumn dc5 = new DataColumn("SettlementTime", Type.GetType("System.String"));
            DataTable dt1 = new DataTable();
            dt1.Columns.Add(dc3);
            dt1.Columns.Add(dc5);
            dt1.TableName = "本周收益统计图";
            ds.Tables.Add(dt1.Copy());
            return EchartsHelp.EchartJsonToMultipleBar(ds, SysDictionary.GetWeek(), "SettlementTime", "count");
        }
        #endregion

        #region 收益结算
        /// <summary>
        /// 收益结算
        /// </summary>
        /// <param name="date"></param>
        /// <param name="db"></param>
        /// <param name="curPrice"></param>
        /// <param name="isBefore">是否结算date以前的</param>
        public void ToSettlement(DateTime date, DbMallEntities db, bool isBefore)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int count = 0;
            var start = date.Date;
            var end = start.AddDays(1);
            var query = db.Fin_Info.Where(a => true && a.CreateTime < end);

            if (isBefore == false)
            {
                query = query.Where(a => a.CreateTime >= start);
            }
            var list = query.ToList();

            var memberids = list.GroupBy(a => a.MemberId).Select(a => a.Key).ToList();
            var members = db.Member_Info.Where(a => memberids.Contains(a.MemberId)).ToList();
            foreach (var item in members)
            {
                #region 更新收益
                var comList = list.Where(a => a.MemberId == item.MemberId).ToList();
                if (comList.Count > 0)
                {
                    foreach (var citem in comList)
                    {
                        citem.IsSettlement = true;
                        citem.SettlementTime = DateTime.Now;
                    }
                    //更新会员收益
                    var realAmount = comList.Sum(a => a.RealAmount) ?? 0;
                    item.Commission += realAmount;
                    item.CommissionSum += realAmount;
                    count++;
                }
                #endregion 
            }
            db.SaveChanges();
            sw.Stop();
            LogHelper.Info(string.Format("收益结算 日期：{2},会员[{0}]个，分配奖[{3}]个，用时：[{1}]]", list.Count, sw.Elapsed, date.ToString("yyyy-MM-dd"), count));
        }
        #endregion

        #region 获得dt—foot统计
        /// <summary>
        /// 获得dt—foot统计
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="id">会员id</param>
        /// <returns></returns>
        public string getFootTotal(DateTime? start, DateTime? end, string id = null)
        {


            if (end != null)
            {
                end = end.Value.AddDays(1);
            }
            var query = DB.Fin_Info.Where(a => true && (start == null ? true : a.CreateTime >= start)
                      && (end == null ? true : a.CreateTime < end));

            if (!string.IsNullOrEmpty(id))
            {
                query = query.Where(a => a.MemberId == id);
            }
            var r = DB.Fin_Info.Where().Take(1).Select(a => new
            {
                推广奖 = query.Where(p => p.TypeName == "推广奖").Sum(p => p.Amount) ?? 0,
                平级奖 = query.Where(p => p.TypeName == "平级奖").Sum(p => p.Amount) ?? 0,
                分红奖 = query.Where(p => p.TypeName == "分红奖").Sum(p => p.Amount) ?? 0,
                一代平级奖 = query.Where(p => p.TypeName == "一代平级奖").Sum(p => p.Amount) ?? 0,
                超越奖 = query.Where(p => p.TypeName == "超越奖").Sum(p => p.Amount) ?? 0,
                突出贡献奖 = query.Where(p => p.TypeName == "突出贡献奖").Sum(p => p.Amount) ?? 0,
                代理推荐奖 = query.Where(p => p.TypeName == "代理推荐奖").Sum(p => p.Amount) ?? 0,
                分红 = query.Where(p => p.TypeName == "分红").Sum(p => p.Amount) ?? 0,
                代理津贴 = query.Where(p => p.TypeName == "代理津贴").Sum(p => p.Amount) ?? 0,
                拼团奖 = query.Where(p => p.TypeName == "拼团奖").Sum(p => p.Amount) ?? 0,
                购物返利 = query.Where(p => p.TypeName == "购物返利").Sum(p => p.Amount) ?? 0,
                //委托奖励积分 = query.Where(p => p.TypeName == "委托奖励积分").Sum(p => p.Amount) ?? 0,
                委托奖励收益 = query.Where(p => p.TypeName == "委托奖励收益").Sum(p => p.Amount) ?? 0,
                分享消费奖 = query.Where(p => p.TypeName == "分享消费奖").Sum(p => p.Amount) ?? 0,
                辅导奖 = query.Where(p => p.TypeName == "辅导奖").Sum(p => p.Amount) ?? 0,
                小计 = query.Sum(p => p.Amount) ?? 0,
                税 = query.Sum(p => p.Poundage) ?? 0,
                重消 = query.Sum(p => p.CongXiao) ?? 0,
                实际金额 = query.Sum(p => p.RealAmount) ?? 0
            }).FirstOrDefault().ToJsonString();
            return r;
        }
        #endregion

        #region 获得支出汇总      
        public decimal? getExpendSum(DateTime? start, DateTime? end)
        {
            if (end != null)
            {
                end = end.Value.AddDays(1);
            }
            return DB.Fin_Info.Where(a => (start == null ? true : a.CreateTime >= start)
                                        && (end == null ? true : a.CreateTime < end))
                                        .Sum(p => p.RealAmount);
        }
        #endregion
    }
}
