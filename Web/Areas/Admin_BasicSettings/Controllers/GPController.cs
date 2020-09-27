using Business;
using Common;
using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.Admin_BasicSettings.Controllers
{
    public class GPController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_BasicSettings/GP
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SaveMember()
        {
            var code = Request["Code"];
            var member = DB.Member_Info.FindEntity(p => p.Code == code);
            var addCommission = Convert.ToDecimal(Request["AddCommission"]);
            var addCommissionSum = Convert.ToDecimal(Request["AddCommissionSum"]);
            var addCoins = Convert.ToDecimal(Request["AddCoins"]);

            var addShopCoins = Convert.ToDecimal(Request["AddShopCoins"]);
            var addScores = Convert.ToDecimal(Request["AddScores"]);
            var addTourScores = Convert.ToDecimal(Request["AddTourScores"]);
            var Pwd2 = Request["Pwd2"];
            JsonHelp data = DB.Member_Info.SaveZeng(Enums.LoginType.admin, code, Pwd2,addCommission, addCommissionSum, addCoins, addShopCoins, addScores, addTourScores);
            if (data.Status == "y")
            {
                if (addCommission != 0)
                {
                    DB.SysLogs.setAdminLog("Edit", "增减会员[" + member.Code + "]收益，数量：[" + addCommission + "]，操作成功");
                }
                if (addCommissionSum != 0)
                {
                    DB.SysLogs.setAdminLog("Edit", "增减会员[" + member.Code + "]收益累计，数量：[" + addCommissionSum + "]，操作成功");
                }
                if (addCoins != 0)
                {
                    DB.SysLogs.setAdminLog("Edit", "增减会员[" + member.Code + "]报单积分，数量：[" + addCoins + "]，操作成功");
                }
                if (addShopCoins != 0)
                {
                    DB.SysLogs.setAdminLog("Edit", "增减会员[" + member.Code + "]推广奖，数量：[" + addShopCoins + "]，操作成功");
                }
                if (addScores != 0)
                {
                    DB.SysLogs.setAdminLog("Edit", "增减会员[" + member.Code + "]配货价，数量：[" + addScores + "]，操作成功");
                }
                if (addTourScores != 0)
                {
                    DB.SysLogs.setAdminLog("Edit", "增减会员[" + member.Code + "]商城积分，数量：[" + addTourScores + "]，操作成功");
                }
            }
            return base.Json(data);
        }

    }
}