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
    public partial class Product_Info
    {
    	/// <summary>  
    	///   
    	/// </summary>
        public int ProductId { get; set; }
    	/// <summary>  
    	/// 商品类别Id  
    	/// </summary>
        public Nullable<int> CategoryId { get; set; }
    	/// <summary>  
    	/// 商品类别名  
    	/// </summary>
        public string CategoryName { get; set; }
    	/// <summary>  
    	/// 商品名称  
    	/// </summary>
        public string ProductName { get; set; }
    	/// <summary>  
    	/// 规格型号  
    	/// </summary>
        public string ModelName { get; set; }
    	/// <summary>  
    	/// 图片路径  
    	/// </summary>
        public string ImgUrl { get; set; }
    	/// <summary>  
    	/// 原价  
    	/// </summary>
        public Nullable<decimal> OriginalPrice { get; set; }
    	/// <summary>  
    	/// 售价  
    	/// </summary>
        public Nullable<decimal> SalePrice { get; set; }
    	/// <summary>  
    	/// 折扣  
    	/// </summary>
        public string Discount { get; set; }
    	/// <summary>  
    	/// 浏览量  
    	/// </summary>
        public Nullable<decimal> PV { get; set; }
    	/// <summary>  
    	/// 库存数量  
    	/// </summary>
        public Nullable<int> StockQty { get; set; }
    	/// <summary>  
    	/// 销售数量  
    	/// </summary>
        public Nullable<int> SaledQty { get; set; }
    	/// <summary>  
    	/// 排序号  
    	/// </summary>
        public Nullable<int> SortValue { get; set; }
    	/// <summary>  
    	/// 状态：在售，停售  
    	/// </summary>
        public string State { get; set; }
    	/// <summary>  
    	/// 商品详情  
    	/// </summary>
        public string ProductComment { get; set; }
    	/// <summary>  
    	/// 发布时间，可修改  
    	/// </summary>
        public Nullable<System.DateTime> PubTime { get; set; }
    	/// <summary>  
    	/// 创建时间  
    	/// </summary>
        public Nullable<System.DateTime> CreateTime { get; set; }
    	/// <summary>  
    	///   
    	/// </summary>
        public string CreateEmpId { get; set; }
    	/// <summary>  
    	///   
    	/// </summary>
        public string CreateEmpName { get; set; }
    }
}