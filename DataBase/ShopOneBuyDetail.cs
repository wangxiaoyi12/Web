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
    /// 一元云购记录  
    /// </summary>
    public partial class ShopOneBuyDetail
    {
    	/// <summary>  
    	/// ID  
    	/// </summary>
        public long ID { get; set; }
    	/// <summary>  
    	/// 云购ID  
    	/// </summary>
        public int OneBuyID { get; set; }
    	/// <summary>  
    	/// 参与次数  
    	/// </summary>
        public int Count { get; set; }
    	/// <summary>  
    	/// 参与时间  
    	/// </summary>
        public System.DateTime CreateTime { get; set; }
    	/// <summary>  
    	/// 会员id  
    	/// </summary>
        public string MemberID { get; set; }
    	/// <summary>  
    	/// 会员编号  
    	/// </summary>
        public string MemberCode { get; set; }
    	/// <summary>  
    	/// 会员名  
    	/// </summary>
        public string NickName { get; set; }
    
        public virtual ShopOneBuy ShopOneBuy { get; set; }
    }
}