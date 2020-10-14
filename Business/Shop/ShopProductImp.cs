using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class ShopProductImp : EFBase<DataBase.ShopProduct>
    {

        public GuiGeProduct_Info GetSPrice(ShopProduct shopproduct, string guige)
        {
            var cateid = GetCategoryId2(shopproduct.CategoryID.Value);
            if (cateid != null )
            {
                var SPrice = DB.GuiGeProduct_Info.Where(a => a.ProductId == shopproduct.ID && a.SName == cateid.ID && a.SComment == guige).FirstOrDefault();
                return SPrice;
            }else
            {
                return new GuiGeProduct_Info();
            }
          
        }
        public decimal GetYiShou(ShopProduct shopproduct)
        {
            var m = DB.ShopProductCategory.FindEntity(shopproduct.CategoryID);
            while (m.Layer != 2 && m.PID != m.ID)
            {
                m = DB.ShopProductCategory.FindEntity(m.PID);

            }

            var yishou = DB.GuiGeProduct_Info.Where(a => a.ProductId == shopproduct.ID && a.SName == m.ID).Sum(a => (decimal?)a.YiShou) ?? 0;
            return yishou;
        }
        public decimal GetKuCun(ShopProduct shopproduct)
        {
            var m = DB.ShopProductCategory.FindEntity(shopproduct.CategoryID);
            while (m.Layer != 2 && m.PID != m.ID)
            {
                m = DB.ShopProductCategory.FindEntity(m.PID);

            }

            var KuCun = DB.GuiGeProduct_Info.Where(a => a.ProductId == shopproduct.ID && a.SName == m.ID).Sum(a => (decimal?)a.KuCun) ?? 0;
            return KuCun;
        }

        public decimal GetLingShouPrice(ShopProduct shopproduct)
        {
            var m = DB.ShopProductCategory.FindEntity(shopproduct.CategoryID);
            while (m.Layer != 2 && m.PID != m.ID)
            {
                m = DB.ShopProductCategory.FindEntity(m.PID);

            }
            var guigeproduct = DB.GuiGeProduct_Info.Where(a => a.ProductId == shopproduct.ID && a.SName == m.ID);

            var YPrice = 0m;
            if (guigeproduct != null)
            {
                YPrice = DB.GuiGeProduct_Info.Where(a => a.ProductId == shopproduct.ID && a.SName == m.ID).Min(a => (decimal?)a.LingShou) ?? 0;

            }
            return YPrice;
        }

        public decimal GetYouHuiPrice(ShopProduct shopproduct)
        {
            var m = DB.ShopProductCategory.FindEntity(shopproduct.CategoryID);
           
            while (m.Layer != 2 && m.PID!=m.ID)
            {
                m = DB.ShopProductCategory.FindEntity(m.PID);

            }
           var guigeproduct=  DB.GuiGeProduct_Info.Where(a => a.ProductId == shopproduct.ID && a.SName == m.ID);

            var SPrice = 0m;
            if (guigeproduct != null)
            {
                SPrice= DB.GuiGeProduct_Info.Where(a => a.ProductId == shopproduct.ID && a.SName == m.ID).Min(a => (decimal?)a.YouHui) ?? 0;
            }
            return SPrice;
        }
        public decimal GetPeiHuoPrice(ShopProduct shopproduct)
        {
            var m = DB.ShopProductCategory.FindEntity(shopproduct.CategoryID);
            while (m.Layer != 2 && m.PID != m.ID)
            {
                m = DB.ShopProductCategory.FindEntity(m.PID);

            }

            var SPrice = DB.GuiGeProduct_Info.Where(a => a.ProductId == shopproduct.ID && a.SName == m.ID).Min(a => (decimal?)a.PeiHuo) ?? 0;
            return SPrice;
        }
        public ShopProductCategory GetCategoryId2(int CategoryID)
        {
            var m = DB.ShopProductCategory.FindEntity(CategoryID);
            if (m != null)
            {
                while (m.Layer != 2 && m.PID != m.ID)
                {
                    m = DB.ShopProductCategory.FindEntity(m.PID);

                }
                return m;
            }
            else
            {
                return null;
            }
        }
        public List<ShopProduct> GetListByParent(int? id, int? ShopID = 0)
        {
            var query = Where();
            if (id != null && id != 0)
                query = query.Where(q => q.CategoryID == id.Value);
            if (ShopID != 0)
                query = query.Where(q => q.ShopID == ShopID.Value);
            query = query.OrderBy(q => q.Sort);
            return query.ToList();
        }
        #region 查询
        /// <summary>
        /// 订单里显示的价格
        /// </summary>
        /// <param name="model">产品</param>
        /// <param name="number">数量</param>
        /// <returns></returns>
        public string getProductPrice(ShopProduct model, int paytype, int number = 1)
        {
            StringBuilder sb = new StringBuilder();
            if (model.PriceVip > 0)
            {
                sb.AppendFormat("零售价：{0} 元<br/>", model.PriceVip * number);
            }

            //if (model.PriceShopping > 0)
            //{
            //    sb.AppendFormat("购物币：{0} <br/>", model.PriceShopping * number);
            //}
            //if (model.PriceCongXiao > 0)
            //{
            //    sb.AppendFormat("重消币：{0} <br/>", model.PriceCongXiao * number);
            //}
            //if (model.PriceScore > 0)
            //{
            //    sb.AppendFormat("积分：{0} <br/>", model.PriceScore * number);
            //}


            ////根据支付类型显示
            //if (paytype == ShopEnum.PayType.在线支付.GetHashCode())
            //{

            //}
            //else if (paytype == ShopEnum.PayType.电子币.GetHashCode())
            //{

            //}
            //else if (paytype == ShopEnum.PayType.现金加积分.GetHashCode())
            //{

            //}
            //else if (paytype == ShopEnum.PayType.购物币加积分.GetHashCode())
            //{

            //}
            return sb.ToString();
        }

        #endregion



        #region 订单评论等
        /// <summary>
        /// 获取指定商品的评论数量
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        public int GetCommentCount(int ProductID)
        {
            return DB.ShopOrderComment.Where(q => q.ProductID == ProductID)
                .Count();
        }
        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="ProductID"></param>
        /// <param name="SkipCount"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public List<ShopOrderComment> GetCommentList(int ProductID,
            out int totalCount,
            int SkipCount = 0, int PageSize = 10)
        {
            var query = DB.ShopOrderComment.Where(q => q.ProductID == ProductID);
            totalCount = query.Count();
            query = query.OrderByDescending(q => q.CreateTime);
            return query.Skip(SkipCount)
                .Take(PageSize)
                .ToList();
        }
        #endregion
    }
}
