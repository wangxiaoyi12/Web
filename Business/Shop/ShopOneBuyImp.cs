using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    /// <summary>
    /// 一元购，数据访问
    /// </summary>
    public class ShopOneBuyImp :EFBase<DataBase.ShopOneBuy>
    {
        #region 随机发奖
        /// <summary>
        /// 根据购买记录,随机发奖
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonHelp RandomWin(int id)
        {
            var json = new JsonHelp();

            var details = DB.ShopOneBuyDetail.Where(a => a.OneBuyID == id).OrderBy(a => a.ID).Select(a => new { a.Count, a.MemberCode, a.ID });
            var sum = details.Sum(a => a.Count);  //总人次
            var random = DB.Random.Next(1, sum + 1);  //随机中奖数字
            var curSum = 0;
            foreach (var item in details)
            {
                curSum += item.Count;
                if (curSum >= random)
                {
                    json.Msg = item.MemberCode;
                    json.IsSuccess = true;
                    break;
                }
            }
            return json;
        }
        #endregion
    }
}
