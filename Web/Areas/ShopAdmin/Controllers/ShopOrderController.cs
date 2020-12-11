using Business;
using Common;
using DataBase;
using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.ShopAdmin.Controllers
{
    /// <summary>
    /// 订单管理
    /// </summary>
    public class ShopOrderController : Web.Controllers.AdminBaseController
    {
        // GET: ShopAdmin/ShopOrder
        public ActionResult Index(int? id)
        {
            ViewBag.List = DB.ShopProductCategory.getFrom2Layer();
            ViewBag.ID = id;
            ViewBag.code = CurrentUser.LoginName;
            return View();
        }
        #region 查询
        public string getDataSource(int? id, int? category, DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            string usercode = CurrentUser.LoginName;

            var shopid =1;
            if (end != null) end = end.Value.AddDays(1);
            var query = DB.ShopOrder.Where(a =>
           a.ShopID == shopid
            && (string.IsNullOrEmpty(key) ? true :
            (a.NickName.Contains(key)
            || a.MemberCode.Contains(key)
            || a.OrderCode == key
            || a.OrderType == key
            || a.GUID == key
            || (a.ShopOrderProducts.Any(c => c.Name.Contains(key)
            ))))
               && (id == null ? true : a.State == id)
               //&& (category == null ? true : a.ShopOrderProducts.Select(b => b.ca) == category)
               && (startTime == null ? true : a.SubmitTime >= startTime)
               && (end == null ? true : a.SubmitTime < end))
                 .Select(a => new
                 {
                     a.GUID,
                     PayState = a.PayState == 1 ? "已支付" : "未支付",
                     a.MemberCode,
                     a.OrderCode,
                     a.OrderType,
                     a.PayWay,
                     a.PayTime,
                     a.State,
                     a.IsZiTi,
                     a.Tel,
                     ExpressID = a.ShopExpress.Name,
                     a.RealAmount,
                     a.Postage,
                     a.StoreCode,
                     AllCount = a.ShopOrderProducts.Select(q => q.Count).DefaultIfEmpty().Sum(),
                     a.ZongDay,
                     a.YiDay,
                     a.Type
                 });
            var total = query.Count();
            var list = query.OrderByDescending(a => a.PayTime).Skip(start).Take(length).ToList();

            return ToPage(list, total, start, length, draw);
        }
        #endregion

        #region 编辑视图
        public ActionResult Detail(string id)
        {
            //var order = DB.ShopOrder.Include("ShopOrderProducts").Include("ShopProduct").Where(a => a.GUID == id).FirstOrDefault();
            var order = DB.ShopOrder.Where(a => a.GUID == id).FirstOrDefault();
            //ViewBag.ProductList = DB.ShopOrderProduct.Where(a => a.OrderID == id).ToList();
            ViewBag.Express = DB.ShopExpress.Where().Select(a => new { a.Name, a.ID, a.Sort })
                .OrderBy(a => a.Sort).ToList().Select(a => new KeyValuePair<int, string>(a.ID, a.Name)).ToList();
            if (order == null) order = new ShopOrder() { ShopOrderProducts = new List<ShopOrderProduct>() };
            return View(order);
        }
        #endregion 

        #region 保存
        public ActionResult Save(ShopOrder entity)
        {
            var json = new JsonHelp();
            try
            {

                json.IsSuccess = DB.ShopOrder.Update(entity);
                json.Msg = "修改";

                if (json.IsSuccess)
                {
                    json.ReUrl = ControllerPath + "/Index";   //注册成功就跳转到 激活页
                    json.Msg += "成功";
                }
                else
                {
                    json.Msg += "失败";
                }
            }
            catch (Exception e)
            {
                json.IsSuccess = false;
                json.Msg = "操作失败";
                LogHelper.Error("保存快递公司失败：" + WebTools.getFinalException(e));
            }
            return Json(json);
        }
        #endregion

        #region 删除
        public JsonResult Delete(string idList)
        {
            JsonHelp json = new JsonHelp() { Msg = "删除数据失败" };
            //是否为空
            if (string.IsNullOrEmpty(idList))
            {
                json.Msg = "未找到要删除的数据";
                return Json(json);
            }
            var ids = idList.TrimEnd(',').Split(',').ToList();
            if (DB.ShopOrder.Any(a => ids.Contains(a.GUID) && a.State <= 0))
            {
                var names = DB.ShopOrder.Where(a => ids.Contains(a.GUID)).Select(a => a.OrderCode)
                    .ToList().Aggregate((m, n) => m + "," + n);
                if (DB.ShopOrder.Delete(a => ids.Contains(a.GUID)) > 0)
                {
                    json.IsSuccess = true;
                    json.Msg = "删除数据成功";
                }
                //添加操作日志
                DB.SysLogs.setMemberLog("Delete", "删除订单编号为[" + names + "]的商品订单 ");
            }
            else
            {
                json.Msg = "只能删除已取消或已关闭或已退货的订单，请确认";
            }
            return Json(json);
        }
        #endregion

        #region 取消订单
        public JsonResult Cancel(string idList)
        {
            JsonHelp json = new JsonHelp() { Msg = "取消订单数据失败" };
            //是否为空
            if (string.IsNullOrEmpty(idList))
            {
                json.Msg = "未找到要取消的订单";
                return Json(json);
            }
            var ids = idList.TrimEnd(',').Split(',').ToList();
            if (DB.ShopOrder.Any(a => ids.Contains(a.GUID) && (a.State == 0 || a.State == 1)))
            {
                var names = DB.ShopOrder.Where(a => ids.Contains(a.GUID)).Select(a => a.OrderCode)
                    .ToList().Aggregate((m, n) => m + "," + n);
                var cancelstate = ShopEnum.OrderState.Cancel.GetHashCode();
                var list = DB.ShopOrder.Where(a => ids.Contains(a.GUID)).ToList();
                foreach (var item in list)
                {
                    item.State = cancelstate;
                }
                if (DB.ShopOrder.Update(list) > 0)
                {
                    json.IsSuccess = true;
                    json.Msg = "取消订单成功";
                }
                //添加操作日志
                DB.SysLogs.setMemberLog("Delete", "取消订单编号为[" + names + "]的商品订单 ");
            }
            else
            {
                json.Msg = "只能取消未支付的订单，请确认";
            }
            return Json(json);
        }
        #endregion

        #region 挂卖
        public JsonResult GuaM(string idList)
        {
            JsonHelp json = new JsonHelp() { Msg = "挂卖数据失败" };
            //是否为空
            if (string.IsNullOrEmpty(idList))
            {
                json.Msg = "未找到要挂卖的订单";
                return Json(json);
            }
            var ids = idList.TrimEnd(',').Split(',').ToList();
            if (DB.ShopOrder.Any(a => ids.Contains(a.GUID) && (a.State == 40)))
            {
                var names = DB.ShopOrder.Where(a => ids.Contains(a.GUID)).Select(a => a.OrderCode)
                    .ToList().Aggregate((m, n) => m + "," + n);            
               // var cancelstate = ShopEnum.OrderState.GuaM.GetHashCode();
                var list = DB.ShopOrder.Where(a => ids.Contains(a.GUID)).ToList();
                foreach (var item in list)
                {
                    item.State = ShopEnum.OrderState.Finish.GetHashCode();
                    item.Type = "挂卖成功";
                    item.FinishTime = DateTime.Now;
                    #region 委托成功后给委托人奖励
                    var member = DB.Member_Info.FindEntity(p => p.MemberId == item.MemberID);
                    //DB.Jiang.InsertFin(member, member, 800, "挂卖奖励积分", "挂卖奖励积分");//积分
                    DB.Jiang.InsertFin(member, member, 2100, "委托奖励收益", "委托奖励收益");//收益
                    DB.Jiang.FenXiang(member);
                    #endregion
                    //item.State = cancelstate;
                    //item.Type = "挂卖中";
                    //item.SubmitTime = DateTime.Now;//挂卖时间
                    ////给零售区加库存
                    //var lingshouqu = DB.ShopOrderProduct.Where(p => p.OrderID == item.GUID).FirstOrDefault();
                    //var shopOrderProudct = DB.ShopProduct.Where(p => p.Title == lingshouqu.Name && p.CategoryID1 == 15).FirstOrDefault();
                    //shopOrderProudct.Inventory = Convert.ToInt32(shopOrderProudct.Inventory + item.ZongDay);
                    //DB.ShopProduct.Update(shopOrderProudct);
                }
                if (DB.ShopOrder.Update(list) > 0)
                {
                    json.IsSuccess = true;
                    json.Msg = "挂卖成功";
                }
                //添加操作日志
                DB.SysLogs.setMemberLog("Delete", "挂卖编号为[" + names + "]的商品订单 ");
            }
            else
            {
                json.Msg = "只能处理挂卖的订单，请确认";
            }
            return Json(json);
        }
        #endregion 

        #region 发货
        private static readonly object obj1 = new object();
        public ActionResult FaHuo(string id, int ExpressID, string ExpressCode)
        {
            var json = new JsonHelp();
            var order = DB.ShopOrder.FindEntity(id);
            if (order != null)
            {
                lock (obj1)
                {
                    if (order.State == ShopEnum.OrderState.FaHuo.GetHashCode())
                    {
                        json.Msg = "已发货，不能重复发货";
                        return Json(json);
                    }
                    using (var tran = DB.Member_Info.BeginTransaction)
                    {
                        order.State = ShopEnum.OrderState.FaHuo.GetHashCode();
                        order.ExpressCode = ExpressCode;
                        order.ExpressID = ExpressID;
                        order.PostTime = DateTime.Now;
                        order.OperateCode = CurrentUser.LoginName;

                        json.IsSuccess = DB.ShopOrder.Update(order);
                        if (json.IsSuccess)
                        {
                            //发放服务费
                            //json = DB.Jiang.Service(order);
                            if (json.IsSuccess == false)
                            {
                                json.Msg = "发货失败";
                                return Json(json);
                            }
                            tran.Complete();
                            json.Msg = "发货成功";
                            return Json(json);
                        }
                    }
                }

            }
            json.Msg = "发货失败";
            return Json(json);
        }

        public ActionResult ShouHuo(string id)
        {
            var json = new JsonHelp();
            json= DB.ShopOrder.ShouHuo(id);
          
            return Json(json);
        }

        //修改订单邮费
        public ActionResult UpdatePost(string id, decimal amount)
        {
            var json = new JsonHelp();
            var order = DB.ShopOrder.FindEntity(id);

            if (order != null)
            {
                order.RealCongXiao = amount;
                order.TraceNo = Guid.NewGuid().ToString().Replace("-", "");
                DB.ShopOrder.Update(order);

                json.IsSuccess = true;
                json.Msg = "修改成功";
                return Json(json);
            }
            json.Msg = "修改失败";
            return Json(json);
        }
        #endregion

        #region 修改收货人信息
        public ActionResult UpdateReceiver(string id, string Receiver, string Tel, string PostAddress, string Remark)
        {
            var json = new JsonHelp();
            var order = DB.ShopOrder.FindEntity(id);
            if (order != null)
            {
                order.Receiver = Receiver;
                order.Tel = Tel;
                order.PostAddress = PostAddress;
                order.Remark = Remark;
                json.IsSuccess = DB.ShopOrder.Update(order);
                if (json.IsSuccess)
                {
                    json.Msg = "修改收货人信息成功";
                    return Json(json);
                }
            }
            json.Msg = "修改失败";
            return Json(json);
        }
        #endregion



        #region 导出excel
        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="end"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult ToExcel(DateTime? startTime, DateTime? end, string key, int? state)
        {
            string usercode = CurrentUser.LoginName;

            var shopid = 1;
            if (end != null) end = end.Value.AddDays(1);
            var objList = DB.ShopOrder.Where(a =>
           a.ShopID == shopid
            && (string.IsNullOrEmpty(key) ? true :
            (a.NickName.Contains(key)
            || a.MemberCode.Contains(key)
            || a.OrderCode == key
            || a.OrderType == key
            || a.GUID == key
            || (a.ShopOrderProducts.Any(c => c.Name.Contains(key)
            ))))
               && (state == null ? true : a.State == state)
               //&& (category == null ? true : a.ShopOrderProducts.Select(b => b.ca) == category)
               && (startTime == null ? true : a.SubmitTime >= startTime)
               && (end == null ? true : a.SubmitTime < end))
                 .Select(a => new
                 {
                     a.GUID,
                     PayState = a.PayState == 1 ? "已支付" : "未支付",
                     a.MemberCode,
                     a.OrderCode,
                     a.OrderType,
                     a.PayWay,
                     a.PayTime,
                     a.State,
                     a.IsZiTi,
                     a.Tel,
                     ExpressID = a.ShopExpress.Name,
                     a.RealAmount,
                     a.Postage,
                     a.StoreCode,
                     AllCount = a.ShopOrderProducts.Select(q => q.Count).DefaultIfEmpty().Sum(),
                     a.ZongDay,
                     a.YiDay,
                     a.Type
                 }).ToList();
            if (objList.Count <= 0)
                return Content("导出列表为空");

            return base.ToExcel(objList);
        }

        public class ShopOrderExcel
        {
            public string GUID { get; set; }
            public string MemberCode { get; set; }

            //public string NickName { get; set; }

            public string Reciver { get; set; }
            public string PostAddress { get; set; }
            public string Tel { get; set; }
            public string Postage { get; set; }
            public string ShopName { get; set; }
            public string ProductName { get; set; }
            public int Count { get; set; }
            public DateTime SubmitTime { get; set; }


            public string PayWay { get; set; }


            public string PayAmount { get; set; }

            public string ExpressCode { get; set; }
            public string ExpressName { get; set; }

            public string StoreCode { get; set; }
            public string OperateCode { get; set; }
        }
        #endregion
    }
}