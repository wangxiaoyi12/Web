using Common;
using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementation
{
    public class Sys_MsgImp :EFBase<DataBase.Sys_Msg>
    {
        #region 查询
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="id">会员id</param>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        public List<Sys_Msg> getDataSource(string id, string key, out int total, int _start, int pageSize, int? state = null, bool? isonlyRecenve = false)
        {
            var query = Where();
            if (isonlyRecenve == true)
            {
                query = query.Where(a => a.ReceiverId == id);
            }
            else
            {
                query = query.Where(a => a.ReceiverId == id || a.SenderId == id);
            }

            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(a => a.Title.Contains(key) || a.SenderName.Contains(key) || a.ReceiverName.Contains(key) || a.Comment.Contains(key));
            }
            if (state != null)
            {
                query = query.Where(a => a.State == state);
            }
            var data = query.OrderByDescending(p => p.CreateTime).Skip(_start).Take(pageSize).ToList();
            total = query.Count(); 
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
                    //开启事务，可以不使用事务,也可以使用多个事务

                    if (Delete(a => id.Contains(a.MsgId)) > 0)
                    {
                        json.Status = "y";
                        json.Msg = "删除数据成功";
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                //添加操作日志
                DB.SysLogs.setMemberLog("Delete", "删除ID为[" + idList + "]的留言消息 ");

                return json;
            }
            catch (Exception e) { throw e.InnerException; }
        }
        #endregion

        #region 保存
        public JsonHelp Save(Sys_Msg entity)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "保存失败" };

            var members = DB.Member_Info.Where(a => a.Code == entity.SenderCode || a.Code == entity.ReceiverCode).Select(a => new { a.NickName, a.MemberId, a.Code }).ToList();
            var sender = members.FirstOrDefault(a => a.Code == entity.SenderCode);
            var receiver = members.FirstOrDefault(a => a.Code == entity.ReceiverCode);
            if (sender == null)
            {
                json.Msg = "发送人为空";
                return json;
            }
            if (receiver == null)
            {
                json.Msg = "接收人为空";
                return json;
            }
            entity.SenderId = sender.MemberId;
            entity.SenderName = sender.NickName;
            entity.ReceiverId = receiver.MemberId;
            entity.ReadTime = DateTime.Now;
            entity.ReceiverName = receiver.NickName;
            if (entity.MsgId == 0)
            {
                entity.State = 0;
                if (Insert(entity))
                {
                    json.Status = "y";
                    json.Msg = "操作成功";
                    //添加操作日志
                    DB.SysLogs.setMemberLog("Save", string.Format("发消息成功，信息id：[{0}]，操作成功", entity.MsgId));
                }
            }
            else
            {
                var model = DB.Sys_Msg.FindEntity(entity.MsgId);
                WebTools.CopyToObject(entity, model);
                if (Update(entity))
                {
                    json.Status = "y";
                    json.Msg = "操作成功";
                    //添加操作日志
                    DB.SysLogs.setMemberLog("Save", string.Format("更新消息成功，信息id：[{0}]，操作成功", entity.MsgId));
                }
            }
            return json;
        }
        #endregion 
    }
}
