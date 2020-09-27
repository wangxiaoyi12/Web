using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class ShopOneBuyDetailImp :EFBase<DataBase.ShopOneBuyDetail>
    {
        /// <summary>
        /// 获取指定会员的，商品投注数量
        /// </summary>
        /// <param name="MemberID"></param>
        /// <returns></returns>
        public int GetCount(string MemberID, int ID)
        {
            return DB.ShopOneBuyDetail.Where(q => q.MemberID == MemberID && q.OneBuyID == ID)
                 .Count();
        }
    }
}
