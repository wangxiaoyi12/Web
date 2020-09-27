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
    /// 商品类别管理
    /// </summary>
    public class ShopProductCategoryController : Web.Controllers.AdminBaseController
    {
        public string getDataSource()
        {
            var list = DB.GuiGeName.ToList();
            return ToPage(list, 10000000, 0, 0, 0);
        }
        [Web.Controllers.Permissions]
        public ActionResult GSave(DataBase.GuiGeName_Info GuiGeName_Info)
        {
            var entity = DB.GuiGeName.FindEntity(GuiGeName_Info.GId);
            entity.CName = GuiGeName_Info.CName;
            entity.GName = GuiGeName_Info.GName;
            entity.GInfoName = GuiGeName_Info.GInfoName;
            DB.GuiGeName.Update(entity);
            JsonHelp json = new JsonHelp() { Status = "y", Msg = "保存成功" };
            return Json(json);
        }

        public string getGuiGe(int id)
        {
            var entity = DB.GuiGeName.FindEntity(id);
            return Common.JsonConverter.Serialize(entity);
        }
        public JsonResult addguigeinfo(string CName, string GName, string GInfoName)
        {
            JsonHelp json = new JsonHelp() { Msg = "增加数据失败" };

            if (DB.GuiGeName.Any(a => a.CName == CName && a.GName == GName))
            {
                json.Msg = "已有相关规格，请进行编辑！";
                return Json(json);
            }
            GuiGeName_Info gg = new GuiGeName_Info();
            gg.CName = CName;
            gg.CreateTime = DateTime.Now;
            gg.GName = GName;
            gg.GInfoName = GInfoName;
            DB.GuiGeName.Insert(gg);
            json.IsSuccess = true;
            json.Msg = "增加数据成功";
            return Json(json);
        }

        public JsonResult GGDelete(string idList)
        {
            JsonHelp json = new JsonHelp() { Msg = "删除数据失败" };
            //是否为空
            if (string.IsNullOrEmpty(idList))
            {
                json.Msg = "未找到要删除的数据";
                return Json(json);
            }
            var ids = idList.TrimEnd(',').Split(',').Select(a => Convert.ToInt32(a)).ToList();
            if (DB.GuiGeName.Any(a => ids.Contains(a.GId)))
            {
                var names = DB.GuiGeName.Where(a => ids.Contains(a.GId)).Select(a => a.GName)
                    .ToList().Aggregate((m, n) => m + "," + n);
                if (DB.GuiGeName.Delete(a => ids.Contains(a.GId)) > 0)
                {
                    json.IsSuccess = true;
                    json.Msg = "删除数据成功";
                }
                //添加操作日志
                DB.SysLogs.setAdminLog("Delete", "删除名字为[" + names + "]的规格 ");
            }
            else
            {
                json.Msg = "未找到要删除的对象，请刷新页面重试";
            }
            return Json(json);
        }
        public ActionResult CIndex()
        {
            var list = DB.ShopProductCategory.Where(a => a.Layer == 2).GroupBy(a => a.Name).Select(g => (g.Key));

            ViewBag.List1 = list;


            return View(new DataBase.GuiGeName_Info());
        }
        // GET: ShopAdmin/ShopProductCategory
        public ActionResult Index()
        {
            ViewBag.List = DB.ShopProductCategory.getFrom2Layer();
            return View(new DataBase.ShopProductCategory());
        }
        #region 查询，加载数据
        /// <summary>
        /// 加载菜单树
        /// </summary>
        /// <returns></returns>
        public string getjsTreeData(string id)
        {
            return DB.ShopProductCategory.ConvertjsTreeData();
        }

        /// <summary>
        /// 获得菜单对象
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public string getDetail(int id)
        {
            var data = DB.ShopProductCategory.Where(a => a.ID == id).ToList().Select(a => new
            {
                a.Description,
                a.ID,
                a.Name,
                a.Sort,
                a.PID,
                a.Layer,
                a.Logo,
            }).FirstOrDefault();
            return data.ToJsonString();
        }
        #endregion

        #region 保存
        //[Web.Controllers.Permissions]
        public ActionResult Save(DataBase.ShopProductCategory entity)
        {
            var json = new JsonHelp();
            try
            {
                entity.Layer = 1;
                if (entity.ID == 0)
                {
                    if (entity.PID != 0)
                    {
                        var p1 = DB.ShopProductCategory.Where(a => a.ID == entity.PID).Select(a => a.Layer).FirstOrDefault();
                        entity.Layer = p1 + 1;
                    }
                    else
                    {
                        entity.PID = null;
                    }
                    json.IsSuccess = DB.ShopProductCategory.Insert(entity);
                    json.Msg = "添加";
                }
                else
                {
                    if (entity.PID != 0)
                    {
                        var p = DB.ShopProductCategory.Where(a => a.ID == entity.PID).Select(a => a.Layer).FirstOrDefault();
                        entity.Layer = p + 1;
                    }
                    else
                    {
                        entity.PID = null;
                    }
                    json.IsSuccess = DB.ShopProductCategory.Update(entity);
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
                LogHelper.Error("保存产品分类失败：" + WebTools.getFinalException(e));
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

            if(ids.Contains(321))
            {
                json.Msg = "积分专区不可删除";
                return Json(json);
            }

            if (DB.ShopProductCategory.Any(a => ids.Contains(a.ID)))
            {
                var names = DB.ShopProductCategory.Where(a => ids.Contains(a.ID)).Select(a => a.Name)
                    .ToList().Aggregate((m, n) => m + "," + n);
                if (DB.ShopProductCategory.Delete(a => ids.Contains(a.ID)) > 0)
                {
                    json.IsSuccess = true;
                    json.Msg = "删除数据成功";
                }
                //添加操作日志
                DB.SysLogs.setAdminLog("Delete", "删除名字为[" + names + "]的产品分类 ");
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