//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataBase
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>  
    /// 管理角色权限表  
    /// </summary>
    public partial class Sys_Role_Nav
    {
    	/// <summary>  
    	/// 自增ID  
    	/// </summary>
        public int id { get; set; }
    	/// <summary>  
    	/// 角色ID  
    	/// </summary>
        public Nullable<int> role_id { get; set; }
    	/// <summary>  
    	/// 导航名称  
    	/// </summary>
        public string nav_name { get; set; }
    	/// <summary>  
    	/// 权限类型  
    	/// </summary>
        public string action_type { get; set; }
    }
}
