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
    ///   
    /// </summary>
    public partial class Article_Info
    {
    	/// <summary>  
    	///   
    	/// </summary>
        public int ArticleId { get; set; }
    	/// <summary>  
    	/// 标题  
    	/// </summary>
        public string Title { get; set; }
    	/// <summary>  
    	/// 最后编辑时间  
    	/// </summary>
        public Nullable<System.DateTime> LastEditTime { get; set; }
    	/// <summary>  
    	/// 内容  
    	/// </summary>
        public string Comment { get; set; }
    	/// <summary>  
    	/// 阅读次数  
    	/// </summary>
        public Nullable<int> ReadNum { get; set; }
    }
}
