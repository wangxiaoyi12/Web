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
    public partial class Sys_Level
    {
    	/// <summary>  
    	///   
    	/// </summary>
        public int LevelId { get; set; }
    	/// <summary>  
    	///   
    	/// </summary>
        public string LevelName { get; set; }
    	/// <summary>  
    	/// 投资金额  
    	/// </summary>
        public Nullable<decimal> Investment { get; set; }
    	/// <summary>  
    	/// 推荐一代奖励  
    	/// </summary>
        public Nullable<decimal> RecAward1 { get; set; }
    	/// <summary>  
    	/// 推荐2代奖励  
    	/// </summary>
        public Nullable<decimal> FindPointMoney { get; set; }
    	/// <summary>  
    	/// 推荐3代奖励  
    	/// </summary>
        public Nullable<int> FindPointLayer { get; set; }
    	/// <summary>  
    	/// 升级条件左右区各累计金额  
    	/// </summary>
        public Nullable<decimal> MinInvestment { get; set; }
    	/// <summary>  
    	/// 升级条件左右区各有最小级别  
    	/// </summary>
        public Nullable<int> MinLevelId { get; set; }
    	/// <summary>  
    	/// 奖励团退新增业务比例(极差)  
    	/// </summary>
        public Nullable<decimal> TeamAwardRange { get; set; }
    	/// <summary>  
    	/// 奖励团退新增业务比例  
    	/// </summary>
        public Nullable<decimal> TeamAward { get; set; }
    	/// <summary>  
    	/// 全球业绩比例  
    	/// </summary>
        public Nullable<decimal> GlobalAward { get; set; }
    	/// <summary>  
    	/// 对碰比例  
    	/// </summary>
        public Nullable<decimal> DuiPeng { get; set; }
    	/// <summary>  
    	/// 层碰比例  
    	/// </summary>
        public Nullable<decimal> LayerPeng { get; set; }
    	/// <summary>  
    	/// 直推二代  
    	/// </summary>
        public Nullable<decimal> RecAward2 { get; set; }
    	/// <summary>  
    	///   
    	/// </summary>
        public Nullable<decimal> MaxDay { get; set; }
    	/// <summary>  
    	///   
    	/// </summary>
        public Nullable<decimal> LingDao1_3 { get; set; }
    	/// <summary>  
    	///   
    	/// </summary>
        public Nullable<decimal> LingDao4_6 { get; set; }
    	/// <summary>  
    	///   
    	/// </summary>
        public Nullable<decimal> LingDao7_9 { get; set; }
    }
}
