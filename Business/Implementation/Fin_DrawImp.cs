using Common;
using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Wechat;

namespace Business.Implementation
{
    public class Fin_DrawImp : EFBase<DataBase.Fin_Draw>
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
        public List<Fin_Draw> getDataSource(string id, DateTime? start, DateTime? end, string key, out int total, int _start, int pageSize)
        {
            var query = DB.Fin_Draw.Where();
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
                query = query.Where(a => a.MemberCode.Contains(key) || a.NickName.Contains(key) || a.DrawState.Contains(key));
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

        #region 保存（添加与更新）
        public bool Save(string Pwd2, Fin_Draw entity)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "保存失败" };

            using (var tran = DB.Fin_Draw.BeginTransaction)
            {
                try
                {
                    int weeknow = Convert.ToInt32(DateTime.Today.DayOfWeek);
                    string tixian = DB.XmlConfig.XmlSite.DrawName;
                    if (tixian.IndexOf(weeknow.ToString()) == -1)
                    {
                        //json.Msg = "每周" + DB.XmlConfig.XmlSite.DrawName + "提现"; ;

                        throw new Exception("暂未到提现时间");
                    }

                    #region 检查支付密码是否正确,提款金额是否超限, 检验提现金额，最小提现金额与提现倍数等
                    var model = DB.Member_Info.FindEntity(entity.MemberId);
                    if (string.IsNullOrEmpty(model.BankCode) || string.IsNullOrEmpty(model.BankAccount) || string.IsNullOrEmpty(model.BankName))
                    {
                        throw new Exception("请完善银行信息");
                    }
                    if (model.IsLockDraw == true)
                    {
                        throw new Exception("此会员已锁定提现功能!");

                    }
                    if (model != null && model.Pwd2 == Pwd2)
                    {
                        if (entity.DrawAmount <= 0)
                        {
                            throw new Exception("提现金额要大于0!");

                        }
                        if (entity.DrawAmount > model.Commission)
                        {
                            throw new Exception("提现金额不能大于余额!");

                        }
                        var min = DB.XmlConfig.XmlSite.MinAmount;  //提现最小金额
                        var Multiple = DB.XmlConfig.XmlSite.Multiple; //提现金额是这个的整数倍
                        if (entity.DrawAmount < min)
                        {
                            throw new Exception("最小提现金额" + min + "！");


                        }
                        if (entity.DrawAmount % Multiple != 0)
                        {
                            throw new Exception("提现倍数为" + Multiple + "！");

                        }
                        //计算税金                        
                        var DrawPoundage = DB.XmlConfig.XmlSite.DrawPoundage;
                        entity.Poundage = entity.DrawAmount * DrawPoundage / 100M;
                        entity.Amount = entity.DrawAmount - entity.Poundage;
                        if (entity.DrawId == 0)
                        {
                            if (Insert(entity))
                            {
                                //更新会员表的收益,
                                model.Commission -= entity.DrawAmount;
                                DB.Fin_LiuShui.AddLS(model.MemberId, entity.DrawAmount.Value, "提现申请");
                                DB.Member_Info.Update(model);

                                //添加操作日志
                                DB.SysLogs.setMemberLog(Enums.EventType.Add, string.Format("提现申请已提交，操作人：[{0}]，金额：[{1}]，操作成功", entity.NickName, entity.Amount));
                            }
                        }
                        else
                        {
                            if (Update(entity))
                            {
                                json.Status = "y";
                                json.Msg = "操作成功";
                                json.ReUrl = "/Member_Finance/Draw/Index";
                                //添加操作日志
                                DB.SysLogs.setMemberLog(Enums.EventType.Edit, string.Format("修改提现申请已提交，操作人：[{0}]，金额：[{1}]，操作成功", entity.NickName, entity.Amount));
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("支付密码错误！");

                    }
                    tran.Complete();
                    return true;
                    #endregion
                }
                catch (Exception ex)
                {
                    DB.Rollback();
                    throw ex;
                }
            }

        }
        #endregion

        #region 审核 是否发放
        /// <summary>
        /// 是否发放
        /// </summary>
        /// <param name="id">Drawid</param>
        /// <param name="type">发放类型 1发放 2驳回</param>
        /// <returns></returns>
        public JsonHelp Issue(int id, int type, string userId, string userName, string summary)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "操作失败" };

            using (var tran = DB.Fin_Draw.BeginTransaction)
            {
                try
                {

                    var Draw = FindEntity(p => p.DrawId == id);
                    if (Draw.DrawState == "已发放" || Draw.DrawState == "已驳回")
                    {
                        json.Msg = "提现已审核，不能重复审核";
                        return json;
                    }

                    Member_Info member = DB.Member_Info.FindEntity(Draw.MemberId);
                    if (type == 1)
                    {
                        Draw.DrawState = "已发放";
                        Draw.ConfirmTime = DateTime.Now;
                        Draw.ConfirmEmpId = userId;
                        Draw.ConfirmEmpName = userName;
                        if (Update(Draw))
                        {
                            json.Status = "y";
                            json.Msg = "发放成功";
                            DB.SysLogs.setAdminLog(Enums.EventType.Edit, string.Format("提现发放成功，提现人：[{0}]，提现金额：[{1}]", Draw.MemberCode, Draw.DrawAmount));
                            tran.Complete();


                            //发送模板通知
                            if (member != null && string.IsNullOrEmpty(member.OpenID) == false)
                            {
                                MsgMessage _msg = new MsgMessage();
                                _msg.Send_DrawSuccess(Draw, member.OpenID, $"http://{HttpContext.Current.Request.Url.Host}/Member_Finance/Draw/Index");
                            }
                        }
                    }
                    else if (type == 2)
                    {
                        member.Commission = member.Commission + Draw.DrawAmount;
                        DB.Fin_LiuShui.AddLS(member.MemberId, Draw.DrawAmount.Value, "提现驳回");
                        if (DB.Member_Info.Update(member))
                        {
                            Draw.DrawState = "已驳回";
                            Draw.ConfirmTime = DateTime.Now;
                            Draw.ConfirmEmpId = userId;
                            Draw.ConfirmEmpName = userName;
                            Draw.Summary = summary;

                            Update(Draw);
                            json.Status = "y";
                            json.Msg = "驳回成功";
                            DB.SysLogs.setAdminLog(Enums.EventType.Edit, string.Format("提现已驳回，提现人：[{0}]，提现金额：[{1}]", Draw.MemberCode, Draw.DrawAmount));
                            tran.Complete();

                            //发送模板通知
                            if (member != null && string.IsNullOrEmpty(member.OpenID) == false)
                            {
                                MsgMessage _msg = new MsgMessage();
                                _msg.Send_DrawError(Draw, member.OpenID, $"http://{HttpContext.Current.Request.Url.Host}/Member_Finance/Draw/Index");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    DB.Rollback();
                    LogHelper.Error("发放资金出错：" + e.Message);
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

                using (var tran = DB.Fin_Draw.BeginTransaction)
                {
                    try
                    {
                        if (Any(a => list_id.Contains(a.DrawId) && (a.DrawState == "已通过" || a.DrawState == "已驳回")))
                        {
                            json.Msg = "只能删除申请中的提现记录";
                            return json;
                        }
                        var old = Where(a => list_id.Contains(a.DrawId)).ToList();
                        if (Delete(a => id.Contains(a.DrawId)) > 0)
                        {
                            var oldid = old.Select(a => a.MemberId).ToList();
                            var members = DB.Member_Info.Where(a => oldid.Contains(a.MemberId)).ToList().Distinct();
                            //更新会员收益
                            foreach (var item in old)
                            {
                                var m = members.FirstOrDefault(a => a.MemberId == item.MemberId);
                                if (m == null)
                                {
                                    DB.Rollback();
                                    json.Msg = "删除失败，请刷新页面重试";
                                    return json;
                                }
                                m.Commission = m.Commission + item.DrawAmount;
                                if (!DB.Member_Info.Update(m))
                                {
                                    DB.Rollback();
                                    return json;
                                }
                            }
                            json.Status = "y";
                            json.Msg = "删除数据成功";
                            //添加操作日志
                            DB.SysLogs.setAdminLog(Enums.EventType.Delete, "删除 名称为【" + idList + "】的提现信息");
                        }
                        tran.Complete();
                    }
                    catch (Exception ex)
                    {
                        //回滚事务
                        DB.Rollback();
                        throw ex;
                    }
                }
                return json;
            }
            catch (Exception e) { throw e.InnerException; }
        }
        #endregion
    }
}
