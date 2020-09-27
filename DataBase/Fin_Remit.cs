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
    public partial class Fin_Remit
    {
    	/// <summary>  
    	/// 汇款Id  
    	/// </summary>
        public int RemitId { get; set; }
    	/// <summary>  
    	/// 会员Id  
    	/// </summary>
        public string MemberId { get; set; }
    	/// <summary>  
    	/// 会员编号  
    	/// </summary>
        public string MemberCode { get; set; }
    	/// <summary>  
    	/// 会员姓名  
    	/// </summary>
        public string NickName { get; set; }
    	/// <summary>  
    	/// 银行名  
    	/// </summary>
        public string BankName { get; set; }
    	/// <summary>  
    	/// 开户行  
    	/// </summary>
        public string OpenBank { get; set; }
    	/// <summary>  
    	/// 开户行地址  
    	/// </summary>
        public string BankAddress { get; set; }
    	/// <summary>  
    	/// 账户名  
    	/// </summary>
        public string BankAccount { get; set; }
    	/// <summary>  
    	/// 帐号  
    	/// </summary>
        public string BankCode { get; set; }
    	/// <summary>  
    	/// 汇款金额  
    	/// </summary>
        public Nullable<decimal> Amount { get; set; }
    	/// <summary>  
    	/// 汇款时间  
    	/// </summary>
        public Nullable<System.DateTime> RemitTime { get; set; }
    	/// <summary>  
    	/// 汇款说明  
    	/// </summary>
        public string Comment { get; set; }
    	/// <summary>  
    	/// 服务中心Id  
    	/// </summary>
        public string ServiceCenterId { get; set; }
    	/// <summary>  
    	/// 服务中心编号  
    	/// </summary>
        public string ServiceCenterCode { get; set; }
    	/// <summary>  
    	/// 创建时间  
    	/// </summary>
        public Nullable<System.DateTime> CreateTime { get; set; }
    	/// <summary>  
    	/// 汇款状态：未确认，已确认  
    	/// </summary>
        public string RemitState { get; set; }
    	/// <summary>  
    	/// 确认时间  
    	/// </summary>
        public Nullable<System.DateTime> ConfirmTime { get; set; }
    	/// <summary>  
    	/// 确认人Id  
    	/// </summary>
        public string ConfirmEmpId { get; set; }
    	/// <summary>  
    	/// 确认人姓名  
    	/// </summary>
        public string ConfirmEmpName { get; set; }
    	/// <summary>  
    	///   
    	/// </summary>
        public string Image { get; set; }
    }
}
