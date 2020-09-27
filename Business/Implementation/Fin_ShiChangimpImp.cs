using Common;
using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementation
{
    public class Fin_ShiChangimpImp : EFBase<DataBase.Fin_ShiChangimp>
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
        public List<Fin_ShiChangimp> getDataSource(string id, DateTime? start, DateTime? end, string key, out int total, int _start, int pageSize)
        {
            var query = DB.Fin_ShiChangimp.Where();
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
                query = query.Where(a => a.MemberCode.Contains(key) || a.NickName.Contains(key) || a.State.Contains(key));
            }

            total = query.Count();
            var data = query.OrderByDescending(p => p.CreateTime).Skip(_start).Take(pageSize).ToList();
            return data;
        }
        public List<Fin_ShiChangimp> getDataSourceMember(string id, DateTime? start, DateTime? end, string key, out int total, int _start, int pageSize)
        {
            var query = DB.Fin_ShiChangimp.Where();
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
                query = query.Where(a => a.MemberCode.Contains(key) || a.NickName.Contains(key) || a.State.Contains(key));
            }

            total = query.Count();
            var data = query.OrderByDescending(a => a.CreateTime).Skip(pageSize * (_start - 1)).Take(pageSize).ToList();
            return data;
        }
        #endregion

        #region 保存（添加与更新）
        public JsonHelp Save(Fin_ShiChangimp entity)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "保存失败" };

            using (var tran = DB.Fin_ShiChangimp.BeginTransaction)
            {
                try
                {
                    #region 检查
                    var model = DB.Member_Info.FindEntity(entity.MemberId);
                    if (model != null)
                    {
                        var LAmount = DB.Member_Info.Where(p => p.Position.StartsWith(model.Position + "1")).Sum(p => p.ActiveAmount) ?? 0;
                        var RAmount = DB.Member_Info.Where(p => p.Position.StartsWith(model.Position + "2")).Sum(p => p.ActiveAmount) ?? 0;
                        var min = Math.Min(LAmount, RAmount);

                        if (entity.TypeName == "10万车")
                        {
                            var jiang = DB.Fin_ShiChangimp.FindEntity(p => p.TypeName == "10万车" && p.MemberId == model.MemberId && p.State != "已驳回");
                            if (jiang != null)
                            {
                                json.Msg = "您申请过这个奖励不得重复申请!";
                                return json;
                            }
                            if (min < 500000)
                            {
                                json.Msg = "你未达到条件,申请失败!";
                                return json;
                            }
                            entity.JiangL = "10万车";
                        }
                        else if (entity.TypeName == "20万车")
                        {
                            var jiang1 = DB.Fin_ShiChangimp.FindEntity(p => p.TypeName == "20万车" && p.MemberId == model.MemberId && p.State != "已驳回");
                            if (jiang1 != null)
                            {
                                json.Msg = "您申请过这个奖励不得重复申请!";
                                return json;
                            }
                            if (min < 1000000)
                            {
                                json.Msg = "你未达到条件,申请失败!";
                                return json;
                            }
                            entity.JiangL = "20万车";
                        }
                        else if (entity.TypeName == "30万车")
                        {
                            var jiang2 = DB.Fin_ShiChangimp.FindEntity(p => p.TypeName == "30万车" && p.MemberId == model.MemberId && p.State != "已驳回");
                            if (jiang2 != null)
                            {
                                json.Msg = "您申请过这个奖励不得重复申请!";
                                return json;
                            }
                            if (min < 2000000)
                            {
                                json.Msg = "你未达到条件,申请失败!";
                                return json;
                            }
                            entity.JiangL = "30万车";
                        }
                        else if (entity.TypeName == "50万车")
                        {
                            var jiang3 = DB.Fin_ShiChangimp.FindEntity(p => p.TypeName == "50万车" && p.MemberId == model.MemberId && p.State != "已驳回");
                            if (jiang3 != null)
                            {
                                json.Msg = "您申请过这个奖励不得重复申请!";
                                return json;
                            }
                            if (min < 3000000)
                            {
                                json.Msg = "你未达到条件,申请失败!";
                                return json;
                            }
                            entity.JiangL = "50万车";
                        }
                        else if (entity.TypeName == "80万车")
                        {
                            var jiang4 = DB.Fin_ShiChangimp.FindEntity(p => p.TypeName == "80万车" && p.MemberId == model.MemberId && p.State != "已驳回");
                            if (jiang4 != null)
                            {
                                json.Msg = "您申请过这个奖励不得重复申请!";
                                return json;
                            }
                            if (min < 5000000)
                            {
                                json.Msg = "你未达到条件,申请失败!";
                                return json;
                            }
                            entity.JiangL = "80万车";
                        }
                        else if (entity.TypeName == "海景房一套")
                        {
                            var jiang5 = DB.Fin_ShiChangimp.FindEntity(p => p.TypeName == "海景房一套" && p.MemberId == model.MemberId && p.State != "已驳回");
                            if (jiang5 != null)
                            {
                                json.Msg = "您申请过这个奖励不得重复申请!";
                                return json;
                            }
                            if (min < 8000000)
                            {
                                json.Msg = "你未达到条件,申请失败!";
                                return json;
                            }
                            entity.JiangL = "海景房一套";
                        }
                        else if (entity.TypeName == "公司原始股东")
                        {
                            var jiang5 = DB.Fin_ShiChangimp.FindEntity(p => p.TypeName == "公司原始股东" && p.MemberId == model.MemberId && p.State != "已驳回");
                            if (jiang5 != null)
                            {
                                json.Msg = "您申请过这个奖励不得重复申请!";
                                return json;
                            }
                            if (min < 13000000)
                            {
                                json.Msg = "你未达到条件,申请失败!";
                                return json;
                            }
                            entity.JiangL = "公司原始股东";
                        }

                        if (entity.id == 0)
                        {
                            if (Insert(entity))
                            {
                                json.Status = "y";
                                json.Msg = "操作成功";
                                json.ReUrl = "/Member_Center/ServiceApply/Index";
                                //添加操作日志
                                DB.SysLogs.setMemberLog(Enums.EventType.Add, string.Format("提现申请已提交，操作人：[{0}]，操作成功", entity.NickName));
                            }
                        }
                        else
                        {
                            if (Update(entity))
                            {
                                json.Status = "y";
                                json.Msg = "操作成功";
                                json.ReUrl = "/Member_Center/ServiceApply/Index";
                                //添加操作日志
                                DB.SysLogs.setMemberLog(Enums.EventType.Edit, string.Format("修改申请已提交，操作人：[{0}]，操作成功", entity.NickName));
                            }
                        }
                    }
                    else
                    {
                        json.Msg = "会员信息不存在！";
                        return json;
                    }
                    tran.Complete();
                    #endregion
                }
                catch (Exception e)
                {
                    DB.Rollback();
                    LogHelper.Error("申请出错：" + e.Message);
                }
            }
            return json;
        }
        #endregion

        #region 审核 是否发放
        /// <summary>
        /// 是否发放
        /// </summary>
        /// <param name="id">Drawid</param>
        /// <param name="type">发放类型 1发放 2驳回</param>
        /// <returns></returns>
        public JsonHelp Issue(int id, int type, string userId, string userName)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "操作失败" };

            using (var tran = DB.Fin_ShiChangimp.BeginTransaction)
            {
                try
                {
                    var Draw = DB.Fin_ShiChangimp.FindEntity(p => p.id == id);
                    if (type == 1)
                    {
                        Draw.State = "已发放";
                        Draw.ConfirmTime = DateTime.Now;
                        Draw.ConfirmEmpId = userId;
                        Draw.ConfirmEmpName = userName;
                        if (Update(Draw))
                        {
                            json.Status = "y";
                            json.Msg = "发放成功";
                        }
                    }
                    else if (type == 2)
                    {

                        Draw.State = "已驳回";
                        Draw.ConfirmTime = DateTime.Now;
                        Draw.ConfirmEmpId = userId;
                        Draw.ConfirmEmpName = userName;
                        Update(Draw);
                        json.Status = "y";
                        json.Msg = "驳回成功";
                    }
                    tran.Complete();
                }
                catch (Exception e)
                {
                    DB.Rollback();
                    LogHelper.Error("发奖出错：" + e.Message);
                }
            }
            return json;
        }
        #endregion

    }
}
