using Common;
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
    /// 商品分类，数据访问
    /// </summary>
    public class ShopProductCategoryImp : EFBase<DataBase.ShopProductCategory>
    {
        /// <summary>
        /// 获取前两层的 类别名与id
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<int, string>> getFrom2Layer()
        {
            var list = DB.ShopProductCategory.Where(a => a.Layer <= 2).Select(a => new { a.ID, a.Name, a.PID, a.Sort, a.Layer }).ToList();
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
        public List<KeyValuePair<int, string>> getFrom1Layer()
        {
            var list = DB.ShopProductCategory.Where(a => a.Layer <= 2).Select(a => new { a.ID, a.Name, a.PID, a.Sort, a.Layer }).ToList();
            var r = new List<KeyValuePair<int, string>>();
            var layer1 = list.Where(a => a.Layer == 1).OrderBy(a => a.Sort);
            foreach (var item in layer1)
            {
                r.Add(new KeyValuePair<int, string>(item.ID, item.Name));
            }
            return r;
        }
        public List<KeyValuePair<int, string>> getFrom8Layer()
        {
            var list = DB.ShopProductCategory.Where(a => a.Layer <= 2).Select(a => new { a.ID, a.Name, a.PID, a.Sort, a.Layer }).ToList();
            var r = new List<KeyValuePair<int, string>>();
            var layer1 = list.Where(a => a.Layer == 1 && a.ID != 8).OrderBy(a => a.Sort);
            foreach (var item in layer1)
            {
                r.Add(new KeyValuePair<int, string>(item.ID, item.Name));
            }
            return r;
        }
        public List<KeyValuePair<int, string>> getFrom23Layer(int pid)
        {
            var list = DB.ShopProductCategory.Where(a => a.Layer <= 3).Select(a => new { a.ID, a.Name, a.PID, a.Sort, a.Layer }).ToList();
            var r = new List<KeyValuePair<int, string>>();
            var layer2 = list.Where(a => a.Layer == 2 && a.PID == pid).OrderBy(a => a.Sort);
            foreach (var item in layer2)
            {
                r.Add(new KeyValuePair<int, string>(item.ID, item.Name));
                var layer3 = list.Where(a => a.PID == item.ID).OrderBy(a => a.Sort);
                foreach (var c in layer3)
                {
                    r.Add(new KeyValuePair<int, string>(c.ID, "-- " + c.Name));
                }
            }
            return r;
        }
        public List<KeyValuePair<int, string>> getFrom3Layer()
        {
            var list = DB.ShopProductCategory.Where(a => a.Layer <= 3).Select(a => new { a.ID, a.Name, a.PID, a.Sort, a.Layer }).ToList();
            var r = new List<KeyValuePair<int, string>>();
            var layer1 = list.Where(a => a.Layer == 1).OrderBy(a => a.Sort);
            foreach (var item in layer1)
            {
                r.Add(new KeyValuePair<int, string>(item.ID, item.Name));
                var layer2 = list.Where(a => a.PID == item.ID).OrderBy(a => a.Sort);
                foreach (var c in layer2)
                {
                    r.Add(new KeyValuePair<int, string>(c.ID, "-- " + c.Name));
                    var layer3 = list.Where(a => a.PID == c.ID).OrderBy(a => a.Sort);
                    foreach (var d in layer3)
                    {
                        r.Add(new KeyValuePair<int, string>(d.ID, "-- -- " + d.Name));
                    }
                }
            }
            return r;
        }
        /// <summary>
        /// 组合树(三级)
        /// </summary>
        /// <returns></returns>
        public string ConvertjsTreeData()
        {
            var list = DB.ShopProductCategory.Where(a => a.Layer <= 3).Select(a => new { a.ID, a.Name, a.PID, a.Sort, a.Layer }).ToList();
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


        public List<ShopProductCategory> GetListByParentID(int? ParentID)
        {
            var query = Where();
            if (ParentID != 0)
                query = query.Where(q => q.PID == ParentID.Value);
            query = query.OrderBy(q => q.Sort);
            return query.ToList();
        }
      
        public List<ShopProductCategory> GetListByParent(ShopProductCategory model)
        {
            if (model == null)
                return GetListByParentID(null);
            return GetListByParentID(model.ID);
        }
        /// <summary>
        /// 获取当前子节点两级的id列表,不包括自己
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<int> GetChildIDList(ShopProductCategory model)
        {
            List<int> list = new List<int>();
            List<ShopProductCategory> children = GetListByParent(model);
            foreach (var item in children)
            {
                list.Add(item.ID);

                List<ShopProductCategory> children2 = GetListByParent(item);
                foreach (var item2 in children2)
                {
                    list.Add(item2.ID);
                }
            }
            return list;
        }

    }
}
