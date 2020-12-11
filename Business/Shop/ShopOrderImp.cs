using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Common;
using DataBase;
using Wechat;

namespace Business
{
    /// <summary>
    /// 订单管理，数据操作封装
    /// </summary>
    public class ShopOrderImp : EFBase<DataBase.ShopOrder>
    {
        #region 产品相关
        /// <summary>
        /// 获取指定会员的订单列表
        /// </summary>
        public List<ShopProduct> GetProductList(string MemberID, out int total, int? State = null,
            DateTime? start = null, DateTime? end = null,
            int? pageIndex = null, int? pageSize = null)
        {
            var query = DB.ShopOrderProduct.Where();
            if (string.IsNullOrEmpty(MemberID) == false)
                query = query.Where(q => q.MemberID == MemberID);
            if (State != null)
                query = query.Where(q => q.ShopOrder.State == State);
            if (start != null)
                query = query.Where(q => q.ShopOrder.SubmitTime >= start);
            if (end != null)
                query = query.Where(q => q.ShopOrder.SubmitTime < end);

            query = query.OrderByDescending(q => q.ID);
            if (pageIndex > 0)
            {
                query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }
            total = query.Count();

            List<int> ids = query.Select(q => q.ProductID).ToList();
            return DB.ShopProduct.Where(q => ids.Contains(q.ID)).ToList();
        }

        /// <summary>
        /// 获取指定会员的订单列表
        /// </summary>
        public List<ShopOrderProduct> GetOrderProductList(string MemberID, out int total, int? State = null,
            DateTime? start = null, DateTime? end = null,
            int? pageIndex = null, int? pageSize = null)
        {
            var query = DB.ShopOrderProduct.Where();
            if (string.IsNullOrEmpty(MemberID) == false)
                query = query.Where(q => q.MemberID == MemberID);
            if (State != null)
                query = query.Where(q => q.ShopOrder.State == State);
            if (start != null)
                query = query.Where(q => q.ShopOrder.SubmitTime >= start);
            if (end != null)
                query = query.Where(q => q.ShopOrder.SubmitTime < end);

            query = query.OrderByDescending(q => q.ID);
            if (pageIndex > 0)
            {
                query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }
            total = query.Count();
            return query.ToList();
        }

        /// <summary>
        /// 购物结算处理
        /// </summary>
        /// <param name="AddressID"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool Calcute(Member_Info curUser, string orderid, string payPwd)
        {
            using (var tran = BeginTransaction)
            {
                try
                {
                    if (curUser.Pwd2 != Common.CryptHelper.DESCrypt.Encrypt(payPwd))
                    {
                        throw new Exception("支付密码不正确");
                    }
                    Xml_Shop shopConfig = DB.XmlConfig.XmlShop;
                    int state = ShopEnum.OrderState.Submit.GetHashCode();
                    var query = DB.ShopOrder.Where(q => q.MemberID == curUser.MemberId && q.State == state);
                    if (string.IsNullOrEmpty(orderid) == false)
                        query = query.Where(q => q.GUID == orderid);
                    List<ShopOrder> orderList = query.ToList();
                    foreach (var order in orderList)
                    {
                        if (order.State != 1)
                        {
                            throw new Exception("订单不是待支付订单不可支付");
                        }
                        Fin_LiuShui _liushui = new Fin_LiuShui();
                        //1.修改订单状态
                        order.PayState = ShopEnum.OrderPayState.Pay.GetHashCode();
                        order.PayTime = DateTime.Now;
                        order.State = ShopEnum.OrderState.Pay.GetHashCode();
                        if (DB.ShopOrder.Update(order) == false)
                            throw new Exception("修改订单失败");


                        decimal zong = (order.RealAmount + order.Postage.Value);

                        if (curUser.Commission < zong)
                        {
                            throw new Exception("余额不足");
                        }
                        curUser.Commission = Convert.ToDecimal(curUser.Commission - zong);
                        DB.Fin_LiuShui.AddLS(curUser.MemberId, -zong, "商城下单");


                        if (DB.Member_Info.Update(curUser) == false)
                            throw new Exception("修改会员信息失败");


                        DB.Jiang.GiveJiang(DB.Member_Info.FindEntity(order.MemberID), order);


                    }
                    tran.Complete();
                    return true;
                }
                catch (Exception ex)
                {
                    DB.Rollback();
                    throw ex;
                }
            }

        }
        /// <summary>
        /// 生成订单，进入代付款状态
        /// </summary>
        /// <returns></returns>
        public bool GenerateOrder(Member_Info curUser, int AddressID, string remark)
        {
            //1.获取购物车
            List<ShopCat> catList = DB.ShopCat.Where(q => q.MemberID == curUser.MemberId).ToList();
            var member = DB.Member_Info.Where(p => p.MemberId == curUser.MemberId).FirstOrDefault();
            var Level = DB.Sys_Level.Where(p => p.LevelId == member.MemberLevelId).FirstOrDefault();

            using (var tran = BeginTransaction)
            {
                try
                {

                    //获取地址
                    ShopMyAddress address = DB.ShopMyAddress.FindEntity(q => q.ID == AddressID);

                    //2. 生成订单
                    ShopOrder order = new ShopOrder();
                    string orderID = Guid.NewGuid().ToString().Replace("-", "");
                    order.GUID = orderID;
                    order.ShopID = 1;
                    order.TraceNo = orderID;
                    order.OrderCode = generateCode();
                    order.MemberID = curUser.MemberId;
                    order.MemberCode = curUser.Code;
                    order.NickName = curUser.NickName;
                    order.PayState = ShopEnum.OrderPayState.WaitPay.GetHashCode();
                    order.State = ShopEnum.OrderState.Submit.GetHashCode();
                    order.SubmitTime = DateTime.Now;
                    order.Remark = remark;

                    if (address != null)
                    {
                        order.Receiver = address.Name;
                        order.PostAddress = address.ShowAddressName;
                        order.Tel = address.Tel;
                        order.Type = address.ShowAddressId;
                    }
                    foreach (var item in catList)
                    {
                        //if (member.MemberLevelId < 1)
                        //{
                        //    if (item.ShopProduct.CategoryID1 != 8)
                        //        throw new Exception("您不是会员无法去购买其他专区");
                        //}

                        int shopID = item.ShopProduct.ShopID;

                        order.PayWay = "";
                        //if (catList.Count > 1 && item.ShopProduct.IsDingDan())
                        //    throw new Exception("订单产品必须单独结算，请移除后提交");

                        //decimal totalPrice = 0;
                        decimal totalShopping = 0;
                        decimal totalScore = 0;
                        decimal totalCongXiao = 0;
                        //3.统计价格
                        var catItem = item;


                        totalShopping += DB.ShopProduct.GetSPrice(catItem.ShopProduct, catItem.GuiGe).LingShou.Value * catItem.Count; //零售价
                        totalScore += DB.ShopProduct.GetSPrice(catItem.ShopProduct, catItem.GuiGe).YouHui.Value * catItem.Count; //优惠价
                        totalCongXiao += DB.ShopProduct.GetSPrice(catItem.ShopProduct, catItem.GuiGe).PeiHuo.Value * catItem.Count; //配货价

                        order.ShopID = shopID;
                        order.Postage = 0;
                        order.RealAmount += totalScore;//优惠价
                        if (catItem.ShopProduct.CategoryID1 == DB.XmlConfig.XmlSite.Scores)
                        {


                            order.OrderType = "团购订单";
                        }
                        else
                        {

                            order.OrderType = "报单订单";
                        }
                        order.StoreCode = "未发放";
                        order.RealShopping += totalScore;//零售价
                        order.RealScore += totalScore;//优惠价

                        order.Postage += catItem.ShopProduct.Postage;

                        order.RealCongXiao += totalCongXiao;//pv
                        order.ZongDay = catItem.Count;//挂卖总数量
                        order.YiDay = catItem.Count;//剩余数量
                        order.Count += catItem.Count;


                    }
                    if (order.RealAmount >= DB.XmlConfig.XmlSite.MinAmountHuZ)
                    {
                        order.Postage = 0;
                    }

                    if (DB.ShopOrder.Insert(order) == false)
                        throw new Exception("添加订单失败");

                    //生成订单关联
                    foreach (var item in catList)
                    {
                        var catItem = item;
                        var gg = DB.ShopProduct.GetSPrice(catItem.ShopProduct, catItem.GuiGe);

                        ShopOrderProduct orderProduct = new ShopOrderProduct();
                        orderProduct.OrderID = orderID;
                        orderProduct.ProductID = catItem.ProductID;
                        orderProduct.Name = catItem.ShopProduct.Title;
                        orderProduct.Count = catItem.Count;
                        orderProduct.PriceShopping = gg.LingShou.Value; //零售
                        orderProduct.PriceScore = gg.YouHui.Value;               //优惠
                        orderProduct.PriceCongXiao = gg.PeiHuo.Value;    //pv

                        orderProduct.MoneyShopping = orderProduct.PriceShopping * catItem.Count;
                        orderProduct.MoneyScore = orderProduct.PriceScore * catItem.Count;
                        orderProduct.MoneyCongXiao = orderProduct.PriceCongXiao * catItem.Count;

                        orderProduct.Price = gg.YouHui.Value;

                        orderProduct.Money += orderProduct.MoneyScore;//配货价

                        orderProduct.GuiGe = catItem.GuiGe;
                        DB.ShopOrderProduct.Insert(orderProduct);

                        ////扣除订单的销售量
                        //catItem.ShopProduct.Sales += orderProduct.Count;
                        //catItem.ShopProduct.Inventory -= orderProduct.Count;
                        //if (catItem.ShopProduct.Inventory < 0)
                        //    throw new Exception("库存不足");
                        //DB.ShopProduct.Update(catItem.ShopProduct);

                        gg.KuCun -= orderProduct.Count;
                        gg.YiShou += orderProduct.Count;
                        DB.GuiGeProduct_Info.Update(gg);
                    }



                    //4.清空购物车
                    DB.ShopCat.Delete(catList);
                    tran.Complete();
                    return true;
                }
                catch (Exception ex)
                {
                    DB.Rollback();
                    throw ex;
                }
            }
        }
        private static readonly object obj1 = new object();
        public JsonHelp ShouHuo(string id)
        {
            var json = new JsonHelp();
            var order = DB.ShopOrder.FindEntity(id);
            if (order != null)
            {
                lock (obj1)
                {
                    if (order.State == ShopEnum.OrderState.Finish.GetHashCode())
                    {
                        json.Msg = "确认收货，不能重复操作";
                        return json;
                    }
                    using (var tran = DB.Member_Info.BeginTransaction)
                    {
                        order.State = ShopEnum.OrderState.Finish.GetHashCode();
                     
                        order.FinishTime = DateTime.Now;
                      

                        json.IsSuccess = DB.ShopOrder.Update(order);
                        if (json.IsSuccess)
                        {
                            //发放服务费
                            //json = DB.Jiang.Service(order);
                            if (json.IsSuccess == false)
                            {
                                json.Msg = "收货失败";
                                return json;
                            }
                            tran.Complete();
                            json.Msg = "收货成功";
                            return json;
                        }
                    }
                }

            }
            json.Msg = "收货失败";
            return json;
        }

        /// <summary>
        /// 产生订单编号
        /// </summary>
        /// <returns></returns>
        private string generateCode()
        {
            string timestr = DateTime.Now.ToString("yyyyMMddHHmmssms");
            return timestr + new Random().Next(10);
        }
        /// 修改订单信息
        /// </summary>
        public void UpdateOrderInfo(List<string> orderList, int AddressID, string remark, bool isziti, string paytype)
        {
            //获取地址
            ShopMyAddress address = DB.ShopMyAddress.FindEntity(q => q.ID == AddressID);
            if (address == null)
                throw new Exception("获取收货地址失败");
            foreach (var item in orderList)
            {
                ShopOrder orderModel = DB.ShopOrder.FindEntity(item);
                if (orderModel != null)
                {
                    orderModel.Receiver = address.Name;
                    orderModel.PostAddress = address.ShowAddressName;
                    orderModel.Type = address.ShowAddressId;

                    orderModel.Tel = address.Tel;
                    orderModel.Remark = remark;
                    orderModel.IsZiTi = isziti;
                    orderModel.PayWay = paytype;
                    //if (orderModel.IsZiTi && orderModel.RealCongXiao > 0)
                    //{
                    //    orderModel.RealCongXiao = 0;
                    //}

                    DB.ShopOrder.Update(orderModel);
                }
            }
        }




        #region 删除
        ///// <summary>
        ///// 生成订单3，返回订单的id列表,分消费券和积分扣款处理
        ///// </summary>
        ///// <returns></returns>
        //public List<string> GenerateOrder3(Member_Info curUser, Dictionary<int, int> proList, string TraceNo)
        //{
        //    List<string> orderList = new List<string>();
        //    using (var tran = BeginTransaction)
        //    {
        //        try
        //        {
        //            List<int> proIDList = proList.Select(q => q.Key).ToList();
        //            List<ShopProduct> productList = DB.ShopProduct.Where(q => proIDList.Contains(q.ID))
        //                .ToList();

        //            ////1.统计价格，判断用户余额
        //            //decimal totalShopping1 = 0;//消费券
        //            //decimal totalScores1 = 0;//总积分
        //            //foreach (var proItem in productList)
        //            //{
        //            //    if (proItem.IsScores)
        //            //    {
        //            //        totalScores1 += proItem.PriceScore * proList.Where(q => q.Key == proItem.ID).First().Value;
        //            //    }
        //            //    else
        //            //    {
        //            //        totalShopping1 += proItem.PriceShopping * proList.Where(q => q.Key == proItem.ID).First().Value;
        //            //    }
        //            //}
        //            //if (curUser.ReConsumeCoins < totalShopping1)
        //            //    throw new Exception("消费券余额不足");
        //            //if (curUser.Scores < totalScores1)
        //            //    throw new Exception("推广奖余额不足");

        //            ////扣除用户账户
        //            //curUser.ReConsumeCoins -= totalShopping1;
        //            //curUser.Scores -= totalScores1;
        //            //DB.Member_Info.Update(curUser);



        //            //根据商家分别生成订单
        //            List<int> shopIDList = productList.Select(q => q.ShopID)
        //                .Distinct()
        //                .ToList();
        //            foreach (var shopID in shopIDList)
        //            {
        //                string orderID = Guid.NewGuid().ToString();
        //                orderList.Add(orderID);

        //                //获取对应单的总消费
        //                decimal totalShopping = 0;//消费券
        //                decimal totalScores = 0;//总积分
        //                //foreach (var proItem in productList)
        //                //{
        //                //    if (proItem.IsScores)
        //                //    {
        //                //        totalScores += proItem.PriceScore * proList.Where(q => q.Key == proItem.ID).First().Value;
        //                //    }
        //                //    else
        //                //    {
        //                //        totalShopping += proItem.PriceShopping * proList.Where(q => q.Key == proItem.ID).First().Value;
        //                //    }
        //                //}


        //                //2. 生成订单
        //                ShopOrder order = new ShopOrder();
        //                order.GUID = orderID;
        //                order.OrderCode = generateCode();
        //                order.TraceNo = TraceNo;
        //                order.MemberID = curUser.MemberId;
        //                order.MemberCode = curUser.Code;
        //                order.NickName = curUser.NickName;
        //                order.PayState = ShopEnum.OrderPayState.WaitPay.GetHashCode();
        //                order.State = ShopEnum.OrderState.Submit.GetHashCode();
        //                order.SubmitTime = DateTime.Now;
        //                order.ShopID = shopID;

        //                order.RealAmount = totalShopping + totalScores;
        //                order.RealShopping = totalShopping;
        //                order.RealScore = totalScores;

        //                if (DB.ShopOrder.Insert(order) == false)
        //                    throw new Exception("添加订单失败");

        //                //4.添加订单商品关联
        //                foreach (var proItem in productList.Where(q => q.ShopID == shopID))
        //                {
        //                    //购买数量
        //                    int count = proList.Where(q => q.Key == proItem.ID).First().Value;
        //                    ShopOrderProduct orderProduct = new ShopOrderProduct();
        //                    orderProduct.OrderID = orderID;
        //                    orderProduct.ProductID = proItem.ID;
        //                    orderProduct.Name = proItem.Title;
        //                    orderProduct.Count = count;
        //                    orderProduct.Price = proItem.PriceVip;
        //                    orderProduct.PriceShopping = proItem.PriceShopping;

        //                    orderProduct.Money = proItem.PriceShopping * count;
        //                    orderProduct.MoneyShopping += orderProduct.PriceShopping * count;

        //                    DB.ShopOrderProduct.Insert(orderProduct);
        //                    //扣除订单的销售量
        //                    proItem.Inventory -= orderProduct.Count;
        //                    proItem.Sales += orderProduct.Count;
        //                    DB.ShopProduct.Update(proItem);
        //                }
        //            }
        //            //4.清空当前用户购物车
        //            DB.ShopCat.Delete(q => q.MemberID == curUser.MemberId && proIDList.Contains(q.ProductID));

        //            tran.Complete();
        //            return orderList;
        //        }
        //        catch (Exception ex)
        //        {
        //            DB.Rollback();
        //            throw ex;
        //        }
        //    }
        //}


        ///// <summary>
        ///// 支付完成，订单进入待发货状态
        ///// </summary>
        ///// <param name="orderidlist"></param>
        ///// <returns></returns>
        //public void PayComplete(Member_Info curUser, string tranno, decimal amount)
        //{
        //    LogHelper.Debug("支付完成：tranno:" + tranno);
        //    //获取单号的订单
        //    List<ShopOrder> orderList = DB.ShopOrder.Where(q => q.TraceNo == tranno).ToList();
        //    if (orderList.Count == 0) return;

        //    List<string> orderIDList = orderList.Select(q => q.GUID).ToList();
        //}


        ///// <summary>
        ///// 生成订单，进入代付款状态
        ///// </summary>
        ///// <returns></returns>
        //public bool GenerateOrder(Member_Info curUser, int AddressID, string remark)
        //{
        //    //1.获取购物车
        //    List<ShopCat> catList = DB.ShopCat.Where(q => q.MemberID == curUser.MemberId).ToList();
        //    using (var tran = BeginTransaction)
        //    {
        //        try
        //        {
        //            //获取地址
        //            ShopMyAddress address = DB.ShopMyAddress.FindEntity(q => q.ID == AddressID);
        //            if (address == null)
        //                throw new Exception("获取收货地址失败");


        //            //根据商家分别生成订单
        //            List<int> shopIDList = catList.Select(q => q.ShopProduct.ShopID)
        //                .Distinct()
        //                .ToList();
        //            foreach (var shopID in shopIDList)
        //            {
        //                string orderID = Guid.NewGuid().ToString();
        //                decimal totalPrice = 0;
        //                decimal totalShopping = 0;
        //                decimal totalScore = 0;
        //                decimal totalCongXiao = 0;
        //                //3.统计价格
        //                foreach (var catItem in catList.Where(q => q.ShopProduct.ShopID == shopID))
        //                {
        //                    totalPrice += curUser.GetProductPrice(catItem.ShopProduct) * catItem.Count;
        //                    totalShopping += catItem.ShopProduct.PriceShopping * catItem.Count;
        //                    totalScore += catItem.ShopProduct.PriceScore * catItem.Count;
        //                    totalCongXiao += catItem.ShopProduct.PriceCongXiao * catItem.Count;
        //                }

        //                //2. 生成订单
        //                ShopOrder order = new ShopOrder();
        //                order.GUID = orderID;
        //                order.OrderCode = generateCode();
        //                order.MemberID = curUser.MemberId;
        //                order.MemberCode = curUser.Code;
        //                order.NickName = curUser.NickName;
        //                order.PayState = ShopEnum.OrderPayState.WaitPay.GetHashCode();
        //                order.State = ShopEnum.OrderState.Submit.GetHashCode();
        //                order.SubmitTime = DateTime.Now;
        //                order.ShopID = shopID;

        //                order.Receiver = address.Name;
        //                order.PostAddress = address.ShowAddressName;
        //                order.Tel = address.Tel;
        //                order.Remark = remark;

        //                order.RealAmount = totalPrice;
        //                order.RealShopping = totalShopping;
        //                order.RealScore = totalScore;
        //                order.RealCongXiao = totalCongXiao;


        //                if (DB.ShopOrder.Insert(order) == false)
        //                    throw new Exception("添加订单失败");

        //                //4.添加订单商品关联
        //                foreach (var catItem in catList.Where(q => q.ShopProduct.ShopID == shopID))
        //                {
        //                    ShopOrderProduct orderProduct = new ShopOrderProduct();
        //                    orderProduct.OrderID = orderID;
        //                    orderProduct.ProductID = catItem.ProductID;
        //                    orderProduct.Name = catItem.ShopProduct.Title;
        //                    orderProduct.Count = catItem.Count;
        //                    orderProduct.Price = catItem.ShopProduct.PriceVip;
        //                    orderProduct.PriceShopping = catItem.ShopProduct.PriceShopping;
        //                    orderProduct.PriceScore = catItem.ShopProduct.PriceScore;
        //                    orderProduct.PriceCongXiao = catItem.ShopProduct.PriceCongXiao;

        //                    orderProduct.Money = curUser.GetProductPrice(catItem.ShopProduct) * catItem.Count;
        //                    orderProduct.MoneyShopping += orderProduct.PriceShopping * catItem.Count;
        //                    orderProduct.MoneyScore += orderProduct.PriceScore * catItem.Count;
        //                    orderProduct.MoneyCongXiao += orderProduct.PriceCongXiao * catItem.Count;

        //                    DB.ShopOrderProduct.Insert(orderProduct);
        //                    //扣除订单的销售量
        //                    catItem.ShopProduct.Sales -= orderProduct.Count;
        //                    DB.ShopProduct.Update(catItem.ShopProduct);
        //                }

        //            }
        //            //4.清空购物车
        //            DB.ShopCat.Delete(catList);
        //            tran.Complete();
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            DB.Rollback();
        //            throw ex;
        //        }
        //    }
        //}




        //private decimal getTotalAmountByCat(List<ShopCat> list, ShopEnum.CoinType type, Member_Info curUser)
        //{
        //    decimal result = 0;
        //    foreach (var item in list)
        //    {
        //        if (type == ShopEnum.CoinType.购物币)
        //        {
        //            if (curUser.IsVip())
        //                result += item.ShopProduct.PriceVip ?? 0 * item.Count;
        //            else
        //                result += item.ShopProduct.PriceShopping ?? 0 * item.Count;
        //        }
        //        else if (type == ShopEnum.CoinType.积分)
        //        {
        //            result += item.ShopProduct.PriceScore ?? 0 * item.Count;
        //        }
        //        else if (type == ShopEnum.CoinType.重销币)
        //        {
        //            result += item.ShopProduct.PriceCongXiao ?? 0 * item.Count;
        //        }
        //    }
        //    return result;
        //}
        #endregion
        #endregion

        #region 获取数量
        /// <summary>
        /// 获取指定状态的订单数量
        /// </summary>
        /// <param name="MemberID"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public int GetCount(string MemberID, int? state = null)
        {
            var query = DB.ShopOrder.Where(q => q.MemberID == MemberID);
            if (state != null)
                query = query.Where(q => q.State == state);
            return query.Count();
        }
        #endregion



        #region 查询订单总价格
        /// <summary>
        /// 获取订单的总金额显示字段
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string getTotalMoney(ShopOrder model)
        {
            if (model == null) return string.Empty;
            StringBuilder sb = new StringBuilder();
            Xml_Shop shopConfig = DB.XmlConfig.XmlShop;
            sb.AppendFormat("{0}", model.RealShopping);
            //sb.AppendFormat("积分：{0}   ", model.RealScore);
            return sb.ToString();
        }
        public string getTotalJiF(ShopOrder model)
        {
            if (model == null) return string.Empty;
            StringBuilder sb = new StringBuilder();
            Xml_Shop shopConfig = DB.XmlConfig.XmlShop;
            //sb.AppendFormat("{0}", model.RealShopping);
            sb.AppendFormat("积分：{0}   ", model.RealScore);
            return sb.ToString();
        }
        #endregion
    }
}
