using Common;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Business;

namespace Web.Areas.Admin_BasicSettings.Controllers
{
    public class DataBaseManageController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_BasicSettings/DataBaseManage
        private string Before_Restore = "Before_Restore_";
        public ActionResult Index()
        {
            var path = Server.MapPath("/backup/");
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            return View();
        }

        #region getDataSource 加载数据列表
        public string getDataSource()
        {
            var path = Server.MapPath("/backup/");
            var name = DataBaseHelper.GetDbName();
            var autoName = name + "_backup";
            //每天自动备份命名:DBName+"_backup_"+"日期"
            ///过滤掉还原前的自动备份的文件（看不见就不会删除了），以防止万一还原错了，没法还原回还原前的状态
            var list = FileOperate.GetDirectoryFiles(path, "bak").Select(a => new { Name = a.Name, CreationTime = a.CreationTime })
                .Where(a => (a.Name.StartsWith(Before_Restore) == false) && a.Name.StartsWith(autoName) == false)
                .OrderByDescending(a => a.CreationTime).ToList();
            return JsonHelp.ConvertToTablesJson(list);
        }
        #endregion

        #region 备份
        public JsonResult BackupDataBase()
        {
            var json = new JsonHelp(true, "备份成功");
            try
            {
                DataBaseHelper.CreateBackup();
                DB.SysLogs.setAdminLog(Enums.EventType.Backup, "数据库备份成功");
            }
            catch (Exception e)
            {
                json.IsSuccess = false;
                LogHelper.Error("备份数据库出错：" + e.Message);
            }
            return Json(json);
        }
        #endregion

        #region 还原
        public JsonResult RestoreDataBase(string name)
        {
            var json = new JsonHelp() { Status = "n", Msg = "操作失败" };
            try
            {
                #region 还原前先备份               
                //DataBaseHelper.CreateBackup(Before_Restore);
                #endregion

                #region 还原 
                DataBaseHelper.Restore(name);
                #endregion

                #region 还原后刷新缓存
                DB.RefreshCache();
                #endregion

                json.Status = "y";
                json.Msg = "操作成功";
                DB.SysLogs.setAdminLog(Enums.EventType.Backup, "数据库还原成功");
            }
            catch (Exception e)
            {
                LogHelper.Error("还原数据库出错：" + e.Message);
                json.Msg = e.Message;
            }
            return Json(json);
        }
        #endregion

        #region 清空
        /// <summary>
        /// 清空数据库
        /// </summary>
        /// <returns></returns>
        public JsonResult ClearDataBase()
        {
            JsonHelp json = new JsonHelp(true, "清空数据成功");
            try
            {
                #region 先备份               
                DataBaseHelper.CreateBackup(Before_Restore + "clear_");
                #endregion
                DB.Sys_Level.ExecuteProc("proc_ClearData");
                DB.SysLogs.setAdminLog(Enums.EventType.Backup, "数据库清空成功");
            }
            catch (Exception e)
            {
                json.IsSuccess = false;
                LogHelper.Error("清空数据出错：" + e.Message);
            }
            return Json(json);
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>        
        public ActionResult Delete(string idList)
        {
            var json = new JsonHelp() { Status = "n", Msg = "删除失败" };
            try
            {
                var list = idList.Split(',');
                foreach (var item in list)
                {
                    var path = Server.MapPath("/backup/" + item);
                    FileOperate.DelFile(path);
                }
                json.Status = "y";
                json.Msg = "删除成功";
                DB.SysLogs.setAdminLog(Enums.EventType.Backup, "数据库删除成功[" + idList + "]");
            }
            catch (Exception e)
            {
                LogHelper.Error("删除数据库出错：" + e.Message);
            }
            return Json(json);
        }
        #endregion


        #region 分红奖发放
        /// <summary>
        /// 分红奖发放
        /// </summary>
        /// <returns></returns>
        public JsonResult FenHong()
        {
            JsonHelp json = new JsonHelp(true, "分红奖发放");
            try
            {
                TaskTime t = new TaskTime();
                t.FenHong();
            }
            catch (Exception e)
            {
                json.IsSuccess = false;
                LogHelper.Error("分红奖发放：" + e.Message);
            }
            return Json(json);
        }
        #endregion

        #region 突出贡献奖
        /// <summary>
        /// 突出贡献奖
        /// </summary>
        /// <returns></returns>
        public JsonResult TuChu()
        {
            JsonHelp json = new JsonHelp(true, "突出贡献奖发放");
            try
            {
                TaskTime t = new TaskTime();
                t.TuChu();
            }
            catch (Exception e)
            {
                json.IsSuccess = false;
                LogHelper.Error("突出贡献奖发放：" + e.Message);
            }
            return Json(json);
        }
        #endregion

        #region 奖金发放
        /// <summary>
        /// 奖金发放
        /// </summary>
        /// <returns></returns>
        public JsonResult FaFang()
        {
            JsonHelp json = new JsonHelp(true, "奖金发放");
            try
            {
                TaskTime t = new TaskTime();
                t.FaFang();
            }
            catch (Exception e)
            {
                json.IsSuccess = false;
                LogHelper.Error("奖金发放：" + e.Message);
            }
            return Json(json);
        }
        #endregion
    }
}