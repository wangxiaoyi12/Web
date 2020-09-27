using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// 日期，时间 帮助类
    /// </summary>
    public class DateTimeHelper
    {
        #region 计算周一与周日         
        /// <summary> 
        /// 计算某日起始日期（礼拜一的日期） 
        /// </summary> 
        /// <param name="someDate">该周中任意一天</param> 
        /// <returns>返回礼拜一日期，后面的具体时、分、秒和传入值相等</returns> 
        public static DateTime GetMondayDate(DateTime someDate)
        {
            int i = someDate.DayOfWeek - DayOfWeek.Monday;
            if (i == -1) i = 6;// i值 > = 0 ，因为枚举原因，Sunday排在最前，此时Sunday-Monday=-1，必须+7=6。 
            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            return someDate.Subtract(ts);
        }
        /// <summary> 
        /// 计算某日结束日期（礼拜日的日期） 
        /// </summary> 
        /// <param name="someDate">该周中任意一天</param> 
        /// <returns>返回礼拜日日期，后面的具体时、分、秒和传入值相等</returns> 
        public static DateTime GetSundayDate(DateTime someDate)
        {
            int i = someDate.DayOfWeek - DayOfWeek.Sunday;
            if (i != 0) i = 7 - i;// 因为枚举原因，Sunday排在最前，相减间隔要被7减。 
            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            return someDate.Add(ts);
        }
        #endregion
        #region 注释
        DateTime dt = TestDate.getNow();  //当前时间
        /// <summary>
        /// 本月月初
        /// </summary>
        public DateTime MonthStart
        {
            get
            {
                return dt.AddDays(1 - dt.Day);  
            }
        }
        /// <summary>
        /// 本月月末
        /// </summary>
        public DateTime MonthEnd
        {
            get
            {
                return MonthStart.AddMonths(1).AddDays(-1);    
            }
        }
        /// <summary>
        /// 本季度初
        /// </summary>
        public DateTime QuarterStart
        {
            get
            {
                var dt = DateTime.Now;
                return dt.AddMonths(0 - (dt.Month - 1) % 3).AddDays(1 - dt.Day);
            }
        }
        /// <summary>
        /// 本季度末
        /// </summary>
        public DateTime QuarterEnd
        {
            get
            {
                return QuarterStart.AddMonths(3).AddDays(-1);
            }
        }
        /// <summary>
        /// 本年年初
        /// </summary>
        public DateTime YearStart
        {
            get
            {
                return new DateTime(dt.Year, 1, 1);
            }
        }
        /// <summary>
        /// 本年年末
        /// </summary>
        public DateTime YearEnd
        {
            get
            {
                return new DateTime(dt.Year, 12, 31);
            }
        }
        #endregion
    }
}
