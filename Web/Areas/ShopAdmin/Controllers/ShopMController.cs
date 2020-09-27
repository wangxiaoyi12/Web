using Business;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.ShopAdmin.Controllers
{
    public class ShopMController : Web.Controllers.MemberBaseController
    {
        #region 编辑视图
        public ActionResult Index()
        {
            DataBase.Shop entity = DB.Shop.FindEntity(a => a.MemberID == CurrentUser.Id);
            if (entity == null)
            {
                entity = new DataBase.Shop() { IsEnable = true };
                ViewBag.IsCheck = "未开通";
            }
            else
            {
                entity = DB.Shop.FindEntity(entity.ID);
                ViewBag.IsCheck = entity.IsCheck ? "已审核" : "未审核";
            }
            return View(entity);
        }
        #endregion



        #region 保存
        public ActionResult Save(DataBase.Shop entity)
        {
            var json = new JsonHelp();
            try
            {
                if (entity.ID == 0)
                {
                    entity.MemberID = CurrentUser.Id;
                    entity.NickName = CurrentUser.Name;
                    entity.MemberCode = CurrentUser.LoginName;

                    entity.CreateTime = DateTime.Now;
                    json.IsSuccess = DB.Shop.Insert(entity);
                }
                else
                {
                    var model = DB.Shop.FindEntity(entity.ID);
                    WebTools.CopyToObject(entity, model);
                    entity.CheckTime = DateTime.Now;
                    entity.IsCheck = true;
                    json.IsSuccess = DB.Shop.Update(model);
                }
                if (json.IsSuccess)
                {
                    //json.ReUrl = ControllerPath + "/Index";   //注册成功就跳转到 激活页
                    json.Msg = "保存成功";
                }
                else
                {
                    json.Msg = "保存失败";
                }
            }
            catch (Exception e)
            {
                json.IsSuccess = false;
                json.Msg = "操作失败";
                LogHelper.Error("保存商家失败：" + WebTools.getFinalException(e));
            }
            return Json(json);
        }
        #endregion
    }
}