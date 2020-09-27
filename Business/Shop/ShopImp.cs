using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class ShopImp :EFBase<DataBase.Shop>
    {
        /// <summary>
        /// 获取会员的商家id
        /// </summary>
        /// <param name="memberid"></param>
        /// <returns></returns>
        public int getIDByMemberID(string memberid)
        {
            var shopid = DB.Shop.Where(a => a.MemberID == memberid).Select(a => a.ID).ToList();
            if (shopid.Count > 0) return shopid[0];
            return 0;
        }
    }
}
