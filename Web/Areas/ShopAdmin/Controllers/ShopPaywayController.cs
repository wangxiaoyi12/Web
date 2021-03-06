﻿using Business;
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
    /// 支付方式
    /// </summary>
    public class ShopPaywayController : Web.Controllers.AdminBaseController
    {
        // GET: ShopAdmin/ShopPayway
        public ActionResult Index()
        {
            return View();
        }

        #region 查询
        public string getDataSource(string key, int start, int length, int draw)
        {
            var query = DB.ShopPayWay.Where(a => string.IsNullOrEmpty(key) ? true : a.PayWay.Contains(key))
                 .Select(a => new
                 {
                     a.ID,
                     IsDefault = a.IsDefault == true ? "是" : "否",
                     a.PayWay,
                     a.Sort
                 });
            var total = query.Count();
            var list = query.OrderBy(a => a.Sort).Skip(start).Take(length).ToList();

            return ToPage(list, total, start, length, draw);
        }
        #endregion

        #region 编辑视图
        public ActionResult Detail(int? id)
        {
            ShopPayWay entity = null;
            if (id == null)
            {
                entity = new ShopPayWay();
            }
            else
            {
                entity = DB.ShopPayWay.FindEntity(id);
            }

            return View(entity);
        }
        #endregion

        #region 保存
        public ActionResult Save(ShopPayWay entity)
        {
            var json = new JsonHelp();
            try
            {
                if (entity.ID == 0)
                {
                    json.IsSuccess = DB.ShopPayWay.Insert(entity);
                    json.Msg = "添加";
                }
                else
                {
                    json.IsSuccess = DB.ShopPayWay.Update(entity);
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
                LogHelper.Error("保存支付方式失败：" + WebTools.getFinalException(e));
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
            if (DB.ShopPayWay.Any(a => ids.Contains(a.ID)))
            {
                var names = DB.ShopPayWay.Where(a => ids.Contains(a.ID)).Select(a => a.PayWay)
                    .ToList().Aggregate((m, n) => m + "," + n);
                if (DB.ShopPayWay.Delete(a => ids.Contains(a.ID)) > 0)
                {
                    json.IsSuccess = true;
                    json.Msg = "删除数据成功";
                }
                //添加操作日志
                DB.SysLogs.setAdminLog("Delete", "删除名字为[" + names + "]的支付方式 ");
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