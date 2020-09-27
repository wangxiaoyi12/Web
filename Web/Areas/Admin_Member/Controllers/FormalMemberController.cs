using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Business;
using DataBase;

namespace Web.Areas.Admin_Member.Controllers
{
    public class FormalMemberController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_Member/FormalMember
        public ActionResult Index()
        {
            return View();
        }

        #region Detail 视图
        public ActionResult Detail(string id)
        {
            ViewData["Banks"] = DB.Sys_BasicData.getBasicDataByType("银行");
            ViewData["Level"] = DB.Sys_Level.getList();
            ViewData["Prov"] = DB.Area_Province.ToList();
            var list = new List<Sys_Level>();
            list.Add(new Sys_Level() { LevelId = 0, LevelName = "普通会员" });
            list.AddRange(DB.Sys_Level.getList());
               //list.Add(new ZCInfo() {ZCId=4,ZCIdName = "4,四星合伙人", ZCName = "四星合伙人" });
               //list.Add(new ZCInfo() {ZCId=5,ZCIdName = "5,五星合伙人", ZCName = "五星合伙人" });
               //list.Add(new ZCInfo() {ZCId= 6, ZCIdName = "6,六星合伙人", ZCName = "六星合伙人" });
               ViewData["ZC"] = list;
            var model = DB.Member_Info.FindEntity(id);
            ViewData["City"] = DB.Area_City.ToList(a => a.pid == model.ProvId);
            ViewData["County"] = DB.Area_County.ToList(a => a.cid == model.CityId);
            if (model != null)
            {
                ViewBag.RecommendId = model.RecommendCode;
                //推荐人数
                ViewBag.RCount = DB.Member_Info.Count(a => a.RecommendId == id&&a.MemberLevelId>0);
                return View(model);
            }
            return View(new Member_Info());
        }
        #endregion

        #region 保存个人信息
        public ActionResult SaveMember(Member_Info entity)
        {
            var level = DB.Sys_Level.getList().FirstOrDefault(a => a.LevelId == entity.MemberLevelId);
            var oldM = DB.Member_Info.FindEntity(entity.MemberId);
            var oldLevelName = oldM.MemberLevelName;
            if (level != null)
            {
                entity.MemberLevelName = level.LevelName;
            }
            var addCommission = Convert.ToDecimal(Request["AddCommission"]);
            var addCommissionSum = Convert.ToDecimal(Request["AddCommissionSum"]);
            var addCoins = 0;// Convert.ToDecimal(Request["AddCoins"]);

            var addShopCoins = 0;// Convert.ToDecimal(Request["AddShopCoins"]);
            var addScores = 0;// Convert.ToDecimal(Request["AddScores"]);
            var addTourScores = 0;// Convert.ToDecimal(Request["AddTourScores"]);

            //赋值省市区
            if (entity.ProvId != null)
            {
                entity.ProvName = DB.Area_Province.FindEntity(entity.ProvId).name;
            }
            if (entity.CityId != null)
            {
                entity.CityName = DB.Area_City.FindEntity(entity.CityId).name;
            }
            if (entity.CountyId != null)
            {
                entity.CountyName = DB.Area_County.FindEntity(entity.CountyId).name;
            }
            var NiCheng = Request["NiCheng"];
            var Pwd2 = Request["Pwd2"];
            var json = DB.Member_Info.Save(entity, Enums.LoginType.admin, NiCheng, Pwd2, addCommission, addCommissionSum, addCoins, addShopCoins, addScores, addTourScores);
            if (json.Status == "y")
            {
                //记录操作日志
                if (oldLevelName != entity.MemberLevelName)
                {
                    DB.SysLogs.setAdminLog("Edit", "修改会员[" + oldM.Code + "]级别为[" + entity.MemberLevelName + "]，操作成功");
                }
                if (addCommission != 0)
                {
                    DB.SysLogs.setAdminLog("Edit", "增减会员[" + oldM.Code + "]收益，数量：[" + addCommission + "]，操作成功");
                }
                if (addCommissionSum != 0)
                {
                    DB.SysLogs.setAdminLog("Edit", "增减会员[" + oldM.Code + "]收益累计，数量：[" + addCommissionSum + "]，操作成功");
                }
                if (addCoins != 0)
                {
                    DB.SysLogs.setAdminLog("Edit", "增减会员[" + oldM.Code + "]报单积分，数量：[" + addCoins + "]，操作成功");
                }
                if (addShopCoins != 0)
                {
                    DB.SysLogs.setAdminLog("Edit", "增减会员[" + oldM.Code + "]推广奖，数量：[" + addShopCoins + "]，操作成功");
                }
                if (addScores != 0)
                {
                    DB.SysLogs.setAdminLog("Edit", "增减会员[" + oldM.Code + "]积分，数量：[" + addScores + "]，操作成功");
                }
                if (addTourScores != 0)
                {
                    DB.SysLogs.setAdminLog("Edit", "增减会员[" + oldM.Code + "]商城积分，数量：[" + addTourScores + "]，操作成功");
                }
            }
            return Json(json);
        }
        #endregion

        #region 每日返利
        public ActionResult btnFuMinFenH_Click()
        {
            var json = DB.Jiang.FanLi();
            return Json(json);
        }
        #endregion

        #region 查询
        public JsonResult getMember(string id)
        {
            var m = DB.Member_Info.FindEntity(id);
            return Json(m);
        }
        public string getDataSource(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.Member_Info.getSearchList(null, startTime, end, key, "已激活", out total, start, length)
                .ToList()
                 .Select(a => new
                 {
                     a.ActiveAmount,
                     a.RecommendName,
                     a.Code,
                     a.MemberId,
                     a.Mobile,
                     a.NickName,
                     a.ActiveTime,
                     a.RecommendCode,
                     a.UpperCode,
                     a.CreateTime,
                     a.MemberLevelName,
                     a.Commission,
                     a.AppellationLeveName,
                     a.ZCName,
                     a.Pwd3,
                     TD = a.ActiveAmount+a.RAmount,
                  
                     a.ServiceCenterCode
                 }).ToList();
            return ToPage(list, total, start, length, draw);
        }
        #endregion

        #region 登录前台
        public ActionResult login(string id)
        {
            SysManage.Controllers.AccountController Login = new SysManage.Controllers.AccountController();
            var member = DB.Member_Info.FindEntity(id);

            return Login.Member_login(id);
        }
        #endregion
        #region 删除会员
        public JsonResult Delete(string idList)
        {
            var r = DB.Member_Info.Delete(idList, Enums.LoginType.admin);
            return Json(r);
        }
        #endregion
        #region 注册会员 视图
        public ActionResult Insert(string position, string upid)
        {
            #region 获取相关数据，如会员级别，银行，省等
            ViewData["Level"] = DB.Sys_Level.Where().ToList();
            ViewData["Banks"] = DB.Sys_BasicData.getBasicDataByType("银行");
            ViewData["Prov"] = DB.Area_Province.ToList();
            #endregion

            #region 自动生成Code  HY+N倍随机数字
            //var code = new Random().Next(1000000, 9999999);
            //var exist = DB.Member_Info.Any(a => a.Code == code);
            //while (exist)
            //{
            //    code = new Random().Next(1000000, 9999999);
            //    exist = DB.Member_Info.Any(a => a.Code == code);
            //}
            #endregion    
                      

            return View(new DataBase.Member_Info() {RecommendCode = "admin" });
        }
        #endregion

        #region 注册会员--保存方法
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ActionResult Save(Member_Info DataBase)
        {
            DataBase.CreateMemberId = CurrentUser.Id;
            DataBase.CreateMemberName = CurrentUser.Name;
            DataBase.CreateTime = DateTime.Now;
            //赋值省市区
            if (DataBase.ProvId != null)
            {
                DataBase.ProvName = DB.Area_Province.FindEntity(DataBase.ProvId).name;
            }
            if (DataBase.CityId != null)
            {
                DataBase.CityName = DB.Area_City.FindEntity(DataBase.CityId).name;
            }
            if (DataBase.CountyId != null)
            {
                DataBase.CountyName = DB.Area_County.FindEntity(DataBase.CountyId).name;
            }
            DataBase.Code = DataBase.Code.Trim();
            DataBase.Mobile = DataBase.Mobile;
            var json = DB.Member_Info.Save(DataBase, Enums.LoginType.admin,"","",0,0,0,0);
            if (json.Status == "y")
            {
                json.ReUrl = "/Admin_Member/FormalMember/Index";   //注册成功就跳转到 激活页
            }
            return Json(json);
        }
        #endregion

        #region 重置密码,或支付密码
        public JsonResult ResetPwd(string idList, string PwdType)
        {
            return Json(DB.Member_Info.ResetPwd(idList, PwdType));
        }
        #endregion

        #region 导出
        public FileResult ToExcel(DateTime? startTime, DateTime? end, string key)
        {
            int total = 0;
            var list = DB.Member_Info.getSearchList(null, startTime, end, key, "已激活", out total, 0, int.MaxValue).ToList();
            return base.ToExcel(list);
        }
        #endregion 
    }
}