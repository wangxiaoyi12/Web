using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    /// <summary>
    /// 周结算，月结算扩展
    /// </summary>
    public partial class SettleRecord
    {

        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName
        {
            get
            {
                return Enums.ToObject<Enums.SettleType>(this.Type).ToString();
            }
        }
    }
}
