using Business;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Web.Areas.Admin_BasicSettings.Controllers
{
    public class UserManageController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_BasicSettings/UserManage         
        public ActionResult Index()
        {
            return View();
        }

        [Web.Controllers.Permissions]
        public ActionResult Detail(string id)
        {
            ViewData["DepartmentData"] = DB.Sys_BasicData.getBasicDataByType("部门");
            ViewData["Jobs"] = DB.Sys_BasicData.getBasicDataByType("岗位");
            ViewData["Roles"] = DB.Sys_Role.Where(a => a.id != 1).ToList();

            ViewBag.Title = "编辑";
            if (id == null)
            {
                ViewBag.Title = "新建";
                return View(new DataBase.Sys_Employee());
            }
            return View(DB.Sys_Employee.FindEntity(id));
        }

        #region 获得列表数据
        public string getDataSource()
        {
            var basicData = DB.Sys_BasicData.Where().ToList();
            var roles = DB.Sys_Role.Where().ToList();
            var data = DB.Sys_Employee.Where().OrderByDescending(p => p.LastLogin).ToList().Select(p => new
            {
                p.EmpId,
                p.LoginName,
                p.RealName,
                p.Sex,
                p.Mobile,
                DepartmentName = basicData.Where(y => y.Id == p.DepartmentId).SingleOrDefault() == null ? "" : basicData.Where(y => y.Id == p.DepartmentId).SingleOrDefault().BasicDataName,
                PositionName = basicData.Where(y => y.Id == p.PositionId).SingleOrDefault() == null ? "" : basicData.Where(y => y.Id == p.PositionId).SingleOrDefault().BasicDataName,
                RoleName = roles.Where(a => a.id == p.RoleId).FirstOrDefault() == null ? "" : roles.Where(a => a.id == p.RoleId).FirstOrDefault().role_name,
                p.LastLogin,
                p.Comment,
                p.EmpState
            }).ToList();
            return ToPage(data);
        }
        #endregion

        #region 保存
        [Web.Controllers.Permissions]
        public ActionResult Save(DataBase.Sys_Employee entity)
        {
            var r = DB.Sys_Employee.Save(entity);
            if (r.IsSuccess)
            {
                r.ReUrl = ControllerPath + "/Index";
            }
            return Json(r);
        }
        #endregion

        #region 删除（可批量）
        [Web.Controllers.Permissions]
        public ActionResult Delete(string idList)
        {
            return Json(DB.Sys_Employee.Delete(idList));
        }
        #endregion

        #region 导出excel
        public FileResult ToExcel()
        {
            var basicData = DB.Sys_BasicData.Where().ToList();
            var roles = DB.Sys_Role.Where().ToList();
            var data = DB.Sys_Employee.Where().OrderByDescending(p => p.LastLogin).ToList().Select(p => new
            {
                p.EmpId,
                p.LoginName,
                p.RealName,
                p.Sex,
                p.Mobile,
                DepartmentName = basicData.Where(y => y.Id == p.DepartmentId).SingleOrDefault().BasicDataName,
                PositionName = basicData.Where(y => y.Id == p.PositionId).SingleOrDefault().BasicDataName,
                RoleName = roles.Where(a => a.id == p.RoleId).FirstOrDefault().role_name,
                p.LastLogin,
                p.Comment,
                p.EmpState
            }).ToList();
            return base.ToExcel(data);
        }
        #endregion 
    }
}