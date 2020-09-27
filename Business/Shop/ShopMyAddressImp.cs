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
    /// 用户收货地址操作
    /// </summary>
    public class ShopMyAddressImp :EFBase<DataBase.ShopMyAddress>
    {
        /// <summary>
        /// 设置默认地址
        /// </summary>
        /// <param name="ID"></param>
        public void SetDefault(int ID, string MemberID)
        {
            var list = DB.ShopMyAddress.Where(q => q.MemberID == MemberID).ToList();
            foreach (var item in list)
            {
                if (item.ID == ID)
                {
                    item.IsDefault = true;
                }
                else
                    item.IsDefault = false;
            }
            DB.ShopMyAddress.Update(list);
        }

        /// <summary>
        /// 获取指定会员的默认地址
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ShopMyAddress GetDefault(Member_Info model)
        {
            var query = DB.ShopMyAddress.Where(q => q.MemberID == model.MemberId);
            if (query.Any(q => q.IsDefault == true))
                query = query.Where(q => q.IsDefault == true);
            return query.FirstOrDefault();
        }
    }
}
