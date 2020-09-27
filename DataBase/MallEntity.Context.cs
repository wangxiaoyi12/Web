﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class DbMallEntities : DbContext
    {
        public DbMallEntities()
            : base("name=DbMallEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Allocation_Info> Allocation_Info { get; set; }
        public virtual DbSet<Area_City> Area_City { get; set; }
        public virtual DbSet<Area_County> Area_County { get; set; }
        public virtual DbSet<Area_Province> Area_Province { get; set; }
        public virtual DbSet<Area_Town> Area_Town { get; set; }
        public virtual DbSet<Article_Info> Article_Info { get; set; }
        public virtual DbSet<Bill_Info> Bill_Info { get; set; }
        public virtual DbSet<Fin_Convert> Fin_Convert { get; set; }
        public virtual DbSet<Fin_Draw> Fin_Draw { get; set; }
        public virtual DbSet<Fin_Transfer> Fin_Transfer { get; set; }
        public virtual DbSet<Logs> Logs { get; set; }
        public virtual DbSet<Member_Upgrade> Member_Upgrade { get; set; }
        public virtual DbSet<News_Info> News_Info { get; set; }
        public virtual DbSet<Product_Info> Product_Info { get; set; }
        public virtual DbSet<Sys_BasicData> Sys_BasicData { get; set; }
        public virtual DbSet<Sys_BasicType> Sys_BasicType { get; set; }
        public virtual DbSet<Sys_Employee> Sys_Employee { get; set; }
        public virtual DbSet<Sys_Level> Sys_Level { get; set; }
        public virtual DbSet<Sys_Msg> Sys_Msg { get; set; }
        public virtual DbSet<Sys_Navigation> Sys_Navigation { get; set; }
        public virtual DbSet<Sys_Role> Sys_Role { get; set; }
        public virtual DbSet<Sys_Role_Nav> Sys_Role_Nav { get; set; }
        public virtual DbSet<VEmp> VEmp { get; set; }
        public virtual DbSet<ShopArticle> ShopArticles { get; set; }
        public virtual DbSet<ShopArticleCategory> ShopArticleCategories { get; set; }
        public virtual DbSet<ShopAttention> ShopAttentions { get; set; }
        public virtual DbSet<ShopBrand> ShopBrands { get; set; }
        public virtual DbSet<ShopBrowsingHistory> ShopBrowsingHistories { get; set; }
        public virtual DbSet<ShopCategoryPayWay> ShopCategoryPayWays { get; set; }
        public virtual DbSet<ShopCollectionProduct> ShopCollectionProducts { get; set; }
        public virtual DbSet<ShopExpress> ShopExpresses { get; set; }
        public virtual DbSet<ShopNavigation> ShopNavigations { get; set; }
        public virtual DbSet<ShopOneBuy> ShopOneBuys { get; set; }
        public virtual DbSet<ShopOneBuyDetail> ShopOneBuyDetails { get; set; }
        public virtual DbSet<ShopOrder> ShopOrders { get; set; }
        public virtual DbSet<ShopOrderComment> ShopOrderComments { get; set; }
        public virtual DbSet<ShopOrderProduct> ShopOrderProducts { get; set; }
        public virtual DbSet<ShopOrderState> ShopOrderStates { get; set; }
        public virtual DbSet<ShopPayWay> ShopPayWays { get; set; }
        public virtual DbSet<ShopProductImage> ShopProductImages { get; set; }
        public virtual DbSet<ShopProductPayway> ShopProductPayways { get; set; }
        public virtual DbSet<Shop> Shops { get; set; }
        public virtual DbSet<BNet_Info> BNet_Info { get; set; }
        public virtual DbSet<Fin_Info> Fin_Info { get; set; }
        public virtual DbSet<ShopCat> ShopCats { get; set; }
        public virtual DbSet<ShopSlide> ShopSlides { get; set; }
        public virtual DbSet<ShopBigWheel> ShopBigWheels { get; set; }
        public virtual DbSet<ShopBigWheelDetail> ShopBigWheelDetails { get; set; }
        public virtual DbSet<ShopBigWheelLog> ShopBigWheelLogs { get; set; }
        public virtual DbSet<SettleRecord> SettleRecords { get; set; }
        public virtual DbSet<ScoreRecord> ScoreRecords { get; set; }
        public virtual DbSet<PraiseRecord> PraiseRecords { get; set; }
        public virtual DbSet<Fin_ShiChangimp> Fin_ShiChangimp { get; set; }
        public virtual DbSet<Fin_LiuShui> Fin_LiuShui { get; set; }
        public virtual DbSet<ShopMyAddress> ShopMyAddress { get; set; }
        public virtual DbSet<GuiGe_Info> GuiGe_Info { get; set; }
        public virtual DbSet<GuiGeName_Info> GuiGeName_Info { get; set; }
        public virtual DbSet<GuiGeProduct_Info> GuiGeProduct_Info { get; set; }
        public virtual DbSet<ShopProduct> ShopProducts { get; set; }
        public virtual DbSet<ShopProductCategory> ShopProductCategories { get; set; }
        public virtual DbSet<Member_Info> Member_Info { get; set; }
        public virtual DbSet<Fin_Remit> Fin_Remit { get; set; }
    
        public virtual int UP_PageView(string tbname, string fieldKey, Nullable<int> pageCurrent, Nullable<int> pageSize, string fieldShow, string fieldOrder, string whereString, ObjectParameter recordCount)
        {
            var tbnameParameter = tbname != null ?
                new ObjectParameter("tbname", tbname) :
                new ObjectParameter("tbname", typeof(string));
    
            var fieldKeyParameter = fieldKey != null ?
                new ObjectParameter("FieldKey", fieldKey) :
                new ObjectParameter("FieldKey", typeof(string));
    
            var pageCurrentParameter = pageCurrent.HasValue ?
                new ObjectParameter("PageCurrent", pageCurrent) :
                new ObjectParameter("PageCurrent", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("PageSize", pageSize) :
                new ObjectParameter("PageSize", typeof(int));
    
            var fieldShowParameter = fieldShow != null ?
                new ObjectParameter("FieldShow", fieldShow) :
                new ObjectParameter("FieldShow", typeof(string));
    
            var fieldOrderParameter = fieldOrder != null ?
                new ObjectParameter("FieldOrder", fieldOrder) :
                new ObjectParameter("FieldOrder", typeof(string));
    
            var whereStringParameter = whereString != null ?
                new ObjectParameter("WhereString", whereString) :
                new ObjectParameter("WhereString", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UP_PageView", tbnameParameter, fieldKeyParameter, pageCurrentParameter, pageSizeParameter, fieldShowParameter, fieldOrderParameter, whereStringParameter, recordCount);
        }
    
        public virtual int UP_Split(string symbol, ObjectParameter sourcestring, ObjectParameter outstring)
        {
            var symbolParameter = symbol != null ?
                new ObjectParameter("symbol", symbol) :
                new ObjectParameter("symbol", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UP_Split", symbolParameter, sourcestring, outstring);
        }
    }
}