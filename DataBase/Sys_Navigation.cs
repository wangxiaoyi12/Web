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
    /// 系统导航菜单  
    /// </summary>
    public partial class Sys_Navigation
    {
    	/// <summary>  
    	/// 自增ID  
    	/// </summary>
        public int id { get; set; }
    	/// <summary>  
    	/// 导航类别  
    	/// </summary>
        public string nav_type { get; set; }
    	/// <summary>  
    	/// 导航ID  
    	/// </summary>
        public string name { get; set; }
    	/// <summary>  
    	/// 标题  
    	/// </summary>
        public string title { get; set; }
    	/// <summary>  
    	/// 副标题  
    	/// </summary>
        public string sub_title { get; set; }
    	/// <summary>  
    	/// 链接地址  
    	/// </summary>
        public string link_url { get; set; }
    	/// <summary>  
    	/// 排序数字  
    	/// </summary>
        public Nullable<int> sort_id { get; set; }
    	/// <summary>  
    	/// 是否隐藏0显示1隐藏  
    	/// </summary>
        public Nullable<byte> is_lock { get; set; }
    	/// <summary>  
    	/// 备注说明  
    	/// </summary>
        public string remark { get; set; }
    	/// <summary>  
    	/// 所属父导航ID  
    	/// </summary>
        public Nullable<int> parent_id { get; set; }
    	/// <summary>  
    	/// 菜单ID列表(逗号分隔开)  
    	/// </summary>
        public string class_list { get; set; }
    	/// <summary>  
    	/// 导航深度  
    	/// </summary>
        public Nullable<int> class_layer { get; set; }
    	/// <summary>  
    	/// 所属频道ID  
    	/// </summary>
        public Nullable<int> channel_id { get; set; }
    	/// <summary>  
    	/// 权限资源  
    	/// </summary>
        public string action_type { get; set; }
    	/// <summary>  
    	/// 系统默认  
    	/// </summary>
        public Nullable<byte> is_sys { get; set; }
    	/// <summary>  
    	///   
    	/// </summary>
        public string icon { get; set; }
    }
}
