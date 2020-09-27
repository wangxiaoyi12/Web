using Business;
using Common;
using EntityFramework.Extensions;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.ShopAdmin.Controllers
{
    /// <summary>
    /// 商家管理
    /// </summary>
    public class ShopController : Web.Controllers.AdminBaseController
    {
        // GET: ShopAdmin/Shop
        public ActionResult Index()
        {
            return View();
        }
        #region 查询
        public string getDataSource(string key, int start, int length, int draw)
        {
            var query = DB.Shop.Where(a => string.IsNullOrEmpty(key) ? true : (a.ShopName.Contains(key) || a.NickName.Contains(key) || a.MemberCode.Contains(key)))
                 .Select(a => new
                 {
                     a.ID,
                     IsEnable = a.IsEnable == true ? "开启" : "停止",
                     IsCheck = a.IsCheck == true ? "已审" : "未审",
                     a.Address,
                     a.CheckTime,
                     a.CreateTime,
                     a.Logo,
                     a.MemberCode,
                     a.NickName,
                     a.QQ,
                     a.ShopName,
                     a.Sort,
                     a.Tel
                 });
            var total = query.Count();
            var list = query.OrderBy(a => a.Sort).Skip(start).Take(length).ToList();

            return ToPage(list, total, start, length, draw);
        }
        #endregion

        #region 编辑视图
        public ActionResult Detail(int? id)
        {
            DataBase.Shop entity = null;
            if (id == null)
            {
                entity = new DataBase.Shop() { IsEnable = true };
            }
            else
            {
                entity = DB.Shop.FindEntity(id);
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
                    var member = DB.Member_Info.FindEntity(a => a.Code == entity.MemberCode);
                    if (member != null)
                    {
                        entity.MemberID = member.MemberId;
                        entity.NickName = member.NickName;
                    }
                    else
                    {
                        json.Msg = "会员编号不存在";
                        return Json(json);
                    }
                    entity.CreateTime = DateTime.Now;
                    json.IsSuccess = DB.Shop.Insert(entity);
                    json.Msg = "添加";
                }
                else
                {
                    var old = DB.Shop.FindEntity(entity.ID);
                    entity.IsCheck = old.IsCheck;
                    entity.IsEnable = old.IsEnable;
                    WebTools.CopyToObject(entity, old);

                    json.IsSuccess = DB.Shop.Update(old);
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
                LogHelper.Error("保存商家失败：" + WebTools.getFinalException(e));
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
            if (DB.Shop.Any(a => ids.Contains(a.ID)))
            {
                var names = DB.Shop.Where(a => ids.Contains(a.ID)).Select(a => a.ShopName)
                    .ToList().Aggregate((m, n) => m + "," + n);
                if (DB.Shop.Delete(a => ids.Contains(a.ID)) > 0)
                {
                    json.IsSuccess = true;
                    json.Msg = "删除数据成功";
                }
                //添加操作日志
                DB.SysLogs.setAdminLog("Delete", "删除名字为[" + names + "]的商家 ");
            }
            else
            {
                json.Msg = "未找到要删除的对象，请刷新页面重试";
            }
            return Json(json);
        }
        #endregion

        #region 审核
        public JsonResult Check(string id, int n)
        {
            JsonHelp json = new JsonHelp() { Msg = "审核商家失败" };
            var flag = "审核通过";
            var ischeck = true;
            if (n != 1)
            {
                flag = "取消审核";
                ischeck = false;
            }
            //是否为空
            if (string.IsNullOrEmpty(id))
            {
                json.Msg = "未找到要操作的数据";
                return Json(json);
            }
            var ids = id.TrimEnd(',').Split(',').Select(a => Convert.ToInt32(a)).ToList();
            if (DB.Shop.Any(a => ids.Contains(a.ID)))
            {
                var names = DB.Shop.Where(a => ids.Contains(a.ID)).Select(a => a.ShopName)
                    .ToList().Aggregate((m, x) => m + "," + x);
                if (DB.Shop.Where(a => ids.Contains(a.ID)).Update(a => new DataBase.Shop() { IsCheck = ischeck, CheckTime = DateTime.Now }) > 0)
                {
                    json.IsSuccess = true;
                    json.Msg = flag + " 商家成功";
                }
                //添加操作日志
                DB.SysLogs.setAdminLog("Check", flag + "名字为[" + names + "]的商家 ");
            }
            else
            {
                json.Msg = "未找到要操作的商家，请刷新页面重试";
            }
            return Json(json);
        }
        #endregion
    }
}