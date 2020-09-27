using Business;
using System;
using System.Linq;
using System.Web.Mvc;
using DataBase;

namespace Web.Areas.Admin_BasicSettings.Controllers
{
    public class RoleManageController : Web.Controllers.AdminBaseController
    {
        // GET: BasicSettings/RoleManage
        public ActionResult Index()
        {
            return View();
        }
      
        public ActionResult Detail(int? id)
        {
            ViewBag.Title = "编辑";
            if (id == null)
            {
                ViewBag.Title = "新建";
                return View(new DataBase.Sys_Role());
            }
            return View(DB.Sys_Role.FindEntity(id));
        }

        #region 查询 
        /// <summary>
        /// 获得数据源
        /// </summary>
        /// <returns></returns>
        public string getDataSource()
        {
            var data = DB.Sys_Role.Where().ToList();
            return ToPage(data);
        }

        /// <summary>
        /// 获得角色权限
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns></returns>
        public string getRoleNav(int id)
        {
            return DB.Sys_Role.getRoleNavToJson(id);
        }
        #endregion
        
        #region 保存
        [Web.Controllers.Permissions]
        public ActionResult Save(int id, string name, byte roletype, string rolenav)
        {
            return Json(DB.Sys_Role.Save(id, name, roletype, rolenav));
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除（可批量）
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        [Web.Controllers.Permissions]
        public ActionResult Delete(string idList)
        {
            return Json(DB.Sys_Role.Delete(idList));
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
        public FileResult ToExcel(DateTime? startTime, DateTime? end, string key)
        {
            var list = Common.DataTableHelp.ToDataTable(DB.Sys_Role.Where().Select(a => new { role_name = a.role_name, role_type = a.role_type == 1 ? "超级用户" : "系统用户" }).ToList());
            return base.ToExcel(list);
        }
        #endregion
    }
}