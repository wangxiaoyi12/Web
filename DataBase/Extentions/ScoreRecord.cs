using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    /// <summary>
    /// 积分记录扩展
    /// </summary>
    public partial class ScoreRecord
    {
        /// <summary>
        /// 状态名称
        /// </summary>
        public string StateName
        {
            get
            {
                return Enums.ToObject<Enums.ScoreState>(this.State).ToString();
            }
        }
    }
}
