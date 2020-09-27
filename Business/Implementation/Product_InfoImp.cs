using Common;
using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementation
{
    public class Product_InfoImp :EFBase<DataBase.Product_Info>
    {
        #region 查询  
        /// <summary>
        /// 分页获取产品信息
        /// </summary>
        /// <param name="key">搜索关键字</param>
        /// <param name="pageIndex">当前页数</param>
        /// <param name="pageSize">每页记录条数</param>
        /// <param name="totalCount">总记录条数</param>
        /// <returns></returns>
        public List<Product_Info> getDataSource(string key, int pageIndex, int pageSize, out int totalCount)
        {
            var query = Where(a => a.State == "在售");
            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(a => a.ProductName.Contains(key) || a.ModelName.Contains(key));
            }
            totalCount = query.Count();
            var pageCount = totalCount % pageSize == 0 ? totalCount / pageSize : totalCount / pageSize + 1;
            if (pageIndex > pageCount)
            {
                pageIndex = pageCount;
            }
            if (pageIndex < 1) pageIndex = 1;
            var data = query.OrderByDescending(p => p.CreateTime).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            return data;
        }
        #endregion

        #region 删除
        public JsonHelp Delete(string idList)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "删除数据失败" };
            try
            {
                //是否为空
                if (string.IsNullOrEmpty(idList)) { json.Msg = "未找到要删除的数据"; return json; }
                var id = idList.TrimEnd(',').Split(',').Select(a => Convert.ToInt32(a)).ToList();

                try
                {
                    if (Delete(a => id.Contains(a.ProductId)) > 0)
                    {
                        json.Status = "y";
                        json.Msg = "删除数据成功";
                        //添加操作日志
                        DB.SysLogs.setAdminLog("Delete", "删除 名称为【" + idList + "】的商品信息");
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("删除商品失败：" + ex.Message);
                }

                return json;
            }
            catch (Exception e) { throw e.InnerException; }
        }
        #endregion      

        #region 保存
        public JsonHelp Save(Product_Info entity)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "保存失败" };

            entity.CategoryName = DB.Sys_BasicData.FindEntity(p => p.Id == entity.CategoryId).BasicDataName;
            if (entity.ProductId == 0)
            {
                entity.CreateTime = DateTime.Now;
                if (Insert(entity))
                {
                    json.Status = "y";
                    json.Msg = "保存成功";
                    //添加操作日志
                    DB.SysLogs.setAdminLog("Add", "新建名称为[" + entity.ProductName + "]的商品");
                }
            }
            else
            {
                var m = DB.Product_Info.FindEntity(entity.ProductId);
                var type = DB.Sys_BasicData.FindEntity(a => a.Id == entity.CategoryId);
                m.CategoryId = entity.CategoryId;
                if (type != null)
                {
                    m.CategoryName = type.BasicDataName;
                }
                m.SalePrice = entity.SalePrice;
                m.Discount = entity.Discount;
                m.ImgUrl = entity.ImgUrl;
                m.ModelName = entity.ModelName;
                m.OriginalPrice = entity.OriginalPrice;
                m.ProductComment = entity.ProductComment;
                m.ProductName = entity.ProductName;
                m.PubTime = entity.PubTime;
                m.SaledQty = entity.SaledQty;
                m.StockQty = entity.StockQty;
                m.SortValue = entity.SortValue;
                m.State = entity.State;
                //db.AddDisableUpdateColumn("CreateTime", "CreateEmpId", "CreateEmpName", "PV");
                if (Update(m))
                {
                    json.Status = "y";
                    json.Msg = "保存成功";
                    //添加操作日志
                    DB.SysLogs.setAdminLog("Edit", "更新名称为[" + entity.ProductName + "]的商品");
                }
            }
            return json;
        }
        #endregion 

    }
}
