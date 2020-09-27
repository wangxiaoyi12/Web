using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// 返回带有集合的Base
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseList<T> : Base
    {
        /// <summary>
        /// 数据集合
        /// </summary>
        public List<T> List { get; set; }
    }
}
