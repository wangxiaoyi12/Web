using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.ShopEnum;

namespace System
{
    /// <summary>
    /// 商城需要的枚举
    /// </summary>
    public class ShopEnum
    {
        #region 商城文章分类的类型  普通型   常见问题型
        /// <summary>
        /// 商城文章分类的类型  普通型   常见问题型
        /// </summary>
        public enum ArticleCategoryType
        {
            /// <summary>
            /// 普通型
            /// </summary>
            Normal = 0,
            /// <summary>
            /// 片断型，如常见问题，一个小问题一个片断
            /// </summary>
            Part = 1
        }
        #endregion

        #region 导航类型 主，底
        /// <summary>
        /// 导航类型 主，底
        /// </summary>
        public enum NavType
        {
            /// <summary>
            /// 主导航
            /// </summary>
            Main = 0,
            /// <summary>
            /// 底部导航
            /// </summary>
            Bottom = 1
        }
        #endregion

        #region 一元云购状态
        /// <summary>
        /// 一元云购状态
        /// </summary>
        public enum OneBuyState
        {
            /// <summary>
            /// 开始状态，进行中
            /// </summary>
            Start = 0,
            /// <summary>
            /// 完成状态
            /// </summary>
            Finish = 1,
            /// <summary>
            /// 结束状态
            /// </summary>
            End = 2,
            /// <summary>
            /// 取消状态
            /// </summary>
            Cancel = 11
        }
        #endregion

        #region 订单支付状态
        /// <summary>
        /// 订单支付状态
        /// </summary>
        public enum OrderPayState
        {
            /// <summary>
            /// 待支付
            /// </summary>
            WaitPay = 0,
            /// <summary>
            /// 已支付
            /// </summary>
            Pay = 1
        }
        #endregion

        #region 订单状态
        /// <summary>
        /// 订单状态
        /// </summary>
        public enum OrderState
        {
            /// <summary>
            /// 下单未确认（未付款）
            /// </summary>
            //Create=0,
            /// <summary>
            /// 提交订单：下单并确认(未支付)
            /// </summary>
            Submit = 1,
            /// <summary>
            /// 待处理
            /// </summary>
            Chu = 0,
            /// <summary>
            /// 已支付未发货
            /// </summary>
            Pay = 2,
            /// <summary>
            /// 已发货
            /// </summary>
            FaHuo = 3,
            /// <summary>
            /// 完成订单:确认收货
            /// </summary>
            Finish = 10,
            /// <summary>
            /// 已评价
            /// </summary>
            Comment = 20,
            /// <summary>
            /// 已取消
            /// </summary>
            Cancel = -1,
            /// <summary>
            /// 已关闭
            /// </summary>
            Close = -2,
            /// <summary>
            /// 已退货
            /// </summary>
            Refund = -3,
            /// <summary>
            /// 委托中
            /// </summary>
            WeiTuo = 30,
            /// <summary>
            /// 挂卖
            /// </summary>
            GuaM = 40,
        }
        #endregion
        /// <summary>
        /// 货币类型
        /// </summary>
        public enum CoinType
        {
            购物币 = 1,
            积分 = 2,
            重销币 = 3
        }
        public enum ShopType
        {
            订单商品 = 1,
            会员商品 = 63,
            爆款商品 = 19,
            促销商品 = 165,
            旅游商品 = 4
        }
        /// <summary>
        /// 支付通道
        /// </summary>
        public enum PayType
        {
            现金加积分 = 1,
            现金 = 2
        }
    }
}
