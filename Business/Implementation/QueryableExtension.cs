using DataBase;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementation
{/// <summary>
 /// IQueryable扩展类
 /// </summary>
    public static class QueryableExtension
    {
        /// <summary>
        /// 根据距离排序升序
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <returns></returns>
        public static IQueryable<TEntity> OrderByDistance<TEntity>(this IQueryable<TEntity> queryable, double lng, double lat) where TEntity : class, IHasLngAndLat
        {
            var rtn = from q in queryable
                      let radLat1 = lat * Math.PI / 180.0
                      let radLat2 = q.WeiDu * Math.PI / 180.0
                      let a = radLat1 - radLat2
                      let b = lng * Math.PI / 180.0 - q.JingDu * Math.PI / 180.0
                      let s = 2 * SqlFunctions.Asin(SqlFunctions.SquareRoot(Math.Pow((double)SqlFunctions.Sin(a / 2), 2) +
               SqlFunctions.Cos(radLat1) * SqlFunctions.Cos(radLat2) * Math.Pow((double)SqlFunctions.Sin(b / 2), 2))) * 6378.137
                      let d = Math.Round((double)s * 10000) / 10000
                      orderby d
                      select q;

            return rtn;
        }
        /// <summary>
        /// 根据距离排序降序
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <returns></returns>
        public static IQueryable<TEntity> OrderByDistanceDesc<TEntity>(this IQueryable<TEntity> queryable, double lng, double lat) where TEntity : class, IHasLngAndLat
        {
            var rtn = from q in queryable
                      let radLat1 = lat * Math.PI / 180.0
                      let radLat2 = q.WeiDu * Math.PI / 180.0
                      let a = radLat1 - radLat2
                      let b = lng * Math.PI / 180.0 - q.JingDu * Math.PI / 180.0
                      let s = 2 * SqlFunctions.Asin(SqlFunctions.SquareRoot(Math.Pow((double)SqlFunctions.Sin(a / 2), 2) +
               SqlFunctions.Cos(radLat1) * SqlFunctions.Cos(radLat2) * Math.Pow((double)SqlFunctions.Sin(b / 2), 2))) * 6378.137
                      let d = Math.Round((double)s * 10000) / 10000
                      orderby d descending
                      select q;

            return rtn;
        }
    }

    /// <summary>
    /// 带距离的数据
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DataWithDistance<TEntity>
    {
        ///// <summary>
        ///// 距离（km）
        ///// </summary>
        //public double Distance { get; set; }
        /// <summary>
        /// 实体数据
        /// </summary>
        public TEntity Entity { get; set; }
    }
}
