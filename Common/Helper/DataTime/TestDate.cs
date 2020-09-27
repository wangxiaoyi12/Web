using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// 模拟时间，测试系统用
    /// </summary>
    public class TestDate
    {
        #region 测试时间设计
        public static bool IsTest = false;
        public static DateTime CurTestTime = DateTime.Now;
        /// <summary>
        /// 获取当前时间
        /// </summary>
        /// <returns></returns>
        public static DateTime getNow()
        {
            var time = DateTime.Now;
            if (IsTest == true)
            {
                time = Convert.ToDateTime(CurTestTime.Date.ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
            }
            return time;
        }
        #endregion
    }
}
