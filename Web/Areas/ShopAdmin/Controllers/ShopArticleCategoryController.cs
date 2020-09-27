using Business;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.ShopAdmin.Controllers
{
    /// <summary>
    /// 文章类型管理
    /// </summary>
    public class ShopArticleCategoryController : Web.Controllers.AdminBaseController
    {
        // GET: ShopAdmin/ShopArticleCategory
        public ActionResult Index()
        {
            ViewBag.List = DB.ShopArticleCategory.getFrom2Layer();
            return View(new DataBase.ShopArticleCategory());
        }
        #region 查询，加载数据
        /// <summary>
        /// 加载菜单树
        /// </summary>
        /// <returns></returns>
        public string getjsTreeData(string id)
        {
            return DB.ShopArticleCategory.ConvertjsTreeData();
        }

        /// <summary>
        /// 获得菜单对象
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public string getDetail(int id)
        {
            var data = DB.ShopArticleCategory.Where(a => a.ID == id).ToList().Select(a => new
            {
                a.ID,
                a.Name,
                a.Sort,
                a.PID,
                a.IsIndexBottom,
                a.Type,
                a.Layer,
            }).FirstOrDefault();
            return data.ToJsonString();
        }
        #endregion

        #region 保存
        //[Web.Controllers.Permissions]
        public ActionResult Save(DataBase.ShopArticleCategory entity)
        {
            var json = new JsonHelp();
            try
            {
                entity.Layer = 1;
                if (entity.ID == 0)
                {
                    if (entity.PID != 0)
                    {
                        var p1 = DB.ShopArticleCategory.Where(a => a.ID == entity.PID).Select(a => a.Layer).FirstOrDefault();
                        entity.Layer = p1 + 1;
                    }
                    else
                    {
                        entity.PID = null;
                    }
                    json.IsSuccess = DB.ShopArticleCategory.Insert(entity);
                    json.Msg = "添加";
                }
                else
                {
                    if (entity.PID != 0)
                    {
                        var p = DB.ShopArticleCategory.Where(a => a.ID == entity.PID).Select(a => a.Layer).FirstOrDefault();
                        entity.Layer = p + 1;
                    }
                    else
                    {
                        entity.PID = null;
                    }
                    json.IsSuccess = DB.ShopArticleCategory.Update(entity);
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
                LogHelper.Error("保存文章分类失败：" + WebTools.getFinalException(e));
            }
            return Json(json);
        }
        #endregion

        #region 删除 
        public ActionResult Delete(string idList)
        {
            JsonHelp json = new JsonHelp() { Msg = "删除数据失败" };
            //是否为空
            if (string.IsNullOrEmpty(idList))
            {
                json.Msg = "未找到要删除的数据";
                return Json(json);
            }
            var ids = idList.TrimEnd(',').Split(',').Select(a => Convert.ToInt32(a)).ToList();
            if (DB.ShopArticleCategory.Any(a => ids.Contains(a.ID)))
            {
                var names = DB.ShopArticleCategory.Where(a => ids.Contains(a.ID)).Select(a => a.Name)
                    .ToList().Aggregate((m, n) => m + "," + n);
                if (DB.ShopArticleCategory.Delete(a => ids.Contains(a.ID)) > 0)
                {
                    json.IsSuccess = true;
                    json.Msg = "删除数据成功";
                }
                //添加操作日志
                DB.SysLogs.setAdminLog("Delete", "删除名字为[" + names + "]的文章分类 ");
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