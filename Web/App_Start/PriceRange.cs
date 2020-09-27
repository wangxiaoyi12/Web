using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System.Web
{
    /// <summary>
    /// 产品 价格区间处理
    /// </summary>
    public class PriceRange
    {
        ///// <summary>
        ///// 获取价格区间定义
        ///// </summary>
        ///// <returns></returns>
        //public static Dictionary<int, int> GetList()
        //{
        //    Dictionary<int, int> dic = new Dictionary<int, int>();
        //    dic.Add(0, 50);
        //    dic.Add(50, 150);
        //    dic.Add(150, 500);
        //    dic.Add(500, 1000);
        //    dic.Add(1000, int.MaxValue);
        //    return dic;
        //}

        public static List<Range> GetList()
        {
            List<Range> list = new List<Web.PriceRange.Range>() {
                new Range() { ID=1,Min=0,Max=50},
                new Range() { ID=2,Min=50,Max=150},
                new Range() { ID=3,Min=150,Max=500},
                new Range() { ID=4,Min=500,Max=1000},
                new Range() { ID=5,Min=1000,Max=int.MaxValue},
            };
            return list;
        }
        /// <summary>
        /// 根据ID获取队形
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static Range GetRange(int ID)
        {
            return GetList().Where(q => q.ID == ID).First();
        }

        public class Range
        {
            public int ID { get; set; }
            public int Max { get; set; }
            public int Min { get; set; }
        }

    }
}
