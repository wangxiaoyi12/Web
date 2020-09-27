using Common;
using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementation
{
    public class LogsImp :EFBase<DataBase.Logs>
    {
        public bool setSysLogsData1(string id, string username, string Event, string Description)
        {
            Logs logs = new Logs();
            logs.EmpId = id;
            logs.EmpName = username;
            logs.Event = Event;
            logs.Description = Description;
            logs.CreateTime = DateTime.Now;
            logs.IP = "";
            return Insert(logs);
        }
        #region 查询 系统日志       
        public List<Logs> getList(string key, out int total, int _start, int pageSize)
        {
            var query = DB.SysLogs.Where(a => a.EmpName.Contains(key) || a.Description.Contains(key)).OrderByDescending(p => p.CreateTime);

            var list = query.Skip(_start).Take(pageSize).ToList();
            total = list.Count;
            if (total >= pageSize)
            {
                total = query.Count();
            }
            return list;
        }
        #endregion
        #region 查询 系统日志       
        public List<Logs> getList(string id, DateTime? start, DateTime? end, string key, out int total, int _start, int pageSize)
        {
            var query = DB.SysLogs.Where(a=>a.Event=="Give");
            if (!string.IsNullOrEmpty(id))
            {
                query = query.Where(a => a.EmpId == id);
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
            query = query.Where(a => a.EmpName.Contains(key) || a.Description.Contains(key)).OrderByDescending(p => p.CreateTime);

            var list = query.Skip(_start).Take(pageSize).ToList();
            total = list.Count;
            if (total >= pageSize)
            {
                total = query.Count();
            }
            return list;
        }
        #endregion

        #region 后台--创建系统操纵日志
        public bool setAdminLog(string Event, string Description)
        {
            var c = CookieHelper.GetCookieValue(Enums.LoginType.admin.ToString());
            var id = string.Empty;
            var username = string.Empty;
            if (c != null)
            {
                var user = Common.JsonConverter.ConvertJson(Common.CryptHelper.DESCrypt.Decrypt(c));
                id = user?.id;
                username = user?.username;
            }
            return setSysLogsData(id, username, Event, Description);
        }
        public bool setAdminLog(Enums.EventType Event, string Description)
        {
            return setAdminLog(Event.ToString(), Description);
        }
        #endregion

        #region 前台--创建系统操纵日志
        public bool setMemberLog(string Event, string Description)
        {
            var c = CookieHelper.GetCookieValue(Enums.LoginType.member.ToString());
            var id = string.Empty;
            var username = string.Empty;
            if (c != null)
            {
                var user = Common.JsonConverter.ConvertJson(Common.CryptHelper.DESCrypt.Decrypt(c));
                id = user?.id;
                username = user?.username;
            }
            return setSysLogsData(id, username, Event, Description);
        }
        public bool setMemberLog(Enums.EventType Event, string Description)
        {
            return setMemberLog(Event.ToString(), Description);
        }
        #endregion

        #region 创建系统操纵日志
        public bool setSysLogsData(string id, string username, string Event, string Description)
        {
            Logs logs = new Logs();
            logs.EmpId = id;
            logs.EmpName = username;
            logs.Event = Event;
            logs.Description = Description;
            logs.CreateTime = DateTime.Now;
            logs.IP = Common.WebTools.GetHostAddress();
            return Insert(logs);
        }

        public bool setNoLoginLog(string Event, string Description)
        {
            return setSysLogsData("", "", Event, Description);
        }
        #endregion
    }
}
