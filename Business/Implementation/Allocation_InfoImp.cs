using Common;
using DataBase;
using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementation
{
    public class Allocation_InfoImp : EFBase<DataBase.Allocation_Info>
    {
        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="key"></param>
        /// <param name="total"></param>
        /// <param name="_start"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<Allocation_Info> getDataSource(DateTime? start, DateTime? end, string key, out int total, int _start, int pageSize)
        {
            var query = DB.Allocation_Info.Where();
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
                query = query.Where(a => a.Code.Contains(key));
            }

            var data = query.OrderByDescending(p => p.CreateTime).Skip(_start).Take(pageSize).ToList();
            total = data.Count;
            if (total >= pageSize)
            {
                total = query.Count();
            }
            return data;
        }
        /// <summary>
        /// 计算分红
        /// </summary>
        /// <param name="start">开始日期（包括）</param>
        /// <param name="end">截止日期（不包括）</param>
        /// <returns></returns>
        public Allocation_Info JSFH(DateTime start, DateTime end)
        {
            var entity = new Allocation_Info();
            //合计金额
            end = end.AddDays(1);
            var TotalAmount = DB.Member_Info.Where(a => a.IsActive == "已激活" && a.Code != "admin" && a.ActiveTime >= start && a.ActiveTime < end && a.ActiveAmount != null)
            .Sum(a => a.ActiveAmount) ?? 0;  //排除admin
            var AllocationAmount = TotalAmount * DB.XmlConfig.XmlSite.DayMaxTotalAmountFlower / 100M;
            //合计会员数量
            //var fhdate = DB.XmlConfig.XmlSite.FHDate.Value.AddDays(1);
            var ToList = DB.Member_Info.Where(p => p.IsActive == "已激活" && p.Code != "admin" && p.IsNullActive != true && p.IsLockFen != true).ToList();
            List<Member_Info> list = new List<Member_Info>();
            foreach (var item in ToList)
            {
                var ReCount = DB.Member_Info.Count(p => p.RecommendId == item.MemberId && p.IsActive == "已激活");
                decimal? FengD = 0;
                if (ReCount >= 2)
                {
                    FengD = item.ActiveAmount * 2;
                }
                else if (ReCount >= 1)
                {
                    FengD = item.ActiveAmount * 1.5m;
                }
                else if (ReCount < 1)
                {
                    FengD = item.ActiveAmount;
                }
                if (item.FHSum < FengD)
                {
                    list.Add(item);
                }
            }
            var MemberCount = list.Count();
            //应发收益
            var shouldAmount = 0M;
            if (MemberCount > 0)
            {
                shouldAmount = Math.Round(AllocationAmount / MemberCount, 2);
            }

            entity.TotalAmount = TotalAmount;
            entity.AllocationAmount = AllocationAmount;
            entity.MemberCount = MemberCount;
            entity.ShouldAmount = shouldAmount;
            return entity;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonHelp Save(Allocation_Info entity)
        {
            var json = new JsonHelp(true, "分红成功");
            using (var db = new DbMallEntities())
            {
                try
                {
                    if (entity.StartTime != null)
                    {
                        entity.StartTime = entity.StartTime.Value.AddDays(1);
                    }

                    #region 给每个会员分红

                    var list = db.Member_Info.Where(a => a.IsActive == "已激活" && a.Code != "admin" && a.IsNullActive != true && a.IsLockFen != true);
                    decimal? sum = 0M;
                    var nullMember = new Member_Info();
                    var comment1 = "分红奖";
                    var comment2 = "分红奖累计已达到投资额，后面将不再产生分红奖";
                    foreach (var item in list)
                    {
                        //var ReCount = db.Member_Info.Count(p => p.RecommendId == item.MemberId && p.IsActive == "已激活");
                        //decimal? FengD = 0;
                        //if (ReCount >= 2)
                        //{
                        //    FengD = item.ActiveAmount * 2;
                        //}
                        //else if (ReCount >= 1)
                        //{
                        //    FengD = item.ActiveAmount * 1.5m;
                        //}
                        //else if (ReCount < 1)
                        //{
                        //    FengD = item.ActiveAmount;
                        //}
                        decimal? FengD = 0;
                        if (item.MemberLevelId == 1)
                        {
                            FengD = item.ActiveAmount * 1;
                        }
                        else if (item.MemberLevelId == 2)
                        {
                            FengD = item.ActiveAmount * 1.5m;
                        }
                        else if (item.MemberLevelId == 3)
                        {
                            FengD = item.ActiveAmount * 2;
                        }
                        //判断分红金额累计 是否 会超过 投资额(这里的条件是以投资额作为封顶)
                        var realAmount = entity.PerAmount;
                        if (item.MemberLevelId == 1)
                        {
                            realAmount = entity.PerAmount;
                        }
                        else if (item.MemberLevelId == 2)
                        {
                            realAmount = entity.PerAmount2;
                        }
                        else if (item.MemberLevelId == 3)
                        {
                            realAmount = entity.PerAmount3;
                        }
                        string comment = comment1;
                        if (FengD <= realAmount + item.FHSum)
                        {
                            comment = comment2;
                            realAmount = FengD - item.FHSum;
                            if (realAmount < 0)
                            {
                                realAmount = 0;
                            }
                        }
                        sum += realAmount;  //求总的实发金额
                        item.FHSum = item.FHSum + realAmount;
                        //插入收益
                        json = DB.Jiang.InsertFin(db, item, nullMember, realAmount.Value, "分红", comment);
                    }
                    #endregion

                    #region 更新实际金额（总）
                    entity.RealAmount = sum;
                    db.Allocation_Info.Add(entity);
                    #endregion

                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    json.IsSuccess = false;
                    LogHelper.Error("分红失败：" + WebTools.getFinalException(e));
                }
            }
            return json;
        }
    }
}
