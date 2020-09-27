using Common;
using System.Web.Mvc;
using Business;
using System;

namespace Web.Areas.Admin_BasicSettings.Controllers
{
    public class EditePwdController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_BasicSettings/EditePwd
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Save(string oldpwd, string newpwd)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "保存失败" };
            if (Common.CryptHelper.DESCrypt.Encrypt(oldpwd) != CurrentUser.PassWord)
            {
                json.Msg = "旧密码不正确！";
                return Json(json);
            }
            if (CurrentUser.LoginType == "admin")
            {
                return Json(DB.Sys_Employee.EditPwd(CurrentUser.Id, newpwd));
            }
            else
            {
                return Json(DB.Member_Info.EditPwd(CurrentUser.Id, newpwd));
            } 
        }
    }
}