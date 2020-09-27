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
    /// 幻灯片管理
    /// </summary>
    public class ShopSlideController : Web.Controllers.AdminBaseController
    {
        // GET: ShopAdmin/ShopSlide
        public ActionResult Index()
        {
            return View();
        }
        #region 查询
        public string getDataSource(string key, int start, int length, int draw)
        {
            var query = DB.ShopSlide.Where(a => string.IsNullOrEmpty(key) ? true : a.Title.Contains(key))
                 .Select(a => new
                 {
                     a.ID,
                     a.Img,
                     a.Description,
                     a.Title,
                     a.Link,
                     a.Sort,
                     a.Type,
                     a.Video
                 });
            var total = query.Count();
            var list = query.OrderBy(a => a.Sort).Skip(start).Take(length).ToList();

            return ToPage(list, total, start, length, draw);
        }
        #endregion

        #region 编辑视图
        public ActionResult Detail(int? id)
        {
            ShopSlide entity = null;
           
            if (id == null)
            {
                entity = new ShopSlide();
            }
            else
            {
                entity = DB.ShopSlide.FindEntity(id);
            }

            return View(entity);
        }
        #endregion

        #region 保存
        public ActionResult Save(ShopSlide entity)
        {
            var json = new JsonHelp();
            try
            {
                if (entity.ID == 0)
                {
                    json.IsSuccess = DB.ShopSlide.Insert(entity);
                    json.Msg = "添加";
                }
                else
                {
                    json.IsSuccess = DB.ShopSlide.Update(entity);
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
                LogHelper.Error("保存幻灯片失败：" + WebTools.getFinalException(e));
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
            if (DB.ShopSlide.Any(a => ids.Contains(a.ID)))
            {
                var names = DB.ShopSlide.Where(a => ids.Contains(a.ID)).Select(a => a.Title)
                    .ToList().Aggregate((m, n) => m + "," + n);
                if (DB.ShopSlide.Delete(a => ids.Contains(a.ID)) > 0)
                {
                    json.IsSuccess = true;
                    json.Msg = "删除数据成功";
                }
                //添加操作日志
                DB.SysLogs.setAdminLog("Delete", "删除名字为[" + names + "]的幻灯片 ");
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