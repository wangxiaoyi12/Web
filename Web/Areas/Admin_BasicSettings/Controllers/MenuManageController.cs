using Business;
using System;
using System.Web.Mvc;

namespace Web.Areas.Admin_BasicSettings.Controllers
{
    public class MenuManageController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_BasicSettings/DB.Sys_Navigation
        public ActionResult Index()
        {
            ViewData["ParentMenu"] = DB.Sys_Navigation.getFirstMenu();
            return View(new DataBase.Sys_Navigation());
        }

        #region 查询，加载数据
        /// <summary>
        /// 加载菜单树
        /// </summary>
        /// <returns></returns>
        public string getjsTreeData()
        {
            return DB.Sys_Navigation.ConvertjsTreeData();
        }

        /// <summary>
        /// 获得菜单对象
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public string getMenuById(int id)
        {
            return Common.JsonConverter.Serialize(DB.Sys_Navigation.getMenuById(id));
        }
        #endregion

        #region 保存
        [Web.Controllers.Permissions]
        public ActionResult Save(DataBase.Sys_Navigation entity)
        {
            return Json(DB.Sys_Navigation.Save(entity));
        }
        #endregion

        #region 删除
        [Web.Controllers.Permissions]
        public ActionResult Delete(string idList)
        {
            return Json(DB.Sys_Navigation.Delete(idList));
        }
        #endregion 

    }
}