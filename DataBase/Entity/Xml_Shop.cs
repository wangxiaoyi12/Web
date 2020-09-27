using System.Linq;

namespace DataBase
{
    /// <summary>
    /// site.config XML文件对应的实体类 
    /// </summary>
    public class Xml_Shop
    {
        #region 商城
        /// <summary>
        /// 商城名称
        /// </summary>
        public string ShopName { get; set; } = "商城";
        /// <summary>
        /// 商城Logo
        /// </summary>
        public string ShopLogo { get; set; }
        /// <summary>
        /// 公司
        /// </summary>
        public string ShopCompany { get; set; }
        /// <summary>
        /// 通讯地址
        /// </summary>
        public string ShopAddress { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string ShopTel { get; set; }
        /// <summary>
        /// 传真
        /// </summary>
        public string ShopFax { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string ShopEmail { get; set; }
        /// <summary>
        /// ICP备案号
        /// </summary>
        public string ShopICP { get; set; }
        /// <summary>
        /// 商城是否正在运行状态
        /// </summary>
        public bool ShopIsRunning { get; set; } = true;
        /// <summary>
        /// 关闭维护原因
        /// </summary>
        public string ShopCloseReason { get; set; } = "网站维护中，请稍候访问……";
        /// <summary>
        /// 商城统计代码
        /// </summary>
        public string ShopStatisticalCode { get; set; }
        /// <summary>
        /// 商城版权
        /// </summary>
        public string ShopCopyright { get; set; } = "Copyright ";
        /// <summary>
        /// 商城首页标题
        /// </summary>
        public string ShopIndexTitle { get; set; } = "商城首页";
        /// <summary>
        /// 商城首页关键字
        /// </summary>
        public string ShopIndexKeyword { get; set; } = "商城关键字";
        /// <summary>
        /// 商城首页描述
        /// </summary>
        public string ShopIndexDescription { get; set; } = "商城描述";
        /// <summary>
        /// QQ
        /// </summary>
        public string QQ { get; set; }
        /// <summary>
        /// WeiXin
        /// </summary>
        public string WeiXin { get; set; }
        #endregion



        #region 手机版参数
        public string TopBack { get; set; }
        public string BottomBack { get; set; }
        #endregion

        /// <summary>
        /// 手机版滚动文字
        /// </summary>
        public string GDSummary { get; set; } = "首页滚动文字内容，需要超过30个字。";
        /// <summary>
        /// 在线人数倍数
        /// </summary>
        public int Multiple { get; set; } = 1;
        /// <summary>
        /// 在线人数
        /// </summary>
        public static int OnlineCount { get; set; }
        public int GetOnlineCount()
        {
            return this.Multiple * OnlineCount;
        }


        /// <summary>
        /// 会员商品-图标
        /// </summary>
        public string Img_HY { get; set; }
        /// <summary>
        /// 爆款商品-图标
        /// </summary>
        public string Img_BK { get; set; }
        /// <summary>
        /// 活动专区-图标
        /// </summary>
        public string Img_HD { get; set; }

        /// <summary> 
        /// 包邮订单金额
        /// </summary>
        public decimal BaoYouAmount { get; set; } = 100;





        /// <summary>
        /// 获取总金额，处理邮费
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public decimal GetTotal_YouFei(ShopOrder orderPro)
        {
            return orderPro.RealShopping;
        }
        public decimal GetTotal_XianJin(ShopOrder orderPro)
        {
            return orderPro.RealScore;
        }
        public decimal GetTotal_GouWu(ShopOrder orderPro)
        {
            return orderPro.RealCongXiao;
        }
        /// <summary>
        /// 获取总金额，处理邮费---全额电子币、现金
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public decimal GetTotal_YouFei2(ShopOrder orderPro)
        {
            return orderPro.RealScore;
        }
        /// <summary>
        /// 获取邮费显示--订单
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public decimal GetYouFei(ShopOrder orderPro, bool isBao)
        {
            if (orderPro.RealAmount >= BaoYouAmount && isBao == false)
                return 0;
            return orderPro.RealCongXiao;
        }

    }
}
