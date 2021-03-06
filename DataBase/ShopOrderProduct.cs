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
    /// 订单商品表  
    /// </summary>
    public partial class ShopOrderProduct
    {
    	/// <summary>  
    	/// ID  
    	/// </summary>
        public int ID { get; set; }
    	/// <summary>  
    	/// 订单ID  
    	/// </summary>
        public string OrderID { get; set; }
    	/// <summary>  
    	/// 商品ID  
    	/// </summary>
        public int ProductID { get; set; }
    	/// <summary>  
    	/// 商品显示名  
    	/// </summary>
        public string Name { get; set; }
    	/// <summary>  
    	/// 数量  
    	/// </summary>
        public int Count { get; set; }
    	/// <summary>  
    	/// 价格  
    	/// </summary>
        public decimal Price { get; set; }
    	/// <summary>  
    	/// 金额  
    	/// </summary>
        public decimal Money { get; set; }
    	/// <summary>  
    	///   
    	/// </summary>
        public string MemberID { get; set; }
    	/// <summary>  
    	///   
    	/// </summary>
        public decimal MoneyShopping { get; set; }
    	/// <summary>  
    	///   
    	/// </summary>
        public decimal MoneyCongXiao { get; set; }
    	/// <summary>  
    	///   
    	/// </summary>
        public decimal MoneyScore { get; set; }
    	/// <summary>  
    	///   
    	/// </summary>
        public decimal PriceShopping { get; set; }
    	/// <summary>  
    	///   
    	/// </summary>
        public decimal PriceCongXiao { get; set; }
    	/// <summary>  
    	///   
    	/// </summary>
        public decimal PriceScore { get; set; }
    	/// <summary>  
    	///   
    	/// </summary>
        public string GuiGe { get; set; }
    
        public virtual ShopOrder ShopOrder { get; set; }
        public virtual ShopProduct ShopProduct { get; set; }
    }
}
