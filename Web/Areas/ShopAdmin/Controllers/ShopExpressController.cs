using Business;
using Common;
using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.ShopAdmin.Controllers
{
    /// <summary>
    /// 快递公司管理
    /// </summary>
    public class ShopExpressController : Web.Controllers.AdminBaseController
    {
        // GET: ShopAdmin/ShopExpress
        public ActionResult Index()
        {
            return View();
        }

        #region 查询
        public string getDataSource(string key, int start, int length, int draw)
        {
            var query = DB.ShopExpress.Where(a => string.IsNullOrEmpty(key) ? true : a.Name.Contains(key))
                 .Select(a => new
                 {
                     a.ID,
                     a.Code,
                     a.Description,
                     a.HomePage,
                     IsEnable = a.IsEnable == true ? "是" : "否",
                     a.Name,
                     a.Sort,
                     a.Postage
                 });
            var total = query.Count();
            var list = query.OrderBy(a => a.Sort).Skip(start).Take(length).ToList();

            return ToPage(list, total, start, length, draw);
        }
        #endregion

        #region 编辑视图
        public ActionResult Detail(int? id)
        {
            ShopExpress entity = null;          
            if (id == null)
            {
                entity = new ShopExpress();
            }
            else
            {
                entity = DB.ShopExpress.FindEntity(id);
            }

            return View(entity);
        }
        #endregion

        #region 保存
        public ActionResult Save(ShopExpress entity)
        {
            var json = new JsonHelp();
            try
            {
                if (entity.ID == 0)
                {
                    json.IsSuccess = DB.ShopExpress.Insert(entity);
                    json.Msg = "添加";
                }
                else
                {
                    json.IsSuccess = DB.ShopExpress.Update(entity);
                    json.Msg = "修改";
                }
                if (json.IsSuccess)
                {
                    json.ReUrl = ControllerPath + "/Index";   //注册成功就跳转到 激活页
                    json.Msg += "成功";
                }
                else
                {
                    json.Msg += "失败";
                }
            }
            catch (Exception e)
            {
                json.IsSuccess = false;
                json.Msg = "操作失败";
                LogHelper.Error("保存快递公司失败：" + WebTools.getFinalException(e));
            }
            return Json(json);
        }
        #endregion

        #region 删除
        public JsonResult Delete(string idList)
        {
            JsonHelp json = new JsonHelp() { Msg = "删除数据失败" };
            //是否为空
            if (string.IsNullOrEmpty(idList))
            {
                json.Msg = "未找到要删除的数据";
                return Json(json);
            }
            var ids = idList.TrimEnd(',').Split(',').Select(a => Convert.ToInt32(a)).ToList();
            if (DB.ShopExpress.Any(a => ids.Contains(a.ID)))
            {
                var names = DB.ShopExpress.Where(a => ids.Contains(a.ID)).Select(a => a.Name)
                    .ToList().Aggregate((m, n) => m + "," + n);
                if (DB.ShopExpress.Delete(a => ids.Contains(a.ID)) > 0)
                {
                    json.IsSuccess = true;
                    json.Msg = "删除数据成功";
                }
                //添加操作日志
                DB.SysLogs.setAdminLog("Delete", "删除名字为[" + names + "]的快递公司 ");
            }
            else
            {
                json.Msg = "未找到要删除的对象，请刷新页面重试";
            }
            return Json(json);
        }
        #endregion
    }
}