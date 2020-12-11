using Business;
using Common;
using DataBase;
using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using UploadHandle;
using System.Web;
using Wechat;
using System.Collections.Generic;
using System.IO;

namespace Web.Controllers
{
    /// <summary>
    /// 主要用于Ajax请求数据
    /// </summary>
    public class AjaxController : Controller
    {

        public JsonResult GetProPrice(string str, int id)
        {


            //返回 原价，售价，库存
            string fh = "";
            var product = DB.ShopProduct.FindEntity(id);
            var category = DB.ShopProduct.GetCategoryId2(product.CategoryID.Value);
            var guigeproduct = DB.GuiGeProduct_Info.Where(a => a.SComment == str && a.ProductId == id && a.SName == category.ID).FirstOrDefault();
            if (guigeproduct == null)
            {
                fh += DB.ShopProduct.GetLingShouPrice(product) + "_" + DB.ShopProduct.GetYouHuiPrice(product) + "_" + DB.ShopProduct.GetPeiHuoPrice(product) + "_" + DB.ShopProduct.GetKuCun(product);
            }
            else
            {
                fh += guigeproduct.LingShou + "_" + guigeproduct.YouHui + "_" + guigeproduct.PeiHuo + "_" + guigeproduct.KuCun;
            }

            return Json(fh);
        }
        public JsonResult GetGuiGe(int id)
        {
            //获取二级目录
            var category = DB.ShopProduct.GetCategoryId2(id);

            var gugelist = DB.GuiGeName.Where(a => a.CName == category.Name).OrderBy(a => a.GId).ToList();


            List<string> list = new List<string>();
            for (int i = 0; i < gugelist.Count; i++)
            {
                string a = gugelist[i].GId + "," + gugelist[i].GName + ",";
                string[] xiang = gugelist[i].GInfoName.Split('_');
                int c = 1;
                foreach (var item in xiang)
                {
                    if (c == xiang.Length)
                    {
                        a += item;
                    }
                    else
                    {
                        a += item + ",";
                    }
                    c++;
                }
                list.Add(a);
            }

            //var list = DB.ShopProductCategory.getFrom23Layer(id).Select(a =>
            //new { id = a.Key.ToString(), text = a.Value }).ToList(); ;

            return Json(list);
        }
        // GET: Ajax
        #region 会员表
        /// <summary>
        /// 是否存在管理员
        /// </summary>
        /// <returns></returns>
        public JsonResult IsExistAdmin()
        {
            var p = Request["param"];
            var exist = DB.Sys_Employee.Where(a => a.LoginName == p).Select(a => new { a.RealName }).FirstOrDefault();
            if (exist != null)
            {
                return Json(new { status = "y", info = "管理员名字：" + exist.RealName });
            }
            else
            {
                return Json(new { status = "n", info = "你输入的编号不存在" });
            }
        }
        /// <summary>
        /// 是否存在会员
        /// </summary>
        /// <returns></returns>
        public JsonResult IsExistMember()
        {
            var code = Request["param"];
            var exist = DB.Member_Info.Where(p => p.Code == code).Select(a => new { a.NickName }).FirstOrDefault();
            if (exist != null)
            {
                return Json(new { status = "y", info = "会员名字：" + exist.NickName });
            }
            else
            {
                return Json(new { status = "n", info = "你输入的编号不存在" });
            }
        }
        public JsonResult IsExistMemberByCode(string code)
        {
            var exist = DB.Member_Info.Where(p => p.Code == code).Select(a => new { a.NickName }).FirstOrDefault();
            if (exist != null)
            {
                return Json(new { status = "y", msg = "会员名字：" + exist.NickName });
            }
            else
            {
                return Json(new { status = "n", msg = "你输入的编号不存在" });
            }
        }
        /// <summary>
        /// 是否存在已激活的会员
        /// </summary>
        /// <returns></returns>
        public JsonResult IsExistActivedMember()
        {
            var code = Request["param"];
            var exist = DB.Member_Info.Where(p => p.Code == code && p.IsActive == "已激活").Select(a => new { a.NickName }).FirstOrDefault();
            if (exist != null)
            {
                return Json(new { status = "y", info = "会员名字：" + exist.NickName });
            }
            else
            {
                return Json(new { status = "n", info = "你输入的编号不存在" });
            }
        }

        /// <summary>
        /// 是否存在已激活的邀请码
        /// </summary>
        /// <returns></returns>
        public JsonResult IsExistActivedMemberYQ()
        {
            var code = Request["param"];
            var exist = DB.Member_Info.Where(p => p.Code == code && p.IsActive == "已激活").Select(a => new { a.NickName }).FirstOrDefault();
            if (exist != null)
            {
                return Json(new { status = "y", info = "会员名字：" + exist.NickName });
            }
            else
            {
                return Json(new { status = "n", info = "你输入的邀请码不存在" });
            }
        }
        /// <summary>
        /// 当前会员编号是否存在
        /// </summary>
        /// <returns></returns>
        public JsonResult IsMemberId()
        {
            var code = Request["param"];
            var exist = DB.Member_Info.Any(p => p.Code == code);
            if (exist == false)
            {
                return Json(new { status = "y", info = "" });
            }
            else
            {
                return Json(new { status = "n", info = "你填写的编号已存在，请重新填写" });
            }
        }
        /// <summary>
        /// 店主是否存在
        /// </summary>
        /// <returns></returns>
        public JsonResult IsExistServiceCenter()
        {
            var code = Request["param"];
            var exist = DB.Member_Info.Any(a => a.IsServiceCenter == "是" && a.Code == code && a.IsActive == "已激活");
            if (exist == true)
            {
                return Json(new { status = "y", info = "" });
            }
            else
            {
                return Json(new { status = "n", info = "你输入的编号不存在" });
            }
        }
        #endregion


        #region 一元购
        public JsonResult IsOneBuy(int? id)
        {
            var code = Request["param"];
            var exist = DB.ShopOneBuyDetail.Any(a => a.OneBuyID == id && a.MemberCode == code);
            if (exist == true)
            {
                var name = DB.Member_Info.Where(p => p.Code == code).Select(a => a.NickName).FirstOrDefault();
                return Json(new { status = "y", info = "会员名：" + name });
            }
            else
            {
                return Json(new { status = "n", info = "你输入的会员编号未参加" });
            }
        }
        public JsonResult RandomWin(int id = 0)
        {
            var json = DB.ShopOneBuy.RandomWin(id);
            return Json(json);
        }
        #endregion

        #region _Layout:获取前台会员的留言回复信息 + 获取页面按钮权限
        public string getMyMsg(string id)
        {
            var list = DB.Sys_Msg.Where(a => a.ReceiverId == id && a.State == 0)
                  .Select(a => new { a.MsgId, a.SenderCode, a.Title });
            var r = list.Take(10).ToList();
            var count = r.Count;
            if (count >= 10)
            {
                count = list.Count();
            }
            var sb = new StringBuilder();
            sb.Append(count.ToString());
            sb.Append("_");
            foreach (var item in r)
            {
                sb.AppendFormat("<li><a href=\"/Member_Infos/MyMsg/Show/{0}\" >", item.MsgId);
                sb.Append("<span class=\"photo\"><img src=\"/assets/layouts/layout/img/avatar3_small.jpg\" class=\"img-circle\" alt=''></span>");
                sb.AppendFormat("<span class=\"subject\">来自：{0}</span>", item.SenderCode);
                sb.AppendFormat("<span class=\"message\">{0}</span></a></li>", item.Title);
            }
            return sb.ToString();
        }
        public JsonResult PermissionsLoad(string url, string roleid)
        {
            var IsShow = "none";
            var IsEdit = "hidden";
            var IsAdd = "none";
            var IsDelete = "none";
            var IsBackup = "none";
            var IsClear = "none";
            var IsRestore = "hidden";
            var IsReplace = "none";
            var id = Convert.ToInt32(roleid);

            var list1 = (from a in DB.Sys_Navigation.Where(p => !string.IsNullOrEmpty(p.link_url) && p.link_url.StartsWith(url))
                         join b in DB.Sys_Role_Nav.Where(ab => ab.role_id == id) on a.name equals b.nav_name
                         select new { b.action_type }).ToList();
            foreach (var item in list1)
            {
                if (item.action_type == "Show")
                {
                    IsShow = "block";
                }
                if (item.action_type == "Edit")
                {
                    IsEdit = "visible";
                }
                if (item.action_type == "Add")
                {
                    IsAdd = "block";
                }
                if (item.action_type == "Delete")
                {
                    IsDelete = "block";
                }
                if (item.action_type == "Backup")
                {
                    IsBackup = "block";
                }
                if (item.action_type == "Clear")
                {
                    IsClear = "block";
                }
                if (item.action_type == "Restore")
                {
                    IsRestore = "visible";
                }
                if (item.action_type == "Replace")
                {
                    IsReplace = "block";
                }
            }
            var obj = new { IsShow = IsShow, IsEdit = IsEdit, IsAdd = IsAdd, IsDelete = IsDelete, IsBackup = IsBackup, IsClear = IsClear, IsRestore = IsRestore, IsReplace = IsReplace };
            return Json(obj);
        }
        #endregion


        #region 验证码
        /// <summary>
        /// 验证码是否正确
        /// </summary>
        /// <returns></returns>
        public JsonResult IsExistCode()
        {
            var p = Request["param"];
            var mycode = Session["smscode"] as string;
            if (!string.IsNullOrEmpty(mycode))
            {
                if (p.ToLower() == mycode.ToLower())
                {
                    return Json(new { status = "y", info = "" });
                }
            }
            return Json(new { status = "n", info = "验证码不正确" });
        }
        /// <summary>
        /// 验证码
        /// add duk 
        /// </summary>
        public FileContentResult ValidateCode()
        {
            string code = "";
            System.IO.MemoryStream ms = new verify_code().Create(out code);
            Session["smscode"] = code;
            //验证码存储在Session中，供验证。  
            Response.ClearContent();//清空输出流 
            return File(ms.ToArray(), @"image/png");
        }

        /// <summary>
        /// 验证码---前台
        /// </summary>
        public FileContentResult ValidateCode_Above()
        {
            string code = "";
            System.IO.MemoryStream ms = new verify_code().Create(out code);
            Session["validate"] = code;
            return File(ms.ToArray(), @"image/png");
        }
        #endregion

        #region Fin_Info 收益,收益结算
        public string getFootTotal(DateTime? startTime, DateTime? end, string id = "")
        {
            //var list = DB.Fin_Info.getFootTotal(startTime, end, id);
            //return Common.JsonConverter.Serialize(list, true);
            var r = DB.Fin_Info.getFootTotal(startTime, end, id);
            return r;
        }
        public void FinToSettlement()
        {
            using (var db = new DbMallEntities())
            {
                DB.Fin_Info.ToSettlement(DateTime.Now, db, true);
            }
        }
        #endregion

        #region 网络图
        /// <summary>
        ///获取上层code
        /// </summary>
        /// <param name="id">当前会员code</param>
        /// <returns></returns>
        public string getUpCode(string id)
        {
            var m = DB.Member_Info.Where(p => p.Code == id).Select(a => a.UpperCode).FirstOrDefault();
            if (m != null)
            {
                return m;
            }
            return string.Empty;
        }
        /// <summary>
        /// 网络图搜索会员
        /// </summary>
        /// <param name="id">当前会员code</param>
        /// <param name="code">搜索的code</param>
        /// <returns></returns>
        public JsonResult SearchMemberByCode(string id, string code)
        {
            var list = DB.Member_Info.Where(p => p.Code == code || p.Code == id).Select(a => new { a.Code, a.Position }).ToList();
            var cur = list.FirstOrDefault(a => a.Code == id);
            var search = list.FirstOrDefault(a => a.Code == code);
            if (search != null && search.Position.StartsWith(cur.Position))
            {
                return Json(new { status = "y", msg = "" });
            }
            else
            {
                return Json(new { status = "n", msg = "会员不存在或无权查看" });
            }
        }
        public string Relasition(int x, int y, string code, int type, int layer)
        {
            var list = DB.Member_Info.GetRelation(code, type, x, y, layer);
            return Common.JsonConverter.Serialize(list);
        }
        /// <summary>
        /// 获取关系集合
        /// </summary>
        /// <param name="code">编码</param>
        /// <param name="type">轨数</param>
        /// <returns></returns>
        public string guanxi(string code, int type, int layer)
        {
            var list = DB.Member_Info.guanxi(code, type, layer);
            return Common.JsonConverter.Serialize(list);
        }
        public string getTooltip(string position, string code, int type, bool isAdmin = false)
        {
            var regurl = "/Member_Center/Register/Detail";
            if (isAdmin == true)
            {
                regurl = "/Admin_Member/FormalMember/Insert";
            }
            if (code.IndexOf(">") > 0 || code.IndexOf("<") > 0)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            var list = DB.Member_Info.Where(a => a.Position.StartsWith(position))
                .Select(a => new
                {
                    a.Code,
                    a.NickName,
                    a.MemberLevelName,
                    a.IsActive,
                    a.Position,
                    a.ActiveAmount
                }).ToList();
            var model = list.FirstOrDefault(p => p.Code == code);
            if (model != null)
            {
                sb.AppendFormat("账号：{0}<br />", model.Code);
                sb.AppendFormat("名称：{0}<br />", model.NickName);
                sb.AppendFormat("级别：{0}<br />", model.MemberLevelName);
              
                sb.AppendFormat("<div style=\"text-align:center;width:100%\"><a href=\"javascript:test('{0}')\" id=\"SubSave\" class=\"btn btn-xs green\"><i class=\"fa fa - check\"></i><span>查看</span></a></div>", model.Code);
            }
            else
            {
                //sb.AppendFormat("reg-{0}?position={1}", regurl, position);
            }
            return sb.ToString();
        }
        #endregion

        #region 省 市 区 
        public JsonResult GetCity(int? id)
        {
            var list = DB.Area_City.Where(p => p.pid == id).Select(a => new { id = a.id.ToString(), text = a.name }).ToList();
            //list.Insert(0, new { id = string.Empty, text = "请选择市" });
            return Json(list);
        }
        public JsonResult GetCounty(int? id)
        {
            var list = DB.Area_County.Where(p => p.cid == id).Select(a => new { id = a.id.ToString(), text = a.name }).ToList();
            //list.Insert(0, new { id = string.Empty, text = "请选择区" });
            return Json(list);
        }
        public JsonResult GetCategory2(int id)
        {
            var list = DB.ShopProductCategory.getFrom23Layer(id).Select(a => new { id = a.Key.ToString(), text = a.Value }).ToList(); ;
            //list.Insert(0, new { id = string.Empty, text = "请选择市" });
            return Json(list);
        }
        #endregion

        #region RefreshCache 刷新缓存
        public void RefreshCache()
        {
            DB.RefreshCache();
        }
        #endregion


        #region 上传处理--图片

        public void Upload()
        {
            Receiver _receive = new Receiver();
            //接收文件成功
            _receive.OnSuccess = (data) =>
            {
                //此处，有需要的情况下，执行数据库操作
                LogHelper.Info(string.Format("新文件名{0},旧文件名{1}", data.NewName, data.OldName));
            };
            _receive.OnError = (err) =>
            {
                LogHelper.Error("上传文件失败：" + err.Message);
            };
        }
        public void Upload1()
        {
            Receiver _receive = new Receiver();
            //接收文件成功
            _receive.OnSuccess = (data) =>
            {
                //此处，有需要的情况下，执行数据库操作
                LogHelper.Info(string.Format("新文件名{0},旧文件名{1}", data.NewName, data.OldName));
            };
            _receive.OnError = (err) =>
            {
                LogHelper.Error("上传文件失败：" + err.Message);
            };
        }
        #endregion

        #region 测试代码

        public ActionResult TestUpload()
        {
            return View();
        }
        public ActionResult Test()
        {
            LogHelper.Error("测试");

            //using (var db=new DbMallEntities())
            {
                var db = DB.Member_Info.Context as DbMallEntities;
                var xx = db.Database.SqlQuery<int>("select count(*) from member_info");
                var query = db.Member_Info.Where(a => a.RecommendCode == "admin")
               .Where(a => db.Member_Info.Any(b => b.MemberLevelId == 2 && b.Position.StartsWith(a.Position)))
               .Count();
                var x = query;
            }


            return Content("");
        }
        [OutputCache(CacheProfile = "SqlDependencyCache")]
        public ActionResult TestCache()
        {
            return View();
        }
        public ActionResult TT()
        {
            int x = ShopEnum.ArticleCategoryType.Normal.GetHashCode();
            if (x == 1)
            {

            }
            return View();
        }
        #endregion


        #region 省 市 区 
        public JsonResult GetProv(int? id)
        {
            var list = DB.Area_Province.Where().Select(a => new { id = a.id.ToString(), text = a.name }).ToList();
            return Json(list);
        }
        #endregion

        #region 支付接口回调处理
        /// <summary>
        /// 下单支付成功
        /// </summary>
        /// <returns></returns>
        //public void Trade()
        //{
        //    try
        //    {
        //        string html = Request.Params["html"];
        //        PayInfo info = PayHelper.GetBack(html);
        //        using (var tran = DB.ShopOrder.BeginTransaction)
        //        {
        //            try
        //            {
        //                //1.获取订单对象
        //                ShopOrder order = DB.ShopOrder.FindEntity(a => a.TraceNo == info.Order_ID);
        //                //标记成功
        //                order.State = ShopEnum.OrderState.Pay.GetHashCode();
        //                order.PayTime = DateTime.Now;
        //                if (DB.ShopOrder.Update(order) == false)
        //                    throw new Exception("修改订单失败");

        //                if (order.PayWay == ShopEnum.PayType.现金加积分.GetHashCode().ToString())
        //                {
        //                    Member_Info curUser = DB.Member_Info.FindEntity(order.MemberID);
        //                    if (curUser.Scores < order.RealScore)
        //                        throw new Exception("积分不足");
        //                    curUser.Scores -= order.RealScore;
        //                    if (DB.Member_Info.Update(curUser) == false)
        //                        throw new Exception("修改会员信息失败");
        //                }

        //                //发放收益
        //                DB.ShopOrder.SendBonus(order);


        //                tran.Complete();//提交数据库
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //        }
        //        //标记接收接收成功
        //        PayHelper.SetSuccess(info);
        //    }
        //    catch (Exception ex)
        //    {
        //        StringBuilder str = new StringBuilder();
        //        Exception inner = ex;
        //        while (inner != null)
        //        {
        //            str.AppendLine(inner.Message);
        //            inner = inner.InnerException;
        //        }
        //        LogHelper.Error("支付失败：" + str.ToString());
        //        LogHelper.Error("------------------->");
        //        LogHelper.Error(ex.StackTrace);
        //    }
        //}
        /// <summary>
        /// 充值积分成功
        /// </summary>
        //public void Score(PayInfo info)
        //{
        //    try
        //    {

        //        LogHelper.Debug("订单ID：" + info.Order_ID);
        //        //1.充值记录
        //        int orderid = Convert.ToInt32(info.Order_ID.Split('_')[1]);

        //        lock (scoreLock)
        //        {
        //            using (var tran = DB.ScoreRecord.BeginTransaction)
        //            {
        //                try
        //                {
        //                    ScoreRecord record = DB.ScoreRecord.FindEntity(orderid);
        //                    if (record == null)
        //                        throw new Exception("获取记录失败");
        //                    if (record.State == Enums.ScoreState.已成功.GetHashCode())
        //                        throw new Exception("当前记录已经过期");

        //                    if (record.State != Enums.ScoreState.已成功.GetHashCode())
        //                    {
        //                        //修改记录状态
        //                        record.State = Enums.ScoreState.已成功.GetHashCode();
        //                        DB.ScoreRecord.Update(record);
        //                        //积分到账
        //                        Member_Info member = DB.Member_Info.FindEntity(record.MemberID);
        //                        member.Scores += record.Scores;
        //                        DB.Member_Info.Update(member);

        //                        //发放收益
        //                        decimal baseAmount = record.Amount;
        //                        //推广奖&互助奖
        //                        DB.Jiang.RecommendNew(member, baseAmount);
        //                        //领导奖
        //                        DB.Jiang.LeadBonus(member, baseAmount);

        //                        //会员升级操作
        //                        DB.Jiang.Member_Upgrade(member, DB.Sys_Level.ToList());
        //                        tran.Complete();//执行成功
        //                    }

        //                    //标记接收接收成功
        //                    PayHelper.SetSuccess(info);
        //                }
        //                catch (Exception ex)
        //                {
        //                    DB.Rollback();
        //                    throw ex;
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        StringBuilder str = new StringBuilder();
        //        Exception inner = ex;
        //        while (inner != null)
        //        {
        //            str.AppendLine(inner.Message);
        //            inner = inner.InnerException;
        //        }
        //        LogHelper.Error("充值积分：" + str.ToString());
        //        LogHelper.Error("------------------->");
        //        LogHelper.Error(ex.StackTrace);
        //    }
        //}



        #region 微信支付
        /// <summary>
        /// 下单支付成功
        /// </summary>
        /// <returns></returns>
        public void TradeWX()
        {
            try
            {
                lock (wxLock)
                {
                    PayManage _manage = new PayManage();
                    _manage.OnPaySuccess += (result) =>
                    {
                        using (var tran = DB.ShopOrder.BeginTransaction)
                        {
                            try
                            {
                                //1.获取订单对象
                                ShopOrder order = DB.ShopOrder.FindEntity(a => a.TraceNo == result.out_trade_no);
                                if (order.State == ShopEnum.OrderState.Pay.GetHashCode())
                                    throw new Exception("当前订单已经支付");

                                //标记成功
                                order.State = ShopEnum.OrderState.Pay.GetHashCode();
                                order.PayTime = DateTime.Now;
                                if (DB.ShopOrder.Update(order) == false)
                                    throw new Exception("修改订单失败");
                                if (order.PayWay == ShopEnum.PayType.现金加积分.GetHashCode().ToString())
                                {
                                    Member_Info curUser = DB.Member_Info.FindEntity(order.MemberID);
                                    if (curUser.Scores < order.RealScore)
                                        throw new Exception("积分不足");
                                    curUser.Scores -= order.RealScore;
                                    if (DB.Member_Info.Update(curUser) == false)
                                        throw new Exception("修改会员信息失败");
                                }
                                //发放收益
                                //DB.ShopOrder.SendBonus(order);

                                tran.Complete();//提交数据库
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                        //支付成功
                        LogHelper.Info($"支付成功（商城购物）,微信单号:{result.transaction_id},系统单号:{result.out_trade_no},总金额:{result.total_fee}");
                    };
                    _manage.BackHandle(Request, Response);
                }
            }
            catch (Exception ex)
            {
                StringBuilder str = new StringBuilder();
                Exception inner = ex;
                while (inner != null)
                {
                    str.AppendLine(inner.Message);
                    inner = inner.InnerException;
                }
                LogHelper.Error("支付失败：" + str.ToString());
                LogHelper.Error("------------------->");
                LogHelper.Error(ex.StackTrace);
            }
        }
        private static object scoreLock = new object();
        private static object wxLock = new object();
        /// <summary>
        /// 充值积分成功
        /// </summary>
        //public void ScoreWX()
        //{
        //    try
        //    {
        //        lock (wxLock)
        //        {
        //            PayManage _manage = new PayManage();
        //            _manage.OnPaySuccess += (result) =>
        //            {
        //                using (var tran = DB.ShopOrder.BeginTransaction)
        //                {
        //                    try
        //                    {
        //                        //1.获取订单对象
        //                        int orderid = Convert.ToInt32(result.out_trade_no.Split('_')[1]);
        //                        ScoreRecord record = DB.ScoreRecord.FindEntity(orderid);
        //                        if (record == null)
        //                            throw new Exception("获取记录失败");
        //                        if (record.State == Enums.ScoreState.已成功.GetHashCode())
        //                            throw new Exception("当前记录已经过期");

        //                        if (record.State != Enums.ScoreState.已成功.GetHashCode())
        //                        {
        //                            //修改记录状态
        //                            record.State = Enums.ScoreState.已成功.GetHashCode();
        //                            DB.ScoreRecord.Update(record);
        //                            //积分到账
        //                            Member_Info member = DB.Member_Info.FindEntity(record.MemberID);
        //                            member.Scores += record.Scores;
        //                            DB.Member_Info.Update(member);

        //                            //发放收益
        //                            decimal baseAmount = record.Amount;
        //                            //推广奖&互助奖
        //                            DB.Jiang.RecommendNew(member, baseAmount);
        //                            //领导奖
        //                            DB.Jiang.LeadBonus(member, baseAmount);

        //                            //会员升级操作
        //                            DB.Jiang.Member_Upgrade(member, DB.Sys_Level.ToList());

        //                            tran.Complete();//提交数据库
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        throw ex;
        //                    }
        //                }
        //                //支付成功
        //                LogHelper.Info($"支付成功（积分充值）,微信单号:{result.transaction_id},系统单号:{result.out_trade_no},总金额:{result.total_fee}");
        //            };
        //            _manage.BackHandle(Request, Response);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        StringBuilder str = new StringBuilder();
        //        Exception inner = ex;
        //        while (inner != null)
        //        {
        //            str.AppendLine(inner.Message);
        //            inner = inner.InnerException;
        //        }
        //        LogHelper.Error("充值积分：" + str.ToString());
        //        LogHelper.Error("------------------->");
        //        LogHelper.Error(ex.StackTrace);
        //    }
        //}
        #endregion


        /// <summary>
        /// 判断订单是否付款成功
        /// </summary>
        /// <returns></returns>
        public string CheckOrder(string orderid)
        {
            ShopOrder order = DB.ShopOrder.FindEntity(orderid);
            if (order == null)
                return "0";
            if (order.State == ShopEnum.OrderState.Pay.GetHashCode())
                return "1";
            return "0";
        }



        /// <summary>
        /// 获取所有的社区店列表
        /// </summary>
        public string GetDian(int? provID, int? cityID, int? countyID)
        {
            var query = DB.Member_Info.Where(q => q.MemberLevelId == 4);
            if (provID != null)
                query = query.Where(q => q.ProvId == provID);
            if (cityID != null)
                query = query.Where(q => q.CityId == cityID);
            if (countyID != null)
                query = query.Where(q => q.CountyId == countyID);

            var list = query.OrderBy(q => q.Code).Select(q => new
            {
                code = q.Code,
                name = q.NickName
            });

            return list.ToJsonString();
        }
        public int getonlinecount()
        {
            return DB.XmlConfig.XmlShop.GetOnlineCount();
        }
        #endregion
        public ActionResult Upload(string path)
        {
            #region + 变量
            //文件大小（字节数）
            long fileSize = 0;
            //重命名的文件名称
            string tempName = "";
            //物理路径+重命名的文件名称
            string fileName = "";
            //文件扩展名
            string extname = "";
            //虚拟路径
            if (path.IsNull())
            {
                path = "/upload/ ";
            }
            //上传固定路径
            string realpath = Server.MapPath(path);
            // string realpath = "https://www.chymall.net//upload/Comment/";

            //<a href="http://www.51softs.com/tag/%e4%b8%8a%e4%bc%a0%e6%96%87%e4%bb%b6" title="查看更多关于 上传文件 的文章" target="_blank">上传文件</a>夹名称
            string dir = "";
            //获取提交的文件
            HttpPostedFileBase file = Request.Files[0];
            //反馈对象        
            Base<object> _response = new Base<object>();
            #endregion
            #region + 服务器本地文件上传处理
            try
            {
                if (file != null && !string.IsNullOrEmpty(file.FileName))
                {
                    dir = DateTime.Now.ToString("yyyy-MM-dd");
                    if (!Directory.Exists(realpath + dir))
                    {
                        Directory.CreateDirectory(realpath + dir);
                    }
                    extname = file.FileName.Substring(file.FileName.LastIndexOf('.'), (file.FileName.Length - file.FileName.LastIndexOf('.')));
                    tempName = Guid.NewGuid().ToString().Substring(0, 6) + extname;
                    fileName = realpath + dir + "/" + tempName;
                    fileSize += file.ContentLength;
                    using (FileStream fs = System.IO.File.Create(fileName))
                    {
                        file.InputStream.CopyTo(fs);
                        fs.Flush();
                    }
                    _response.Data = new
                    {
                        src = path + dir + "/" + tempName,
                    };
                    _response.Message = "服务器本地上传成功";
                }
            }
            catch (Exception ex)
            {
                _response.Code = -100;
                _response.Message = "服务器本地上传失败/n" + "错误信息：" + ex.Message;
            }
            #endregion
            return Json(_response);
        }
    }
}