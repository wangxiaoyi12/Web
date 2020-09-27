using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interface
{
    /// <summary>
    /// 管理员用户表接口
    /// </summary>
    public interface ISys_Employee : DataBase.IRepository<DataBase.Sys_Employee>
    {
    }
}
