using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wechat
{
    /// <summary>
    /// 接口管理，基础类
    /// </summary>
    public class BaseManage
    {
        /// <summary>
        /// 当前配置信息
        /// </summary>
        public ConfigInfo config = null;

        public BaseManage()
        {
            config = ConfigInfo.GetInfo();
        }
    }
}
