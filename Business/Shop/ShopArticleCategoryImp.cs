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
    /// 帮助中心分类，数据访问
    /// </summary>
    public class ShopArticleCategoryImp :EFBase<DataBase.ShopArticleCategory>
    {
        /// <summary>
        /// 获取父类下的 子节点列表
        /// </summary>
        /// <param name="ParentID">父类ID</param>
        /// <returns></returns>
        public List<ShopArticleCategory> GetListByParent(int? ParentID)
        {
            var query = Where();
            if (ParentID != 0)
                query = query.Where(q => q.PID == ParentID);
            query = query.OrderBy(q => q.Sort);
            return query.ToList();
        }

        /// <summary>
        /// 组合树(2级)
        /// </summary>
        /// <returns></returns>
        public string ConvertjsTreeData()
        {
            var list = DB.ShopArticleCategory.Where(a => a.Layer <= 3).Select(a => new { a.ID, a.Name, a.PID, a.Sort, a.Layer }).ToList();
            var r = new List<JSTree>();
            var layer1 = list.Where(a => a.Layer == 1).OrderBy(a => a.Sort);
            foreach (var item in layer1)
            {
                r.Add(new JSTree()
                {
                    id = item.ID.ToString(),
                    text = item.Name,
                    children = list.Where(a => a.PID == item.ID).OrderBy(a => a.Sort).Select(a =>
                        new JSTree()
                        {
                            id = a.ID.ToString(),
                            text = a.Name,
                            children = list.Where(b => b.PID == a.ID).OrderBy(b => b.Sort).Select(b =>
                                new JSTree()
                                {
                                    id = b.ID.ToString(),
                                    text = b.Name,
                                }).ToList()
                        }).ToList()
                });
            }
            return r.ToJsonString();
        }

        /// <summary>
        /// 获取前两层的 类别名与id
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<int, string>> getFrom2Layer()
        {
            var list = DB.ShopArticleCategory.Where(a => a.Layer <= 2).Select(a => new { a.ID, a.Name, a.PID, a.Sort, a.Layer }).ToList();
            var r = new List<KeyValuePair<int, string>>();
            var layer1 = list.Where(a => a.Layer == 1).OrderBy(a => a.Sort);
            foreach (var item in layer1)
            {
                r.Add(new KeyValuePair<int, string>(item.ID, item.Name));
                var childs = list.Where(a => a.PID == item.ID).OrderBy(a => a.Sort);
                foreach (var c in childs)
                {
                    r.Add(new KeyValuePair<int, string>(c.ID, "-- " + c.Name));
                }
            }
            return r;
        }
    }
}
