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
    /// 商品浏览记录
    /// </summary>
    public class ShopBrowsingHistoryImp :EFBase<DataBase.ShopBrowsingHistory>
    {
        /// <summary>
        /// 添加产品的浏览记录
        /// </summary>
        /// <param name="ProductID">产品ID</param>
        /// <param name="curUser">当前用户</param>
        /// <returns></returns>
        public bool Add(int ProductID, string MemberID)
        {
            //判断是否已经存在
            if (DB.ShopBrowsingHistory.Any(q => q.ProductID == ProductID && q.MemberID == MemberID))
            {
                ShopBrowsingHistory model = DB.ShopBrowsingHistory.FindEntity(q => q.ProductID == ProductID && q.MemberID == MemberID);
                model.CreateTime = DateTime.Now;
                return DB.ShopBrowsingHistory.Update(model);
            }
            else
            {
                ShopBrowsingHistory model = new ShopBrowsingHistory();
                model.ProductID = ProductID;
                model.MemberID = MemberID;
                model.CreateTime = DateTime.Now;
                return DB.ShopBrowsingHistory.Insert(model);
            }
        }
        /// <summary>
        /// 获取指定商品的浏览人数
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        public int GetCount(int ProductID)
        {
            return DB.ShopBrowsingHistory.Where(q => q.ProductID == ProductID).Count();
        }
    }
}
