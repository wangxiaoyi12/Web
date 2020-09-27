using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common;
using DataBase;
namespace Business
{
    /// <summary>
    /// 商品/订单，评论
    /// </summary>
    public class ShopOrderCommentImp :EFBase<DataBase.ShopOrderComment>
    {
        /// <summary>
        /// 获取指定商品的评论数量
        /// </summary>
        /// <param name="product">商品对象</param>
        /// <returns></returns>
        public int GetCount(ShopProduct product)
        {
            return DB.ShopOrderComment.Where(q => q.ProductID == product.ID).Count();
        }

    }
}
