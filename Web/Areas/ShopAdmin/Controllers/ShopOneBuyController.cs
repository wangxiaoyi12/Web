using Business;
using Common;
using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.ShopAdmin.Controllers
{
    /// <summary>
    /// 一元云购管理
    /// </summary>
    public class ShopOneBuyController : Web.Controllers.MemberBaseController
    {
        // GET: ShopAdmin/ShopOneBuy
        public ActionResult Index(int? id)
        {
            ViewBag.ID = id;
            return View();
        }
        #region 查询
        public string getDataSource(int? id, string key, int start, int length, int draw)
        {
            var query = DB.ShopOneBuy.Where(a => (id == null ? true : a.State == id)
                && (string.IsNullOrEmpty(key) ? true : a.ShopProduct.Title.Contains(key)))
                 .Select(a => new
                 {
                     a.ID,
                     IsEnable = a.IsEnable == true ? "启用" : "停用",
                     IsFinish = a.IsFinish == true ? "完成" : "进行中",
                     a.Code,
                     a.Count,
                     ProductID = a.ShopProduct.Title,
                     a.State,
                     a.CreateTime,
                     a.FinishTime,
                     a.MemberCode,
                     a.NickName,
                     a.ShopPayWay.PayWay,
                     a.TotalCount,
                     a.WinTime,
                 });
            var total = query.Count();
            var list = query.OrderByDescending(a => a.ID).Skip(start).Take(length).ToList();

            return ToPage(list, total, start, length, draw);
        }
        #endregion

        #region 编辑视图
        public ActionResult Detail(int? id, int? pid)
        {
            ShopOneBuy entity = null;
            ViewBag.List = DB.ShopPayWay.Where(a => true).OrderBy(a => a.ID).Select(a => new { a.ID, a.PayWay })
                .ToList().Select(a => new KeyValuePair<int, string>(a.ID, a.PayWay)).ToList();
            if (id == null)
            {
                var ShopProduct = DB.ShopProduct.FindEntity(pid);
                if (ShopProduct != null)
                {
                    ViewBag.PName = ShopProduct.Title;
                }
                var qihao = DB.ShopOneBuy.Any() ? DB.ShopOneBuy.Where().Max(a => a.ID) : 0;
                entity = new ShopOneBuy() { Code = "Y" + 1000000 + qihao, IsEnable = true, CreateTime = DateTime.Now, IsFinish = false, ProductID = pid.Value };
            }
            else
            {
                entity = DB.ShopOneBuy.FindEntity(id);
                var ShopProduct = DB.ShopProduct.FindEntity(entity.ProductID);
                if (ShopProduct != null)
                {
                    ViewBag.PName = ShopProduct.Title;
                }
            }

            return View(entity);
        }
        #endregion

        #region 保存
        public ActionResult Save(ShopOneBuy entity)
        {
            var json = new JsonHelp();
            try
            {
                if (entity.ID == 0)
                {
                    json.IsSuccess = DB.ShopOneBuy.Insert(entity);
                    json.Msg = "添加";
                }
                else
                {
                    var model = DB.ShopOneBuy.FindEntity(entity.ID);

                    #region 赋值
                    model.TotalCount = entity.TotalCount;
                    model.PayWay = entity.PayWay;
                    model.CreateTime = entity.CreateTime;
                    model.IsEnable = entity.IsEnable;
                    model.MemberCode = entity.MemberCode;
                    #endregion

                    #region 检验
                    if (model.TotalCount < model.Count)
                    {
                        json.Msg = "总需人次不能小于已参与人次！";
                        return Json(json);
                    }
                    if (model.IsEnable == false && model.Count > 0)
                    {
                        json.Msg = "一元购正在进行中，不能停用";
                        return Json(json);
                    }
                    if (model.State == ShopEnum.OneBuyState.End.GetHashCode())
                    {
                        json.Msg = "一元购已结束，不能修改";
                        return Json(json);
                    }
                    #endregion

                    using (var tran = DB.ShopOneBuy.BeginTransaction)
                    {
                        if (model.Count > 0)
                        {
                            if (model.State == ShopEnum.OneBuyState.Start.GetHashCode())
                            {
                                //当总需人次与已参与人次相同时
                                if (model.Count >= model.TotalCount)
                                {
                                    model.IsFinish = true;
                                    model.FinishTime = DateTime.Now;
                                    model.State = ShopEnum.OneBuyState.Finish.GetHashCode();
                                }
                            }
                            else
                            {
                                var member = DB.Member_Info.FindEntity(a => a.Code == entity.MemberCode);
                                if (member != null)
                                {
                                    //设置获奖人                                    
                                    model.NickName = member.NickName;
                                    model.Winner = member.MemberId;
                                    model.WinTime = DateTime.Now;
                                    model.State = ShopEnum.OneBuyState.End.GetHashCode();

                                    #region 添加中奖订单 
                                    var product = DB.ShopProduct.FindEntity(a => a.ID == model.ProductID);
                                    var order = new ShopOrder()
                                    {
                                        GUID = Guid.NewGuid().ToString().Replace("-",""),
                                        MemberCode = member.Code,
                                        NickName = member.NickName,
                                        MemberID = member.MemberId,
                                        OrderCode = model.Code + DB.Random.Next(100, 999),
                                        OrderType = "一元购",
                                        PayState = ShopEnum.OrderPayState.Pay.GetHashCode(),
                                        PayTime = DateTime.Now,
                                        SubmitTime = DateTime.Now,
                                        RealAmount = model.TotalCount,
                                        ShopID = product.ShopID,
                                        State = ShopEnum.OrderState.Pay.GetHashCode(),
                                        TotalAmount = model.TotalCount
                                    };
                                    if (model.PayWay != null)
                                    {
                                        order.PayWay = DB.ShopPayWay.FindEntity(model.PayWay).PayWay;
                                    }
                                    else
                                    {
                                        order.PayWay = "电子币";
                                    }
                                    //默认的我的地址
                                    var myaddress = DB.ShopMyAddress.Where(a => a.MemberID == member.MemberId).OrderByDescending(a => a.IsDefault).FirstOrDefault();
                                    if (myaddress != null)
                                    {
                                        order.Receiver = myaddress.Name;
                                        order.PostAddress = myaddress.Address;
                                        order.Tel = myaddress.Tel;
                                    }
                                    model.OrderID = order.GUID;
                                    json.IsSuccess = DB.ShopOrder.Insert(order);
                                    if (json.IsSuccess == false)
                                    {
                                        json.Msg = "生成订单失败，请重试";
                                        return Json(json);
                                    }
                                    #region 添加 商品订单对应 
                                    var orderproduct = new ShopOrderProduct()
                                    {
                                        Count = 1,
                                        MemberID = member.MemberId,
                                        Money = model.TotalCount,
                                        Name = product.Title,
                                        ProductID = model.ProductID,
                                        Price = model.TotalCount,
                                        OrderID = order.GUID
                                    };
                                    json.IsSuccess = DB.ShopOrderProduct.Insert(orderproduct);
                                    if (json.IsSuccess == false)
                                    {
                                        json.Msg = "生成订单失败，请重试";
                                        return Json(json);
                                    }
                                    #endregion
                                    #endregion
                                }
                            }
                        }
                        json.IsSuccess = DB.ShopOneBuy.Update(model);
                        json.Msg = "修改";
                        tran.Complete();
                    }
                }
                if (json.IsSuccess)
                {
                    json.ReUrl = ControllerPath + "/Index";
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
                LogHelper.Error("保存一元购失败：" + WebTools.getFinalException(e));
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
            var ids = idList.TrimEnd(',').Split(',').Select(a => Convert.ToInt32(a)).ToList();
            if (DB.ShopOneBuy.Any(a => ids.Contains(a.ID)))
            {
                //判断是否有进行中的一元购
                if (DB.ShopOneBuy.Any(a => ids.Contains(a.ID) && a.Count > 0))
                {
                    json.Msg = "不能删除已进行中的一元购";
                    return Json(json);
                }
                var names = DB.ShopOneBuy.Where(a => ids.Contains(a.ID)).Select(a => a.ShopProduct.Title)
                    .ToList().Aggregate((m, n) => m + "," + n);
                if (DB.ShopOneBuy.Delete(a => ids.Contains(a.ID)) > 0)
                {
                    json.IsSuccess = true;
                    json.Msg = "删除数据成功";
                }
                //添加操作日志
                DB.SysLogs.setMemberLog("Delete", "删除名字为[" + names + "]的一元购 ");
            }
            else
            {
                json.Msg = "未找到要删除的对象，请刷新页面重试";
            }
            return Json(json);
        }
        #endregion

        #region 一元购记录
        public ActionResult Logs(int id)
        {
            ViewBag.ID = id;
            return View();
        }
        public string getLogDataSource(int? id, DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            if (end != null) end = end.Value.AddDays(1);
            var query = DB.ShopOneBuyDetail.Where(a => (id == null ? true : a.OneBuyID == id)
                && (startTime == null ? true : a.CreateTime >= startTime)
                && (end == null ? true : a.CreateTime < end)
                && (string.IsNullOrEmpty(key) ? true : (a.MemberCode.Contains(key) || a.NickName.Contains(key))))
                 .Select(a => new
                 {
                     a.ID,
                     a.ShopOneBuy.Code,
                     a.ShopOneBuy.ShopProduct.Title,
                     a.Count,
                     a.CreateTime,
                     a.MemberCode,
                     a.NickName
                 });
            var total = query.Count();
            var list = query.OrderByDescending(a => a.ID).Skip(start).Take(length).ToList();

            return ToPage(list, total, start, length, draw);
        }
        #endregion
    }
}