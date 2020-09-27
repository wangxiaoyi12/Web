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
    /// 帮助中心文章表，数据访问
    /// </summary>
    public class ShopArticleImp :EFBase<DataBase.ShopArticle>
    {
        /// <summary>
        /// 浏览量++
        /// </summary>
        /// <param name="ID">文章主键ID</param>
        /// <returns></returns>
        public int ClicksAdd(int ID)
        {
            ShopArticle model = DB.ShopArticle.FindEntity(q => q.ID == ID);
            if (model != null)
            {
                model.Clicks++;
                DB.ShopArticle.Update(model);
                return model.Clicks;
            }
            return 0;
        }



    }
}
