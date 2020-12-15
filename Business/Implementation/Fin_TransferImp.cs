using Common;
using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementation
{
    public class Fin_TransferImp : EFBase<DataBase.Fin_Transfer>
    {
        #region 查询
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="id">当前用户id</param>
        /// <param name="start">开始日期</param>
        /// <param name="end">结束日期</param>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        public List<Fin_Transfer> getDataSource(string id, DateTime? start, DateTime? end, string key, out int total, int _start, int pageSize)
        {
            var query = DB.Fin_Transfer.Where();
            if (!string.IsNullOrEmpty(id))
            {
                query = query.Where(a => a.FromMemberId == id || a.ToMemberId == id);
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
                query = query.Where(a => a.FromMemberCode.Contains(key) || a.FromNickName.Contains(key) || a.ToMemberCode.Contains(key) || a.ToNickName.Contains(key));
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

        #region 保存
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="pwd2">支付密码</param>
        /// <param name="commission">可用收益余额</param>
        /// <param name="coins">可用余额（电子币）</param>
        /// <param name="entity">当前实体对象</param>
        public JsonHelp Save(string pwd2, Fin_Transfer entity)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "保存失败" };

            if (entity.Amount <= 0)
            {
                json.Msg = "转账金额要大于0！";
                return json;
            }

            using (var tran = DB.Fin_Transfer.BeginTransaction)
            {
                try
                {
                    var members = DB.Member_Info.Where(a => a.MemberId == entity.FromMemberId || a.Code == entity.ToMemberCode).ToList();
                    var fm = members.FirstOrDefault(a => a.MemberId == entity.FromMemberId);
                    var tm = members.FirstOrDefault(a => a.Code == entity.ToMemberCode);
                    if (tm == null)
                    {
                        json.Msg = "转给会员编号不正确！";
                        return json;
                    }
                    if (fm.Pwd2 != pwd2)
                    {
                        json.Msg = "支付密码不正确！";
                        return json;
                    }
                    if (fm.MemberId == tm.MemberId)
                    {
                        json.Msg = "不能转给自己！";
                        return json;
                    }
                    if (fm.IsSub.Value )
                    {
                        json.Msg = "转出方开关关闭，操作失败！";
                        return json;
                    }
                    if (tm.IsSub.Value)
                    {
                        json.Msg = "转入方开关关闭，操作失败！";
                        return json;
                    }
                    //if (entity.TransferType == "收益币互转")
                    //{
                    //    if (fm.Commission < entity.Amount)
                    //    {
                    //        json.Msg = "收益不足，不能转账！";
                    //        return json;
                    //    }
                    //}
                    if (entity.TransferType == "余额互转")
                    {
                        if (fm.Commission < entity.Amount)
                        {
                            json.Msg = "余额不足，不能转账！";
                            return json;
                        }
                    }
                    #region 只能上下级关系转账
                    //var canTransfer = false;
                    //// 1.安置关系
                    //if (tm.Position.StartsWith(fm.Position) || fm.Position.StartsWith(tm.Position))
                    //{
                    //    canTransfer = true;
                    //}
                    //// 2.推荐关系
                    //if (tm.RPosition.StartsWith(fm.RPosition) || fm.RPosition.StartsWith(tm.RPosition))
                    //{
                    //    canTransfer = true;
                    //}
                    //if (canTransfer == false)
                    //{
                    //    json.Msg = "只有上下级关系才可以转账！";
                    //    return json;
                    //}
                    #endregion
                    //var min = DB.XmlConfig.XmlSite.MinAmountHuZ;  //提现最小金额
                    //var Multiple = DB.XmlConfig.XmlSite.MultipleHuZ; //提现金额是这个的整数倍
                    //if (entity.Amount < min)
                    //{
                    //    json.Msg = "最小互转金额" + min + "！";
                    //    return json;
                    //}
                    //if (entity.Amount % Multiple != 0)
                    //{
                    //    json.Msg = "互转倍数为" + Multiple + "！";
                    //    return json;
                    //}
                    if (entity.TransferId == 0)
                    {
                        entity.ToMemberId = tm.MemberId;
                        entity.ToNickName = tm.NickName;
                        entity.FromMemberCode = fm.Code;
                        entity.FromNickName = fm.NickName;
                        entity.CreateTime = DateTime.Now;
                        if (Insert(entity))
                        {
                            if (entity.TransferType == "余额互转")
                            {
                                fm.Commission = fm.Commission - entity.Amount;
                                tm.Commission = tm.Commission + entity.Amount;
                                DB.Member_Info.Update(fm, tm);
                                //流水账单
                                Fin_LiuShui _liushui = new Fin_LiuShui();
                                _liushui.MemberId = fm.MemberId;
                                _liushui.Code = fm.Code;
                                _liushui.NickName = fm.NickName;
                                _liushui.Type = "余额";
                                _liushui.Comment = "余额互转";
                                _liushui.Amount = -entity.Amount;
                                _liushui.CreateTime = DateTime.Now;
                                DB.Fin_LiuShui.Insert(_liushui);
                                _liushui.MemberId = tm.MemberId;
                                _liushui.Code = tm.Code;
                                _liushui.NickName = tm.NickName;
                                _liushui.Type = "余额";
                                _liushui.Comment = "余额互转";
                                _liushui.Amount = entity.Amount;
                                _liushui.CreateTime = DateTime.Now;
                                DB.Fin_LiuShui.Insert(_liushui);
                            }
                            //if (entity.TransferType == "收益币互转")
                            //{
                            //    fm.Commission = fm.Commission - entity.Amount;
                            //    tm.Commission = tm.Commission + entity.Amount;
                            //    DB.Member_Info.Update(fm, tm);
                            //}
                            json.Status = "y";
                            json.Msg = "操作成功";
                            //添加操作日志
                            DB.SysLogs.setMemberLog("Save", string.Format("会员转账，转出会员编号：[{0}]，转入会员编号：[{2}]，金额：[{1}]，转账类型：[{3}]操作成功", entity.FromMemberCode, entity.Amount, entity.ToMemberCode, entity.TransferType));
                        }
                    }
                    tran.Complete();
                }
                catch (Exception e)
                {
                    DB.Rollback();
                    LogHelper.Error("会员转账出错：" + e.Message);
                }
            }
            return json;
        }
        #endregion
    }
}
