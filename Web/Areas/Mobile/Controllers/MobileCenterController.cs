using Business;
using Business.Implementation;
using Common;
using DataBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Wechat;
using WxPayAPI;

namespace Web.Areas.Mobile.Controllers
{
    public class MobileCenterController : MobileBaseController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //判断是否登录，如果没有登录跳转登录页
            //if (User_Shop.IsLogin() == false)
            //{
            //    string url = "";
            //    //如果是微信登录跳转到授权页面
            //    if (Url_Mobile.IsWechat())
            //    {
            //        url = Url_Mobile.GetLogin() + "/auth";
            //    }
            //    else
            //    {
            //        url = Url_Mobile.GetLogin();
            //    }
            //    url += "?redirecturl=" + ReqHelper.req.Url.AbsolutePath;
            //    if (string.IsNullOrEmpty(url) == false)
            //        filterContext.Result = new RedirectResult(url);
            //}
            if (User_Shop.IsLogin() == false)
            {
                string url = "";
                url = Url_Mobile.GetLogin();
                //url += "?redirecturl=" + ReqHelper.req.Url.AbsolutePath;
                if (string.IsNullOrEmpty(url) == false)
                    filterContext.Result = new RedirectResult(url);
            }
        }

        // GET: Mobile/MUserInfo
        /// <summary>
        /// 登录人id
        /// </summary>
        public string CurrentUserID
        {
            get
            {
                return User_Shop.GetMemberID();
            }
        }
        /// <summary>
        /// 登录对象
        /// </summary>
        public Member_Info CurrentUser
        {
            get
            {
                return User_Shop.GetMember_Info();
            }
        }
        public ActionResult UpLoadTu(string File1, string path = "/upload/Link/")
        {
            JsonHelp json = new JsonHelp(true);
            //上传和返回(保存到数据库中)的路径
            var tempPath = Server.MapPath(path);
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);//不存在就创建目录 
            }
            else//清空文件夹
            {
                foreach (string var in Directory.GetFiles(tempPath))
                {
                    DB.Member_Info.DeleteFile(var);
                }
            }
            if (Request.Files.Count <= 0) return Json(json);
            var imgFile = Request.Files["file"];
            //创建图片新的名称
            var nameImg = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            //获得上传图片的路径
            var strPath = imgFile.FileName;
            //获得上传图片的类型(后缀名)
            var type = strPath.Substring(strPath.LastIndexOf(".", StringComparison.Ordinal) + 1).ToLower(); ;

            //拼写数据库保存的相对路径字符串
            // savepath = "..\\" + path + "\\";
            path += nameImg + "." + type;
            //拼写上传图片的路径
            var uppath = Server.MapPath(path);
            // uppath += nameImg + "." + type;
            //上传图片
            imgFile.SaveAs(uppath);

            return Json(json.Msg = path);
        }
        public ActionResult Index(string isPhone)
        {
            ViewBag.isPhone = isPhone;
            //Uri u = Request.Url;
            //string url = "http://" + u.Authority;
            //string code = Common.CryptHelper.DESCrypt.Encrypt(CurrentUser.MemberId);
            //string path = Url_Mobile. GetRegister() + $"?code={code}";
            var model = DB.Member_Info.FindEntity(CurrentUserID);
            //if (string.IsNullOrEmpty(model.Position))
            //{
            //    string qrImage = Common.ImageCombin.CombinQR(Url_Mobile.GetQrCodeLink(CurrentUser), CurrentUser.MemberId, Url_Mobile.GetImg(model.Pwd3));
            //    if (!string.IsNullOrEmpty(model.Pwd3))
            //    {
            //        model.Position = qrImage;
            //    }
            //    ViewBag.Src = qrImage;
            //    DB.Member_Info.Update(model);
            //}
            //else
            //{
            //    ViewBag.Src = CurrentUser.Position;
            //}


            return View();
        }

        #region 基本信息等
        /// <summary>
        /// 基本信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Info()
        {
            return View();
        }
        public ActionResult Cash(double Longitude = 0, double Latitude = 0)
        {
            var query = DB.Member_Info.Where(q => q.IsServiceCenter == "是");

            if (Longitude > 0)
            {
                query = query.OrderByDistance(Longitude, Latitude);
            }
            else { query = query.OrderByDescending(q => q.CreateTime); }
            ViewBag.List = query.ToList();
            return View();
        }
        public ActionResult PwdInfo()
        {
            return View();
        }
        /// <summary>
        /// 编辑基本信息
        /// </summary>
        /// <returns></returns>
        public ActionResult InfoEdit()
        {
            ViewData["Prov"] = DB.Area_Province.ToList();
            ViewData["Banks"] = DB.Sys_BasicData.getBasicDataByType("银行");
            return View();
        }
        //public string SaveImage()
        //{
        //    return FileUpload.UpLoad("/upload/images/", Request, Server, 1);
        //}

        public ActionResult SaveImage()
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
            //if (path.IsNull())
            //{

            //}
            string path = "/upload/images/";
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
        /// <summary>
        /// 保存基本信息
        /// </summary>
        /// <returns></returns>
        public ActionResult InfoSave(Member_Info entity)
        {
            try
            {

                if (DB.XmlConfig.XmlSite.IsJiHuo)
                {
                    var code = Session["smscode"] as string;
                    if (string.IsNullOrEmpty(code))
                        throw new Exception("验证码过期");
                    if (code != entity.OpenBank)
                        throw new Exception("验证码不正确");

                }
                Member_Info model = User_Shop.GetMember_Info();
                if (DB.Member_Info.Any(a => a.Mobile == entity.Mobile && a.MemberId != model.MemberId))
                {
                    throw new Exception("手机号已存在");

                }


                //完善资料

                model.Mobile = entity.Mobile;
                if (model.IsDL != "是")
                {
                    model.NickName = entity.NickName;

                    var rec = DB.Member_Info.Where(a => a.RecommendId == model.MemberId && a.MemberId!=model.MemberId).ToList();
                    foreach (var item in rec)
                    {
                        var recommendmodel = DB.Member_Info.FindEntity(item.MemberId);
                        recommendmodel.RecommendName = model.NickName;
                        DB.Member_Info.Update(recommendmodel);
                    }

                    model.IdCode = entity.IdCode;
                    model.IsDL = "是";

                    model.BankAccount = entity.BankAccount;
                    model.BankCode = entity.BankCode;
                    model.BankName = entity.BankName;
                }

                if (DB.Member_Info.Update(model) == false)
                    throw new Exception("修改会员信息失败");

                return Success("修改成功");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        public ActionResult AppIndex()
        {
            ViewData["Sheng"] = DB.Area_Province.ToList();
            var model = DB.Member_Info.FindEntity(CurrentUser.MemberId);
            return View(model);
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="oldPwd"></param>
        /// <param name="pwd"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult UpdatePwd(string oldPwd, string pwd, int type)
        {
            JsonHelp result = new JsonHelp(true, "操作成功");
            try
            {
                if (DB.XmlConfig.XmlSite.IsJiHuo)
                {
                    var code = Session["smscode"] as string;
                    if (string.IsNullOrEmpty(code))
                        throw new Exception("验证码过期");
                    if (code != oldPwd)
                        throw new Exception("验证码不正确");

                }
                Member_Info model = User_Shop.GetMember_Info();
                if (type == 1)
                {

                    model.LoginPwd = Common.CryptHelper.DESCrypt.Encrypt(pwd);
                }
                else
                {

                    model.Pwd2 = Common.CryptHelper.DESCrypt.Encrypt(pwd);
                }
                if (DB.Member_Info.Update(model) == false)
                    throw new Exception("修改失败");
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = ex.Message;
            }
            return Json(result);
        }
        /// <summary>
        /// 我的名片
        /// </summary>
        /// <returns></returns>
        public ActionResult MyCard()
        {
            return View();
        }
        #endregion

        #region 推广记录，新闻
        /// <summary>
        /// 推广记录
        /// </summary>
        /// <returns></returns>
        public ActionResult Promotion()
        {
            return View();
        }
        /// <summary>
        /// 推广列表
        /// </summary>
        /// <returns></returns>
        public PartialViewResult PromotionList(int skipCount = 0)
        {
            var query = DB.Member_Info.Where(q => q.RecommendId == CurrentUserID);
            ViewBag.recordCount = query.Count();
            query = query.OrderByDescending(q => q.CreateTime);
            List<Member_Info> list = query.Skip(skipCount)
                .Take(8)
                .ToList();
            ViewBag.skipCount = skipCount + list.Count;
            return PartialView(list);
        }
        /// <summary>
        /// 新闻首页
        /// </summary>
        /// <returns></returns>
        public ActionResult News()
        {
            return View();
        }
        /// <summary>
        /// 新闻列表
        /// </summary>
        /// <returns></returns>
        public PartialViewResult NewsList(int shipCount = 0)
        {
            var query = DB.News_Info.Where();
            ViewBag.recordCount = query.Count();
            query = query.OrderByDescending(q => q.CreateTime);
            List<News_Info> list = query.Skip(shipCount)
                .Take(8)
                .ToList();
            ViewBag.skipCount = shipCount + list.Count;
            return PartialView(list);
        }
        /// <summary>
        /// 新闻详细
        /// </summary>
        /// <returns></returns>
        public ActionResult NewsDetail(int id)
        {
            ViewBag.id = id;
            return View();
        }
        #endregion

        #region 收藏，评论
        /// <summary>
        /// 我的收藏
        /// </summary>
        /// <returns></returns>
        public ActionResult Collect()
        {
            return View();
        }
        /// <summary>
        /// 收藏列表
        /// </summary>
        /// <returns></returns>
        public PartialViewResult CollectList(int skipCount = 0)
        {
            var query = DB.ShopCollectionProduct.Where(q => q.MemberID == CurrentUserID && q.ShopProduct.IsEnable == true);
            ViewBag.recordCount = query.Count();
            query = query.OrderByDescending(q => q.CreateTime);

            List<ShopCollectionProduct> list = query.Skip(skipCount)
                .Take(8)
                .ToList();
            ViewBag.skipCount = skipCount + list.Count;
            return PartialView(list);
        }



        /// <summary>
        /// 我的评论
        /// </summary>
        /// <returns></returns>
        public ActionResult Comment()
        {
            return View();
        }
        /// <summary>
        /// 评论列表
        /// </summary>
        /// <returns></returns>
        public PartialViewResult CommentList(int skipCount = 0)
        {
            var query = DB.ShopOrderComment.Where(q => q.MemberID == CurrentUserID);
            ViewBag.recordCount = query.Count();
            query = query.OrderByDescending(q => q.CreateTime);
            List<ShopOrderComment> list = query.Skip(skipCount)
                .Take(8)
                .ToList();
            ViewBag.skipCount = skipCount + list.Count;
            return PartialView(list);
        }
        #endregion

        #region 我的订单
        /// <summary>
        /// 我的订单
        /// </summary>
        /// <returns></returns>
        public ActionResult Bill()
        {
            return View();
        }
        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="state"></param>
        /// <param name="skipCount"></param>
        /// <returns></returns>
        public PartialViewResult BillList(int? type, int skipCount = 0)
        {
            var query = DB.ShopOrder.Where(q => q.MemberID == CurrentUserID);
            if (type != null)
            {
                List<int> stateList = new List<int>();
                if (type == 1) //待付款
                {
                    stateList.Add(1);
                }
                else if (type == 0)//待处理
                {
                    stateList.Add(0);
                }
                else if (type == 2)//待发货
                {
                    stateList.Add(2);
                }
                //else if (type == 30)//委托中
                //{
                //    stateList.Add(30);
                //}
                else if (type == 40)//挂卖中
                {
                    stateList.Add(40);
                }
                else if (type == 3)//待收货
                {
                    stateList.Add(3);
                    stateList.Add(-32);
                    stateList.Add(-33);
                }
                else if (type == 4)//已完成
                {
                    stateList.Add(10);
                    stateList.Add(20);
                    stateList.Add(-35);
                }
                else if (type == 10)//已完成
                {
                    stateList.Add(10);
                    stateList.Add(20);
                    stateList.Add(-35);
                }
                query = query.Where(q => stateList.Contains(q.State));
            }


            ViewBag.recordCount = query.Count();
            query = query.OrderByDescending(q => q.SubmitTime);
            List<ShopOrder> list = query.Skip(skipCount)
                .Take(8)
                .ToList();
            return PartialView(list);
        }
        #endregion
        public ActionResult ShouHuo(string id)
        {
            var json = new JsonHelp();
            json = DB.ShopOrder.ShouHuo(id);

            return Json(json);
        }

        #region 购物流程
        /// <summary>
        /// 我的购物车
        /// </summary>
        /// <returns></returns>
        public ActionResult Cart()
        {
            return View();
        }
        public ActionResult Bonus()
        {
            return View();
        }


        public ActionResult AddressDetail(int id)
        {
            var address = DB.ShopMyAddress.FindEntity(id);
            if (id == 0)
            {
                address = new ShopMyAddress();
            }
            ViewBag.m = DB.Member_Info.FindEntity(CurrentUser.MemberId);
            ViewBag.address = address;

            return View();
        }
        public ActionResult BonusList(int skipCount = 0)
        {
            var query = DB.Fin_Info.Where(q => q.MemberId == CurrentUserID);
            //分页
            query = query.OrderByDescending(q => q.CreateTime);

            List<Fin_Info> list = query.Skip(skipCount)
                .Take(1000)
                .ToList();

            return PartialView(list);
        }

        public ActionResult Convert()
        {
            ViewBag.m = DB.Member_Info.FindEntity(CurrentUser.MemberId);

            return View();
        }

        public ActionResult ConvertList()
        {
            ViewBag.m = DB.Member_Info.FindEntity(CurrentUser.MemberId);

            return View();
        }

        public ActionResult ZhuanHuan()
        {
            ViewBag.m = DB.Member_Info.FindEntity(CurrentUser.MemberId);

            return View();
        }

        public ActionResult ZhuanHuanList()
        {
            ViewBag.m = DB.Member_Info.FindEntity(CurrentUser.MemberId);

            return View();
        }
        public ActionResult Transfer()
        {
            ViewBag.m = DB.Member_Info.FindEntity(CurrentUser.MemberId);

            return View();
        }

        public ActionResult TransferList()
        {
            ViewBag.m = DB.Member_Info.FindEntity(CurrentUser.MemberId);

            return View();
        }


        public ActionResult ToZhuanHuan(decimal Amount, string PayPwd)
        {
            try
            {
                var Pwd2 = Common.CryptHelper.DESCrypt.Encrypt(PayPwd);
                var member = DB.Member_Info.FindEntity(CurrentUser.MemberId);
                var Poundage = 0;
                var entity = new Fin_Convert()
                {
                    MemberId = member.MemberId,
                    MemberCode = member.Code,
                    NickName = member.NickName,
                    Amount = Amount,
                    ConvertType = "奖金转余额",
                    CreateTime = DateTime.Now
                };
                DB.Fin_Convert.Save(Pwd2, entity);
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }

        }
        public ActionResult ToTransfer(decimal Amount, string PayPwd, string ToMemberCode)
        {
            try
            {
                var Pwd2 = Common.CryptHelper.DESCrypt.Encrypt(PayPwd);
                var member = DB.Member_Info.FindEntity(CurrentUser.MemberId);
                var Poundage = 0;
                var entity = new Fin_Transfer()
                {
                    FromMemberId = member.MemberId,
                    FromMemberCode = member.Code,
                    FromNickName = member.NickName,
                    Amount = Amount,
                    TransferType = "余额互转",
                    CreateTime = DateTime.Now,
                    ToMemberCode = ToMemberCode

                };
                JsonHelp json = DB.Fin_Transfer.Save(Pwd2, entity);
                if (!json.IsSuccess)
                {
                    return Error(json.Msg);
                }
                else
                {
                    return Success("操作成功");
                }

            }
            catch (Exception ex)
            {
                return Error(ex);
            }

        }
        public ActionResult SaveService()
        {
            try
            {
                var entity = DB.Member_Info.FindEntity(CurrentUser.MemberId);

                DB.Member_Info.ServiceApply(entity);
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }

        }
        public ActionResult SaveDL(Member_Info entity)
        {
            try
            {

                var member = DB.Member_Info.FindEntity(entity.MemberId);
                member.DiZhi = entity.DiZhi;
                member.JingDu = entity.JingDu;
                member.WeiDu = entity.WeiDu;
                member.ShopImage = entity.ShopImage;
                member.ShopName = entity.ShopName;
                JsonHelp help = DB.Member_Info.ServiceDL(member);
                if (help.Status == "n")
                {
                    return Error(help.Msg);
                }
                else
                {
                    return Success(help.Msg);
                }

            }
            catch (Exception ex)
            {
                return Error(ex);
            }

        }
        public ActionResult ToRemit(decimal Amount = 0, string Image = "")
        {
            try
            {

                var model = DB.Member_Info.FindEntity(CurrentUser.MemberId);

                var entity = new Fin_Remit();
                entity.RemitState = "申请中";
                entity.Image = Image;
                entity.Amount = Amount;
                entity.BankName = model.BankName;
                entity.BankAccount = model.BankAccount;
                entity.BankCode = model.BankCode;
                entity.OpenBank = model.OpenBank;
                entity.ServiceCenterCode = "";


                entity.CreateTime = DateTime.Now;
                entity.MemberId = model.MemberId;
                entity.MemberCode = model.Code;
                entity.NickName = model.NickName;

                var help = DB.Fin_Remit.Save(entity);
                if (help.Status == "n")
                {
                    return Error(help.Msg);
                }
                else
                {
                    return Success(help.Msg);
                }
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        public ActionResult ToDraw(decimal DrawAmount = 0, string PayPwd = "")
        {
            try
            {
                PayPwd = Common.CryptHelper.DESCrypt.Encrypt(PayPwd);
                var model = DB.Member_Info.FindEntity(CurrentUser.MemberId);

                var entity = new Fin_Draw();
                entity.DrawState = "未发放";
                entity.CreateTime = DateTime.Now;

                entity.BankName = model.BankName;
                entity.BankAccount = model.BankAccount;
                entity.BankCode = model.BankCode;
                entity.OpenBank = model.OpenBank;
                entity.DrawAmount = DrawAmount;
                entity.ServiceCenterCode = "";


                entity.CreateTime = DateTime.Now;
                entity.MemberId = model.MemberId;
                entity.MemberCode = model.Code;
                entity.NickName = model.NickName;
                entity.Comment = "";
                DB.Fin_Draw.Save(PayPwd, entity);
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        public ActionResult DrawList()
        {
            var model = DB.Member_Info.FindEntity(CurrentUser.MemberId);
            ViewBag.m = model;

            return View(model);
        }
        public ActionResult Link(string isPhone)
        {
            ViewBag.isPhone = isPhone;
            var model = DB.Member_Info.FindEntity(CurrentUser.MemberId);
            ViewBag.m = model;

            return View(model);
        }
        public ActionResult LiuShui(int id)
        {

            ViewBag.id = id;

            return View();
        }
        public ActionResult Draw()
        {
            ViewBag.m = DB.Member_Info.FindEntity(CurrentUser.MemberId);
            ViewData["Banks"] = DB.Sys_BasicData.getBasicDataByType("银行");
            return View();
        }
        #region 付款处理
        /// <summary>
        /// 确认订单页面
        /// </summary>
        /// <returns></returns>
        public ActionResult payment(string orderid)
        {
            LogHelper.Error("回调311111111111111111111111");
            #region 如果页面无参数传入，微信会把Payment作为一个目录处理，这里增加个判断，如果无参数，则随便加个参数
            if (Request.QueryString.Count == 0)
            {
                return RedirectToAction("/mobile/mobilecenter/bill", new { q = "t", orderid = orderid });
            }
            #endregion

            List<string> orderList = new List<string>();
            if (string.IsNullOrEmpty(orderid))
            {
                //throw new Exception("获取订单id失败");
                //来自购物车
                string cartlist = CookieHelper.GetCookieValue("cart");
                if (string.IsNullOrEmpty(cartlist))
                    return RedirectToAction("Bill");
                //获取下单总金额
                cartlist = SecurityHelper.UrlDecode(cartlist);
                Dictionary<int, int> prodList = cartlist.JsonDeserializer<Dictionary<int, int>>();

                //生成订单
                Member_Info curUser = User_Shop.GetMember_Info();
                DB.ShopOrder.GenerateOrder(curUser, 0, null);

                //3.清空cookie
                CookieHelper.ClearCookie("cart");
                CookieHelper.ClearCookie("total");

                return RedirectToAction("Bill");//跳转待付款
            }
            orderList.Add(orderid);
            ShopOrder model = DB.ShopOrder.FindEntity(orderid);
            if (model == null)
                throw new Exception("获取订单信息失败");
            if (model.TraceNo.IsNull())
            {
                model.TraceNo = model.GUID;
                DB.ShopOrder.Update(model);
            }
            CookieHelper.SetCookie("traceno", model.TraceNo, 1);
            CookieHelper.SetCookie("orderlist", orderList.ToJsonString(), null);

            return View(model);
        }
        /// <summary>
        /// 确认付款操作
        /// </summary>
        /// <returns></returns>
        public ActionResult DoPay(int addressid, string paytype, string remark, bool isziti, string payPwd)
        {
            JsonHelp json = new JsonHelp(true);
            try
            {
                Xml_Site config = DB.XmlConfig.XmlSite;
                Xml_Shop shopConfig = DB.XmlConfig.XmlShop;
                Member_Info curUser = User_Shop.GetMember_Info();
                //获取订单信息
                string orderlist = CookieHelper.GetCookieValue("orderlist");
                List<string> orderList = orderlist.JsonDeserializer<List<string>>();
                if (orderList == null)
                {
                    throw new Exception("订单支付异常");
                }
                if (orderList.Count <= 0)
                    throw new Exception("获取订单信息失败");
                DB.ShopOrder.UpdateOrderInfo(orderList, addressid, remark, isziti, paytype);

                if (paytype == "余额")
                {
                    //计算处理
                    DB.ShopOrder.Calcute(curUser, orderList.First(), payPwd);
                    CookieHelper.ClearCookie("orderlist");
                }
                else if (paytype == "支付宝")
                {
                    json.ReUrl = "/Member_Mall/Pay/Pay?OrderId=" + orderList.First();//http://tjyy.fabeisha.cn
                    //r.ReUrl = "http://tjyy.fabeisha.cn/Member_Mall/Pay/Pay?BillId=" + r.BillId;//http://www.738600.cn
                }
                else if (paytype == "微信")
                {
                    var orderModel = DB.ShopOrder.FindEntity(orderList.First());
                    H5Pay h5Pay = new H5Pay();
                    var wxConfig = WxPayConfig.GetConfig();
                    string clientIP = wxConfig.GetIp();//获取客户端真实IP     
                    var url = h5Pay.GetPayUrl(clientIP, orderModel.OrderCode, (orderModel.RealAmount + orderModel.Postage.Value) * 100M);//通过统一下单接口进行H5支付    
                                                                                                                                         //Response.Redirect(url);//跳转到微信支付中间页     
                    json.ReUrl = url;
                    //json.ReUrl = "/Member_Mall/Pay/WXPay?OrderId=" + orderList.First();//http://tjyy.fabeisha.cn
                    //r.ReUrl = "/Member_Mall/Pay/WXPay?BillId=" + r.BillId;//http://tjyy.fabeisha.cn
                }

                //3.清空cookie
                CookieHelper.ClearCookie("cart");
                CookieHelper.ClearCookie("total");

                CookieHelper.ClearCookie("traceno");
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
                json.IsSuccess = false;
                json.Msg = str.ToString();

                str.AppendLine("跟踪：" + ex.StackTrace);
                LogHelper.Debug(str.ToString());

            }

            return Json(json);
        }

        public ActionResult IsFu(string orderid)
        {
            try
            {
                ShopOrder model = DB.ShopOrder.FindEntity(orderid);
                if (model == null)
                    throw new Exception("获取订单信息失败");
                if (model.State == 2)
                {
                    return Error("2");
                }
                else
                {
                    return Error("1");
                }
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        /// <summary>
        /// 取消订单，在发货之前
        /// </summary>
        /// <returns></returns>
        public ActionResult DoCancel(string orderid)
        {
            try
            {
                ShopOrder model = DB.ShopOrder.FindEntity(orderid);
                if (model == null)
                    throw new Exception("获取订单信息失败");
                model.State = ShopEnum.OrderState.Cancel.GetHashCode();
                model.CancelTime = DateTime.Now;
                DB.ShopOrder.Update(model);
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        public ActionResult UpdateImg(string img)
        {
            try
            {
                Member_Info model = DB.Member_Info.FindEntity(CurrentUser.MemberId);


                model.AppellationLeveName = img;
                DB.Member_Info.Update(model);
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        public ActionResult DoCancelTH(string orderid)
        {
            try
            {
                ShopOrder model = DB.ShopOrder.FindEntity(orderid);
                if (model == null)
                    throw new Exception("获取订单信息失败");
                model.State = ShopEnum.OrderState.Refund.GetHashCode();
                model.CancelTime = DateTime.Now;
                DB.ShopOrder.Update(model);
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        /// <summary>
        /// 订单地址，管理
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Address(int? id)
        {
            return View();
        }
        #endregion

        #region 确认收货，评论
        /// <summary>
        /// 提货
        /// </summary>
        /// <returns></returns>
        public ActionResult TiHuo(string guid)
        {
            ViewBag.guid = guid;
            return View();
        }
        public ActionResult TiHuoOrder(string pwd2, string guid)
        {
            try
            {  //1.验证支付密码
                Member_Info curUser = User_Shop.GetMember_Info();
                if (curUser.Pwd2 != Common.CryptHelper.DESCrypt.Encrypt(pwd2))
                    throw new Exception("支付密码不正确");
                ShopOrder model = DB.ShopOrder.FindEntity(q => q.GUID == guid);
                model.State = ShopEnum.OrderState.Pay.GetHashCode();
                model.Type = "提货";
                if (DB.ShopOrder.Update(model))
                {
                    return Success("操作成功");
                }
                return Error("操作失败");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        /// <summary>
        /// 委托
        /// </summary>
        /// <returns></returns>
        public ActionResult WeiTuo(string guid)
        {
            ViewBag.guid = guid;
            return View();
        }
        /// <summary>
        /// 标记订单，挂卖
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public ActionResult WeiTuoOrder(string pwd2, string guid)
        {
            try
            {  //1.验证支付密码
                Member_Info curUser = User_Shop.GetMember_Info();
                if (curUser.Pwd2 != Common.CryptHelper.DESCrypt.Encrypt(pwd2))
                    throw new Exception("支付密码不正确");
                ShopOrder model = DB.ShopOrder.FindEntity(q => q.GUID == guid);
                #region 挂卖
                //给零售区加库存
                //var lingshouqu = DB.ShopOrderProduct.Where(p => p.OrderID == model.GUID).FirstOrDefault();
                //var shopOrderProudct = DB.ShopProduct.Where(p => p.Title == lingshouqu.Name && p.CategoryID1 == 15).FirstOrDefault();
                //shopOrderProudct.Inventory = Convert.ToInt32(shopOrderProudct.Inventory + model.ZongDay);
                //DB.ShopProduct.Update(shopOrderProudct);
                model.State = ShopEnum.OrderState.GuaM.GetHashCode();
                model.Type = "挂卖中";
                model.SubmitTime = DateTime.Now;//挂卖时间
                DB.ShopOrder.Update(model);
                #endregion

                return Success("操作成功");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        /// <summary>
        /// 确认收货
        /// </summary>
        /// <returns></returns>
        public ActionResult TakeGoods(string guid)
        {
            ViewBag.guid = guid;
            return View();
        }
        /// <summary>
        /// 确认收货操作
        /// </summary>
        /// <returns></returns>
        public ActionResult DoTakeGoods(string pwd2, string guid)
        {
            try
            {
                //1.验证支付密码
                Member_Info curUser = User_Shop.GetMember_Info();
                if (curUser.Pwd2 != Common.CryptHelper.DESCrypt.Encrypt(pwd2))
                    throw new Exception("支付密码不正确");

                //2.订单进入，待评价/已收货状态 
                ShopOrder model = DB.ShopOrder.FindEntity(guid);
                model.State = ShopEnum.OrderState.Finish.GetHashCode(); //确认收货
                model.FinishTime = DateTime.Now;
                DB.ShopOrder.Update(model);
                ////3.结算系统处理
                //DB.ShopOrder.OrderSettlement(guid);
                //if (model.OrderType != "积分优惠券")
                //{
                //    DB.Jiang.GiveJiang(DB.Member_Info.FindEntity(model.MemberID), model);
                //}
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        /// <summary>
        /// 订单评论
        /// </summary>
        /// <returns></returns>
        public ActionResult BillComment()
        {
            return View();
        }
        /// <summary>
        /// 订单评论，提交
        /// </summary>
        /// <returns></returns>
        public ActionResult DoComment(string guid, int productid, int score, string description)
        {
            //订单进入，已完成状态
            try
            {
                ShopOrderComment model = new ShopOrderComment();
                model.ProductID = productid;
                model.OrderID = guid;
                model.CreateTime = DateTime.Now;
                model.MemberID = CurrentUserID;
                model.Score = score;
                model.Description = description;

                if (DB.ShopOrderComment.Insert(model) == false)
                    throw new Exception("添加评论失败");
                return Success("评论成功");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        #endregion
        #endregion
    }
}