using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    /// <summary>
    /// 产品收藏，数据操作
    /// </summary>
    public class ShopCollectionProductImp :EFBase<DataBase.ShopCollectionProduct>
    {
        /// <summary>
        /// 获取指定产品的收藏数量
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        public int GetCount(int ProductID)
        {
            return DB.ShopCollectionProduct.Where(q => q.ProductID == ProductID).Count();
        }
    }
}
