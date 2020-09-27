using Common;
using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementation
{
    public class Fin_RemitImp :EFBase<DataBase.Fin_Remit>
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
        public List<Fin_Remit> getDataSource(string id, DateTime? start, DateTime? end, string key, out int total, int _start, int pageSize)
        {
            var query = Where();
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
                query = query.Where(a => a.MemberCode.Contains(key) || a.NickName.Contains(key));
            }

            var data = query.OrderByDescending(p => p.RemitTime).Skip(_start).Take(pageSize).ToList();
            total = data.Count;
            if (data.Count >= pageSize)
            {
                total = query.Count();
            }
            return data;
        }
        public List<Fin_Remit> getDataSources(string id, DateTime? start, DateTime? end, string key, out int total, int _start, int pageSize)
        {
            var query = Where(p => p.BankAddress != null);
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
                query = query.Where(a => a.MemberCode.Contains(key) || a.NickName.Contains(key));
            }

            total = query.Count();
            var data = query.OrderByDescending(p => p.RemitTime).Skip(_start).Take(pageSize).ToList();
            return data;
        }
        #endregion

        #region 保存
        public JsonHelp Save(Fin_Remit entity)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "保存失败" };
            if (entity.Amount <= 0)
            {
                json.Msg = "充值金额要大于0！";
                return json;
            }
            if (string.IsNullOrEmpty(entity.Image ))
            {
                json.Msg = "请上传打款图片！";
                return json;
            }

            if (entity.RemitId == 0)
            {
                if (Insert(entity))
                {
                    json.Status = "y";
                    json.Msg = "操作成功";
                    json.ReUrl = "/Member_Finance/Remit/Index";
                    //添加操作日志
                    DB.SysLogs.setMemberLog(Enums.EventType.Add, string.Format("汇款申请提交，汇款人：[{0}]，汇款金额：[{1}]，操作成功", entity.BankAccount, entity.Amount));
                }
            }
            else
            {
                if (Update(entity))
                {
                    json.Status = "y";
                    json.Msg = "操作成功";
                    json.ReUrl = "/Member_Finance/Remit/Index";
                    //添加操作日志
                    DB.SysLogs.setMemberLog(Enums.EventType.Edit, string.Format("修改汇款申请提交，汇款人：[{0}]，汇款金额：[{1}]，操作成功", entity.BankAccount, entity.Amount));
                }
            }
            return json;
        }
        #endregion

        #region 审核 ，type=1通过 type=2驳回
        public JsonHelp Isok(int id, int type, string userid, string username)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "操作失败" };

            using (var tran = DB.Fin_Remit.BeginTransaction)
            {
                try
                {
                    var remit = FindEntity(id);
                    if (remit.RemitState == "已通过" || remit.RemitState == "已驳回")
                    {
                        json.Msg = "不能重复审核！";
                        return json;
                    }
                    if (type == 1)
                    {
                        remit.RemitState = "已通过";
                        remit.ConfirmTime = DateTime.Now;
                        remit.ConfirmEmpId = userid;
                        remit.ConfirmEmpName = username;
                        if (Update(remit))
                        {
                            var member = DB.Member_Info.FindEntity(p => p.MemberId == remit.MemberId);
                            member.Commission = member.Commission + remit.Amount;
                            DB.Fin_LiuShui.AddLS(member.MemberId, remit.Amount.Value, "汇款通过");
                            DB.Member_Info.Update(member);
                            //流水账单
                            Fin_LiuShui _liushui = new Fin_LiuShui();
                            _liushui.MemberId = member.MemberId;
                            _liushui.Code = member.Code;
                            _liushui.NickName = member.NickName;
                            _liushui.Type = "流水账单";
                            _liushui.Comment = "汇款报单积分(+)";
                            _liushui.Amount = remit.Amount;
                            _liushui.CreateTime = DateTime.Now;
                            DB.Fin_LiuShui.Insert(_liushui);
                            json.Status = "y";
                            json.Msg = "操作成功";
                            DB.SysLogs.setAdminLog(Enums.EventType.Edit, string.Format("汇款申请已通过，汇款人：[{0}],金额：[{1}]", remit.MemberCode, remit.Amount));
                        }
                    }
                    else if (type == 2)
                    {
                        remit.RemitState = "已驳回";
                        remit.ConfirmTime = DateTime.Now;
                        remit.ConfirmEmpId = userid;
                        remit.ConfirmEmpName = username;
                        if (Update(remit))
                        {
                            json.Status = "y";
                            json.Msg = "操作成功";
                            DB.SysLogs.setAdminLog(Enums.EventType.Edit, string.Format("汇款申请已驳回，汇款人：[{0}],金额：[{1}]", remit.MemberCode, remit.Amount));
                        }
                    }
                    tran.Complete();
                }
                catch (Exception e)
                {
                    DB.Rollback();
                    LogHelper.Error("是否通过汇款申请，出错：" + e.Message);
                }
            }
            return json;
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
                var list_id = id.Select(a => Convert.ToInt32(a)).ToList();

                try
                {
                    if (Any(a => list_id.Contains(a.RemitId) && (a.RemitState == "已通过" || a.RemitState == "已驳回")))
                    {
                        json.Msg = "只能删除申请中的汇款记录";
                        return json;
                    }
                    if (Delete(a => id.Contains(a.RemitId)) > 0)
                    {
                        json.Status = "y";
                        json.Msg = "删除数据成功";
                        //添加操作日志
                        DB.SysLogs.setAdminLog(Enums.EventType.Delete, "删除 名称为【" + idList + "】的汇款信息");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return json;
            }
            catch (Exception e) { throw e.InnerException; }
        }
        #endregion
    }
}
