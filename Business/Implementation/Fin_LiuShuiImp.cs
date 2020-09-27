using Common;
using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementation
{
    public class Fin_LiuShuiImp : EFBase<DataBase.Fin_LiuShui>
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
        public List<Fin_LiuShui> getDataSource(string id, DateTime? start, DateTime? end, string key, out int total, int _start, int pageSize)
        {
            var query = DB.Fin_LiuShui.Where(p=>p.Type== "货币流水");
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
                query = query.Where(a => a.Code.Contains(key) || a.NickName.Contains(key));
            }

            total = query.Count();
            var data = query.OrderByDescending(p => p.CreateTime).Skip(_start).Take(pageSize).ToList();
            return data;
        }
        public List<Fin_LiuShui> getDataSources(string id, DateTime? start, DateTime? end, string key, out int total, int _start, int pageSize)
        {
            var query = DB.Fin_LiuShui.Where(p => p.Type == "货币流水");
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
                query = query.Where(a => a.Code.Contains(key) || a.NickName.Contains(key));
            }

            total = query.Count();
            var data = query.OrderByDescending(p => p.CreateTime).Skip(_start).Take(pageSize).ToList();
            return data;
        }
        /// <summary>
        /// 前台
        /// </summary>
        /// <param name="id"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="key"></param>
        /// <param name="total"></param>
        /// <param name="_start"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<Fin_LiuShui> getDataSourceQian(string id, DateTime? start, DateTime? end, string key, out int total, int _start, int pageSize)
        {
            var query = DB.Fin_LiuShui.Where();
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
                query = query.Where(a => a.Code.Contains(key) || a.NickName.Contains(key));
            }

            total = query.Count();
            var data = query.OrderByDescending(p => p.CreateTime).Skip(_start).Take(pageSize).ToList();
            return data;
        }
        public List<Fin_LiuShui> getDataSourceMember(string id, DateTime? start, DateTime? end, string key, out int total, int _start, int pageSize)
        {
            var query = DB.Fin_LiuShui.Where();
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
                query = query.Where(a => a.Code.Contains(key) || a.NickName.Contains(key));
            }

            total = query.Count();
            var data = query.OrderByDescending(a => a.CreateTime).Skip(pageSize * (_start - 1)).Take(pageSize).ToList();
            return data;
        }
        #endregion
        public void AddLS(string memberid,decimal amount,string comment)
        {
            var m = DB.Member_Info.FindEntity(memberid);
            Fin_LiuShui _liushui = new Fin_LiuShui();
            _liushui.MemberId = m.MemberId;
            _liushui.Code = m.Code;
            _liushui.NickName = m.NickName;
            _liushui.Type = "货币流水";
            _liushui.Comment = comment;
            _liushui.Amount = amount;
            _liushui.CreateTime = DateTime.Now;
            DB.Fin_LiuShui.Insert(_liushui);
        }
    }
}
