using System;
using System.Web.Mvc;
using Business;

namespace Web.Areas.Admin_BasicSettings.Controllers
{
    public class DepartmentController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_BasicSettings/Department
        private string type = "部门";
        public ActionResult Index()
        { 
            return View(new DataBase.Sys_BasicData());
        }

        /// <summary>
        /// 加载数据列表 
        /// </summary>
        /// <returns></returns>
        public string getDataSource()
        {
            return JsonHelp.ConvertToTablesJson(DB.Sys_BasicData.getBasicDataByType(type));
        }

        /// <summary>
        /// 获得部门对象
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public string getBasicDataById(int id)
        {
            return Common.JsonConverter.Serialize(DB.Sys_BasicData.FindEntity(id));
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [Web.Controllers.Permissions]
        public ActionResult Save(DataBase.Sys_BasicData DataBase)
        {
            return Json(DB.Sys_BasicData.Save(DataBase, Convert.ToInt32(Common.SysDictionary.BasicType.Department)));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        [Web.Controllers.Permissions]
        public ActionResult Delete(string idList)
        {
            return Json(DB.Sys_BasicData.Delete(idList));
        }
        /// 导出excel
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="end"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public FileResult ToExcel()
        {
            var list = DB.Sys_BasicData.getBasicDataByType(type);
            return base.ToExcel(list);
        }
    }
}