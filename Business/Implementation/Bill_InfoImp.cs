using Common;
using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementation
{
    public class Bill_InfoImp :EFBase<DataBase.Bill_Info>
    {
        #region 查询 
        public List<Bill_Info> getDataSource(string id, DateTime? start, DateTime? end, string key, out int total, int _start, int pageSize)
        {
            var query = DB.Bill_Info.Where();
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
                query = query.Where(a => a.ProductName.Contains(key) || a.ExpressName.Contains(key) || a.ExpressCode.Contains(key) || a.State.Contains(key));
            }
            var data = query.OrderBy(p => p.State).ThenByDescending(a => a.CreateTime).Skip(_start).Take(pageSize).ToList();
            total = data.Count;
            if (data.Count >= pageSize)
            {
                total = query.Count();
            }
            return data;
        }
        #endregion

        #region 删除
        public JsonHelp Delete(string idList)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "删除数据失败" };
            try
            {
                //是否为空
                if (string.IsNullOrEmpty(idList)) { json.Msg = "未找到要删除的数据"; return json; }
                var id = idList.TrimEnd(',').Split(',').Select(a => Convert.ToInt32(a)).ToList();

                try
                {
                    using (var db = new DbMallEntities())
                    {
                        //删除订单的同时，奖下单时产生的电子币加回去
                        var list = db.Bill_Info.Where(a => id.Contains(a.BillId));
                        if (list.Any(a => a.State == "已发货"))
                        {
                            json.Msg = "选择订单中包含已发货订单,请重新选择;";
                            return json;
                        }
                        foreach (var item in list)
                        {
                            var member = db.Member_Info.Find(item.MemberId);
                            member.Coins += item.TotalPrice;
                            db.Bill_Info.Remove(item);
                        }
                        //删除这些对象
                        var r = db.SaveChanges();
                        if (r > 0)
                        {
                            json.Status = "y";
                            json.Msg = "删除数据成功";
                            //添加操作日志
                            DB.SysLogs.setAdminLog(Enums.EventType.Delete, "删除 ID为【" + idList + "】的订单信息");
                        }
                    }
                }
                catch
                {
                    throw;
                }
                return json;
            }
            catch (Exception e) { throw e.InnerException; }
        }
        #endregion

        #region 保存
        public JsonHelp Save(Bill_Info entity)
        {
            JsonHelp json = new JsonHelp();

            using (var tran = DB.Bill_Info.BeginTransaction)
            {
                try
                {
                    if (entity.BillId > 0)
                    {
                        entity.State = "已发货";

                        if (Update(entity))
                        {
                            json.Status = "y";
                            json.Msg = "保存成功";
                            //添加操作日志
                            DB.SysLogs.setAdminLog(Enums.EventType.Edit, "更新编号为[" + entity.BillId + "]的订单");
                        }
                    }
                    else
                    {
                        //	商品下单的时候，扣电子币，如不够，则提示
                        var member = DB.Member_Info.FindEntity(entity.MemberId);
                        if (member == null)
                        {
                            json.Msg = "登录超时，请重新登录";
                            return json;
                        }
                        if (member.Coins < entity.TotalPrice)
                        {
                            json.Msg = "下单失败：电子币不足！";
                            return json;
                        }
                        var m = Insert(entity);
                        if (m)
                        {
                            member.Coins = member.Coins - entity.TotalPrice;
                            var r = DB.Member_Info.Update(member);
                            if (r == true)
                            {
                                json.Status = "y";
                                json.Msg = "下单成功";
                                //添加操作日志
                                DB.SysLogs.setMemberLog(Enums.EventType.Add, string.Format("商城下单，下单人：{0}，订单编号：{1}", entity.MemberCode, m));
                            }
                            else
                            {
                                json.Msg = "扣除会员电子币失败";
                                return json;
                            }
                        }
                    }
                    tran.Complete();
                }
                catch (Exception e)
                {
                    DB.Rollback();
                    LogHelper.Error("当前会员编号：【" + entity.MemberCode + "】下单失败，出现错误：" + e.Message);
                }
            }

            return json;
        }
        #endregion 
    }
}
