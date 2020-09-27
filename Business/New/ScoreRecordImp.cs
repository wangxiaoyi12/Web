using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataBase;
using EntityFramework.Extensions;

namespace Business
{
    /// <summary>
    /// 周结算，月计算处理
    /// </summary>
    public class ScoreRecordImp : EFBase<DataBase.ScoreRecord>
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
        public List<ScoreRecord> getDataSource(string id,DateTime? start, DateTime? end, string key,out int total, int _start, int pageSize)
        {
            var query = Where();
            if (string.IsNullOrEmpty(id) == false)
                query = query.Where(q=>q.MemberID==id);
            if (string.IsNullOrEmpty(key))
                query = query.Where(q=>q.MemberCode.Contains(key)||q.MemberName.Contains(key));
            if (start != null)
            {
                query = query.Where(a => a.CreateTime >= start);
            }
            if (end != null)
            {
                end = end.Value.AddDays(1);
                query = query.Where(a => a.CreateTime < end);
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
    }
}
