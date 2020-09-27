using Common;
using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementation
{
    public class Fin_ConvertImp : EFBase<DataBase.Fin_Convert>
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
        public List<Fin_Convert> getDataSource(string id, DateTime? start, DateTime? end, string key, out int total, int _start, int pageSize)
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
                query = query.Where(a => a.MemberCode.Contains(key) || a.NickName.Contains(key) || a.ConvertType.Contains(key));
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
        /// 保存（添加与更新）
        /// </summary>
        /// <param name="Pwd2">支付密码</param>
        /// <param name="entity">当前实体对象</param>
        /// <returns></returns>
        public bool Save(string Pwd2, Fin_Convert entity)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "保存失败" };

            Xml_Site config = DB.XmlConfig.XmlSite;

            #region 检查支付密码是否正确,转换金额是否超限,  
          
            using (var tran = DB.Fin_Convert.BeginTransaction)
            {
                try
                {
                    var model = DB.Member_Info.FindEntity(entity.MemberId);
                    if (model != null && model.Pwd2 == Pwd2)
                    {
                     
                    }else
                    {
                        throw new Exception("支付密码错误");
                    }
                    if (entity.Amount <= 0)
                    {
                        throw new Exception("转换金额要大于0");

                    }

                    if (entity.ConvertType == "收益转推广奖")
                    {
                        if (entity.Amount > model.Commission)
                        {
                            throw new Exception("可用收益额度不足!");

                        }
                     
                    }
               

                    if (entity.ConvertId == 0)
                    {
                        if (Insert(entity))
                        {  //流水账单
                            Fin_LiuShui _liushui = new Fin_LiuShui();
                            //更新会员表的收益,电子币
                            if (entity.ConvertType == "收益转推广奖")
                            {
                                model.Commission -= entity.Amount;
                                model.ShopCoins += entity.Amount;

                            }
                       

                            DB.Member_Info.Update(model);
                            json.Status = "y";
                            json.Msg = "操作成功";
                            //添加操作日志
                            DB.SysLogs.setMemberLog(Enums.EventType.Add, string.Format("收益转换，操作人：[{0}]，金额：[{1}]，操作成功", entity.NickName, entity.Amount));
                        }
                    }
                    tran.Complete();
                    return true;
                }
                catch (Exception ex)
                {
                    DB.Rollback();
                    throw ex;
                }
            }
         

            #endregion

        }
        #endregion
    }
}
