using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public partial class ShopOrder
    {
        public string StateName
        {
            get
            {
                if (State == 1)
                    return "待付款";
                if (State == 0)
                    return "待处理";
                if (State == 2)
                    return "已付款";
                if (State == 3)
                    return "已发货";
                if (State == 30)
                    return "委托中";
                if (State == 40)
                    return "挂卖中";
                if (State == 10)
                    return "完成订单";
                if (State == -2)
                    return "已关闭";
                if (State == -1)
                    return "已取消";
                return "缺少状态";
                // return Enums.ToObject<ShopEnum.OrderState>(this.State).ToString();
            }
        }



        public string GetPostStr(Xml_Shop shopConfig, bool isBao)
        {
            //爆款不参与订单满减
            //if (this.ShopOrderProducts != null)
            //{
            //    ShopProduct first = this.ShopOrderProducts.Select(q => q.ShopProduct).FirstOrDefault();
            //    if (first != null && first.IsBaoKuan())
            //    {
            //        return this.RealCongXiao + "元";
            //    }
            //}

            if (isBao)
            {
                return this.RealCongXiao + "元";
            }
            if (this.RealShopping >= shopConfig.BaoYouAmount)
                return "免运费";
            return this.RealCongXiao + "元";
        }



        public bool IsBaoQuan()
        {
            if (this.ShopOrderProducts != null)
            {
                ShopProduct first = this.ShopOrderProducts.Select(q => q.ShopProduct).FirstOrDefault();
                if (first != null && first.IsBaoKuan())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
