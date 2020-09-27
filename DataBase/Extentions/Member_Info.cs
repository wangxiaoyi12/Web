using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    /// <summary>
    /// 会员表信息，扩展
    /// </summary>
    public partial class Member_Info: IHasLngAndLat
    {
        /// <summary>
        /// 判断用户是否是正式会员
        /// </summary>
        public bool IsVip()
        {
            return this.IsActive == "已激活";
        }

        /// <summary>
        /// 获取用户的价格
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public decimal GetProductPrice(ShopProduct product)
        {
            //if (IsVip())
            //    return product.PriceVip;
            return product.PriceShopping;
        }




        //public string StarLevelName
        //{
        //    get {
        //        if (this.StarLevel == 0)
        //            return "空";
        //        return Enums.ToObject<Enums.StarLevel>(this.StarLevel).ToString();
        //    }
        //}
    }

    /// <summary>
    /// 具有经纬度
    /// </summary>
    public interface IHasLngAndLat
    {
        /// <summary>
        /// 经度
        /// </summary>
        Nullable<double> JingDu { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        Nullable<double> WeiDu { get; set; }
    }
}
