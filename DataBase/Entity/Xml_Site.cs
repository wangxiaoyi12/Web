using System.Collections.Generic;
using System.Linq;
using System;
using Common;

namespace DataBase
{
    /// <summary>
    /// site.config XML文件对应的实体类 
    /// </summary>
    public class Xml_Site
    {
        #region 会员系统
        /// <summary>
        /// 系统名称
        /// </summary>
        public string webname { get; set; }
        /// <summary>
        /// 系统浏览网址
        /// </summary>
        public string weburl { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string webcompany { get; set; }
        public decimal ZZP { get; set; }=2m;
        public decimal GGP { get; set; } = 1m;
        public decimal CZC { get; set; } = 1m;
        public decimal ZGC { get; set; } = 1m;
        public int ZT1 { get; set; } = 5;
        public int ZT2 { get; set; } = 5;
        public int ZT3 { get; set; } = 5;
        public int ZT4 { get; set; } = 5;
        public int ZT5 { get; set; } = 5;

        public int TD1 { get; set; } = 5;
        public int TD2 { get; set; } = 5;
        public int TD3 { get; set; } = 5;
        public int TD4 { get; set; } = 5;
        public int TD5 { get; set; } = 5;
        /// <summary>
        /// 微信
        /// </summary>
        public string img { get; set; }
        /// <summary>
        /// 支付宝
        /// </summary>
        public string imgZhi { get; set; }

        /// <summary>
        /// 店报单费
        /// </summary>
        public decimal? ActiveFee { get; set; }
        /// <summary>
        /// 提现倍数
        /// </summary>
        public decimal? Multiple { get; set; }
        /// <summary>
        /// 最小提现金额
        /// </summary>
        public decimal? MinAmount { get; set; }
        /// <summary>
        /// 互转倍数
        /// </summary>
        public decimal? MultipleHuZ { get; set; }
        /// <summary>
        /// 最小互转金额
        /// </summary>
        public decimal? MinAmountHuZ { get; set; }
        public decimal Scores { get; set; }
        /// <summary>
        /// 税金
        /// </summary>
        public decimal? Poundage { get; set; }
        public decimal? LingDao1 { get; set; }
        public decimal? GouWuFanL { get; set; }
        public decimal? MeiRiFanL { get; set; }
        public decimal? FenXiang1 { get; set; }
        public decimal? FenXiang2 { get; set; }
        public decimal? FuDao { get; set; }
        public decimal? LingDao2 { get; set; }
        public decimal? LingDao3 { get; set; }
        public decimal? ChongXiao { get; set; }
        public decimal? ZhuanMaiD { get; set; }
        public decimal? ZhiYingD { get; set; }
        public decimal? RecAwardRate2 { get; set; }
        public decimal DayMaxTotalAmountFlower { get; set; }
        public decimal? ManagerAwardRate { get; set; }
        public decimal? StockPrice { get; set; }
        public decimal? FuWuJiang1 { get; set; }
        public decimal? FuWuJiang2 { get; set; }
        public decimal? FuWuJiang3 { get; set; }
        /// <summary>
        /// 提现时间
        /// </summary>
        public string DrawName { get; set; }
        /// <summary>
        /// 推广奖直推第3人比例
        /// </summary>
        public decimal? RecAwardRate3 { get; set; }
        /// <summary>
        /// 推广奖直推第4人比例
        /// </summary>
        public decimal? RecAwardRate4 { get; set; }
        /// <summary>
        /// 推广奖直推第3、4两人封顶比例
        /// </summary>
        public decimal? RecAwardMaxRate34 { get; set; }
        /// <summary>
        /// 提现手续费
        /// </summary>
        public decimal? DrawPoundage { get; set; }
        /// <summary>
        /// 系统开关
        /// </summary>
        public string webstatus { get; set; }
        /// <summary>
        /// 维护/关闭提示
        /// </summary>
        public string webclosereason { get; set; }
        /// <summary>
        /// 收益是否秒结
        /// </summary>
        public bool IsMiaoJie { get; set; }

        public bool IsJiHuo { get; set; }


        public string sysuser { get; set; }

        public string sysrole { get; set; }
        /// <summary>
        /// 网络类型(多用于网络图的 双轨，三轨，N代表N轨)
        /// </summary>
        //public int NetType { get; set; } = 2;
        /// <summary>
        /// 网络图层数，默认3层
        /// </summary>
        //public int NetLayer { get; set; } = 4;
        /// <summary>
        /// 分红百分比率(10%)
        /// </summary>
        public decimal FenHongBi { get; set; } = 10;


        #region 本站使用

        #region 积分设置
        /// <summary>
        /// 注册送积分倍数5倍
        /// </summary>
        public decimal Reg_Score { get; set; }
        /// <summary>
        /// 充值以100为倍数
        /// </summary>
        public decimal ScoreMultiple { get; set; }
        /// <summary>
        /// 充值以100为倍数
        /// </summary>
        public decimal ReCount { get; set; }
        public decimal TuChuBiLi { get; set; } = 4;
        public decimal DaiLiTuiJian { get; set; } = 10;
        public decimal DaiLiAmount { get; set; } = 1000;
        public decimal DaiLiJinTie { get; set; } = 10;
        /// <summary>
        /// 充值以100为倍数
        /// </summary>
        public decimal SanCount { get; set; }
        /// <summary>
        /// 充值送积分倍数10倍
        /// </summary>
        public decimal Remit_Score { get; set; }
        /// <summary>
        /// 充值送积分倍数10倍
        /// </summary>
        public string ZhiFuB { get; set; }
        /// <summary>
        /// 充值送积分倍数10倍
        /// </summary>
        public decimal YinHang { get; set; }
        /// <summary>
        /// 充值送积分倍数10倍
        /// </summary>
        public string ZhiFuBName { get; set; }
        /// <summary>
        /// 充值送积分倍数10倍
        /// </summary>
        public string YinHangName { get; set; }
        public string YinHangBankName { get; set; } = "张三";

        #endregion

        /// <summary>
        /// 互助奖 10%，直推，间推的奖 加权分配给直推会员
        /// </summary>
        public decimal Jing_HuZhu { get; set; } = 10;


        /// <summary>
        /// 师长分红比例
        /// </summary>
        public decimal SZFH_Bi { get; set; } = 5;
        /// <summary>
        /// 社区店分红比例
        /// </summary>
        public decimal SQDFH_Bi { get; set; } = 5;


        #region 旅游积分
        /// <summary>
        /// 收入1万送1个旅游积分 
        /// </summary>
        public decimal TourScoresStep { get; set; } = 10000;
        /// <summary>
        /// 赠送旅游积分数量 
        /// </summary>
        public decimal TourScores { get; set; } = 1;
        #endregion
        /// <summary>
        /// 重复消费最低限额89元
        /// </summary>
        public decimal CongXiao_Min { get; set; } = 89;

        #region 会员商品
        /// <summary>
        /// 重复消费社区享受10%
        /// </summary>
        public decimal CongXiao_SQ { get; set; } = 10;
        /// <summary>
        /// 重复消费直推享受10%
        /// </summary>
        public decimal CongXiao_Rem1 { get; set; } = 10;
        /// <summary>
        /// 重复消费间推享受5%
        /// </summary>
        public decimal CongXiao_Rem2 { get; set; } = 5;
        #endregion

        #region 爆款商品
        /// <summary>
        /// 爆款区社区享受10%
        /// </summary>
        public decimal BaoKuan_SQ { get; set; } = 10;
        #endregion

        #region 促销商品
        /// <summary>
        /// 促销商品社区店享受
        /// </summary>
        public decimal CX_SQ { get; set; } = 5;
        /// <summary>
        /// 促销商品一级推荐享受5%
        /// </summary>
        public decimal CX_Rem1 { get; set; } = 5;
        /// <summary>
        /// 促销商品二级推荐享受2.5%
        /// </summary>
        public decimal CX_Rem2 { get; set; } = 2.5m;
        #endregion

        /// <summary>
        /// 购物币比例
        /// </summary>
        public decimal ShopCoinsPercent { get; set; } = 10;

        #region 领导奖
        /// <summary>
        /// 一星，推荐人数
        /// </summary>
        public int Lead_RecCount { get; set; } = 10;


        /// <summary>
        /// 领导奖
        /// </summary>
        public string Lead_YeJi_List { get; set; }
        /// <summary>
        /// 领导奖，业绩条件对应的星级
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public int GetLead_YeJi(decimal yeji)
        {
            Dictionary<int, decimal> dic = GetLead_YeJi_List();
            return dic.Where(q => q.Value <= yeji)
                .DefaultIfEmpty().Select(q => q.Key).Max();
        }
        public Dictionary<int, decimal> GetLead_YeJi_List()
        {
            Dictionary<int, decimal> dic = new Dictionary<int, decimal>();
            if (string.IsNullOrEmpty(this.Lead_YeJi_List))
            {
                dic.Add(1, 2 * 10000);
                dic.Add(2, 5 * 10000);
                dic.Add(3, 8 * 10000);
                dic.Add(4, 17 * 10000);
                dic.Add(5, 57 * 10000);
                dic.Add(6, 107 * 10000);
            }
            else
            {
                dic = this.Lead_YeJi_List.JsonDeserializer<Dictionary<int, decimal>>();
            }

            return dic;
        }


        public string Lead_Percent_List { get; set; }
        /// <summary>
        /// 获取领导奖比例
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public decimal GetLead_Percent(int level)
        {
            if (level <= 0 || level > 6)
                return 0;
            Dictionary<int, decimal> dic = GetLead_Percent_List();

            return dic.Where(q => q.Key == level).First().Value;
        }
        public Dictionary<int, decimal> GetLead_Percent_List()
        {
            Dictionary<int, decimal> dic = new Dictionary<int, decimal>();
            if (string.IsNullOrEmpty(this.Lead_Percent_List))
            {
                dic.Add(1, 2);
                dic.Add(2, 3);
                dic.Add(3, 4);
                dic.Add(4, 6);
                dic.Add(5, 8);
                dic.Add(6, 10);
            }
            else
            {
                dic = this.Lead_Percent_List.JsonDeserializer<Dictionary<int, decimal>>();
            }
            return dic;
        }
        #endregion

        #endregion

        /// <summary>
        /// 测试金额
        /// </summary>
        public decimal TestAmount { get; set; } = 0.01M;

        public decimal GetAmount(decimal amount)
        {
            if (this.TestAmount > 0)
                return this.TestAmount;
            return amount;
        }



        /// <summary>
        /// 推店奖
        /// </summary>
        public decimal TDJPercent { get; set; } = 5;

        /// <summary>
        /// 报单产品-服务费
        /// </summary>
        public decimal BDServicePercent { get; set; } = 5;
        /// <summary>
        /// 复销产品-服务费
        /// </summary>
        public decimal FXServicePercent { get; set; } = 10;
        #endregion
    }
}
