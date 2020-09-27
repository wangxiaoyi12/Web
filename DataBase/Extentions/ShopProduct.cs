using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    /// <summary>
    /// 商品对象，扩展
    /// </summary>
    public partial class ShopProduct
    {
        /// <summary>
        /// 判断商品是否是订单
        /// </summary>
        /// <returns></returns>
        public bool IsDingDan()
        {
            int classid = 8;
            if (this.CategoryID == classid)
                return true;
            if (this.CategoryID1 == classid)
                return true;
            return false;
        }

        /// <summary>
        /// 判断商品是否是爆款商品
        /// </summary>
        /// <returns></returns>
        public bool IsBaoKuan()
        {
            int classid = 219;
            if (this.CategoryID == classid)
                return true;
            if (this.CategoryID1 == classid)
                return true;
            return false;
        }
        /// <summary>
        /// 判断商品是否是会员商品
        /// </summary>
        /// <returns></returns>
        public bool IsHuiYuan()
        {
            int classid = 210;
            if (this.CategoryID == classid)
                return true;
            if (this.CategoryID1 == classid)
                return true;
            return false;
        }
        /// <summary>
        /// 判断商品是否是促销商品
        /// </summary>
        /// <returns></returns>
        public bool IsCuXiao()
        {
            int classid = 165;
            if (this.CategoryID == classid)
                return true;
            if (this.CategoryID1 == classid)
                return true;
            return false;
        }
        /// <summary>
        /// 判断商品是否是旅游商品
        /// </summary>
        /// <returns></returns>
        public bool IsLvLiu()
        {
            int classid = 4;
            if (this.CategoryID == classid)
                return true;
            if (this.CategoryID1 == classid)
                return true;
            return false;
        }

        /// <summary>
        /// 是否是主流商品
        /// </summary>
        /// <returns></returns>
        public bool IsMain()
        {
            if (this.IsDingDan())
                return false;
            if (this.IsLvLiu())
                return false;
            return true;
        }

    }
}
