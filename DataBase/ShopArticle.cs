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
    /// 商城文章  
    /// </summary>
    public partial class ShopArticle
    {
    	/// <summary>  
    	/// ID  
    	/// </summary>
        public int ID { get; set; }
    	/// <summary>  
    	/// 标题  
    	/// </summary>
        public string Title { get; set; }
    	/// <summary>  
    	/// 作者  
    	/// </summary>
        public string Author { get; set; }
    	/// <summary>  
    	/// 来源  
    	/// </summary>
        public string Source { get; set; }
    	/// <summary>  
    	/// 内容  
    	/// </summary>
        public string Content { get; set; }
    	/// <summary>  
    	/// 是否显示  
    	/// </summary>
        public bool IsShow { get; set; }
    	/// <summary>  
    	/// 所属类别  
    	/// </summary>
        public int CategoryID { get; set; }
    	/// <summary>  
    	/// 添加时间  
    	/// </summary>
        public System.DateTime CreateTime { get; set; }
    	/// <summary>  
    	/// 点击量  
    	/// </summary>
        public int Clicks { get; set; }
    	/// <summary>  
    	/// 是否审核  
    	/// </summary>
        public bool IsCheck { get; set; }
    
        public virtual ShopArticleCategory ShopArticleCategory { get; set; }
    }
}
