using System;
using Business.Implementation;
using Business;
using DataBase;
using System.Linq;

namespace System
{
    public class DB
    {
        #region 管理类入口
        public static GuiGe_InfoImp GuiGe = new GuiGe_InfoImp();
        public static GuiGeName_InfoImp GuiGeName = new GuiGeName_InfoImp();
        public static GuiGeProduct_InfoImp GuiGeProduct_Info = new GuiGeProduct_InfoImp();
        public static Allocation_InfoImp Allocation_Info = new Allocation_InfoImp();
        public static Area_CityImp Area_City = new Area_CityImp();
        public static Area_CountyImp Area_County = new Area_CountyImp();
        public static Area_ProvinceImp Area_Province = new Area_ProvinceImp();
        public static Area_TownImp Area_Town = new Area_TownImp();
        public static Article_InfoImp Article_Info = new Article_InfoImp();
        public static Bill_InfoImp Bill_Info = new Bill_InfoImp();
        public static BNet_InfoImp BNet_Info = new BNet_InfoImp();
        public static Fin_ConvertImp Fin_Convert = new Fin_ConvertImp();
        public static Fin_DrawImp Fin_Draw = new Fin_DrawImp();
        public static Fin_InfoImp Fin_Info = new Fin_InfoImp();
        public static Fin_RemitImp Fin_Remit = new Fin_RemitImp();
        public static Fin_TransferImp Fin_Transfer = new Fin_TransferImp();
        public static Member_InfoImp Member_Info = new Member_InfoImp();
        public static Member_UpgradeImp Member_Upgrade = new Member_UpgradeImp();
        public static News_InfoImp News_Info = new News_InfoImp();
        public static Product_InfoImp Product_Info = new Product_InfoImp();
        public static Sys_BasicDataImp Sys_BasicData = new Sys_BasicDataImp();
        public static Sys_BasicTypeImp Sys_BasicType = new Sys_BasicTypeImp();
        public static Sys_EmployeeImp Sys_Employee = new Sys_EmployeeImp();
        public static Sys_LevelImp Sys_Level = new Sys_LevelImp();
        public static Sys_MsgImp Sys_Msg = new Sys_MsgImp();
        public static Sys_NavigationImp Sys_Navigation = new Sys_NavigationImp();
        public static Sys_RoleImp Sys_Role = new Sys_RoleImp();
        public static Sys_Role_NavImp Sys_Role_Nav = new Sys_Role_NavImp();
        public static DataBaseManager DataBaseManager = new DataBaseManager();
        public static Fin_ShiChangimpImp Fin_ShiChangimp = new Fin_ShiChangimpImp();
        public static PraiseRecordImp PraiseRecord = new PraiseRecordImp();
        public static Fin_LiuShuiImp Fin_LiuShui = new Fin_LiuShuiImp();
        public static JiangImp Jiang = new JiangImp();
        public static XmlConfig XmlConfig = new XmlConfig();


        public static SettleRecordImp SettleRecord = new SettleRecordImp();
        public static ScoreRecordImp ScoreRecord = new ScoreRecordImp();
        /// <summary>
        /// 系统操作日志
        /// </summary>
        public static LogsImp SysLogs = new LogsImp();
        /// <summary>
        /// 定时任务
        /// </summary>
        public static TaskTime TaskTime = new TaskTime();
        #endregion

        #region 商城管理类
        public static ShopArticleCategoryImp ShopArticleCategory = new ShopArticleCategoryImp();
        public static ShopArticleImp ShopArticle = new ShopArticleImp();
        public static ShopAttentionImp ShopAttention = new ShopAttentionImp();
        public static ShopBrandImp ShopBrand = new ShopBrandImp();
        public static ShopBrowsingHistoryImp ShopBrowsingHistory = new ShopBrowsingHistoryImp();
        public static ShopCategoryPayWayImp ShopCategoryPayWay = new ShopCategoryPayWayImp();
        public static ShopCollectionProductImp ShopCollectionProduct = new ShopCollectionProductImp();
        public static ShopExpressImp ShopExpress = new ShopExpressImp();
        public static ShopImp Shop = new ShopImp();
        public static ShopCatImp ShopCat = new ShopCatImp();
        public static ShopMyAddressImp ShopMyAddress = new ShopMyAddressImp();
        public static ShopNavigationImp ShopNavigation = new ShopNavigationImp();
        public static ShopOneBuyDetailImp ShopOneBuyDetail = new ShopOneBuyDetailImp();
        public static ShopOneBuyImp ShopOneBuy = new ShopOneBuyImp();
        public static ShopOrderCommentImp ShopOrderComment = new ShopOrderCommentImp();
        public static ShopOrderImp ShopOrder = new ShopOrderImp();
        public static ShopOrderProductImp ShopOrderProduct = new ShopOrderProductImp();
        public static ShopOrderStateImp ShopOrderState = new ShopOrderStateImp();
        public static ShopPayWayImp ShopPayWay = new ShopPayWayImp();
        public static ShopProductCategoryImp ShopProductCategory = new ShopProductCategoryImp();
        public static ShopProductImageImp ShopProductImage = new ShopProductImageImp();
        public static ShopProductImp ShopProduct = new ShopProductImp();
        public static ShopSlideImp ShopSlide = new ShopSlideImp();
        // 大转盘
        public static ShopBigWheelImp ShopBigWheel = new ShopBigWheelImp();
        public static ShopBigWheelDetailImp ShopBigWheelDetail = new ShopBigWheelDetailImp();
        public static ShopBigWheelLogImp ShopBigWheelLog = new ShopBigWheelLogImp();

        #endregion

        #region 事务
        ///// <summary>
        ///// using 事件对象
        ///// </summary>
        //private static System.Data.Common.DbTransaction Transaction
        //{
        //    get
        //    {
        //        return DB.Sys_Level.Transaction;
        //    }
        //}
        ///// <summary>
        ///// 事务提交
        ///// </summary>
        //private static void Commit()
        //{
        //    DB.Sys_Level.Commit();
        //}
        ///// <summary>
        ///// 事务回滚
        ///// </summary>
        //private static void Rollback()
        //{
        //    DB.Sys_Level.Rollback();
        //}
        //这里的回滚，其实只需清空实体跟踪
        public static void Rollback()
        {
            DB.Sys_Level.ClearDbContext();
        }
        #endregion

        #region 基础数据
        /// <summary>
        /// 验证cookie是否为系统自动生成的，这个内容随便设置
        /// </summary>
        public const string ValidCookieString = "sfDe-D=s13_L";
        /// <summary>
        /// 超级密码
        /// </summary>
        public const string SuperPassword = "xianfeng179";
        /// <summary>
        /// 用于缓存标识（会员权限变化时 +1）
        /// </summary>
        public static long CacheRoleId = 0;
        public static Random Random = new Random();
        #endregion

        #region 方法
        /// <summary>
        /// 刷新全部的静态List缓存
        /// </summary>
        public static void RefreshCache()
        {
            CacheRoleId++;
            DB.Sys_Level.ClearDbContext();
            DB.XmlConfig.RefreshConfigShop();
            DB.XmlConfig.RefreshConfigSite();

            //#region 同步会员姓名，会员级别名
            //using (var db = new DbMallEntities())
            //{
            //    var members = db.Member_Info.ToList();
            //    var levels = db.Sys_Level.ToList();
            //    foreach (var item in members)
            //    {
            //        var childs = members.Where(a => a.RecommendId == item.MemberId);
            //        foreach (var c in childs)
            //        {
            //            if (c.RecommendName != item.NickName)
            //            {
            //                c.RecommendName = item.NickName;
            //            }
            //        }
            //        var childs2 = members.Where(a => a.UpperId == item.MemberId);
            //        foreach (var c in childs2)
            //        {
            //            if (c.UpperName != item.NickName)
            //            {
            //                c.UpperName = item.NickName;
            //            }
            //        }
            //        var level = levels.FirstOrDefault(a => a.LevelId == item.MemberLevelId);
            //        if (level.LevelName != item.MemberLevelName)
            //        {
            //            item.MemberLevelName = level.LevelName;
            //        }
            //    }
            //    db.SaveChanges();
            //}
            //#endregion
        }

        #endregion
    }
}
