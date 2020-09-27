using Business;
using Common;
using DataBase;
using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.ShopAdmin.Controllers
{
    /// <summary>
    /// 大转盘管理
    /// </summary>
    public class BigWheelController : Web.Controllers.AdminBaseController
    {
        // GET: ShopAdmin/ShopBigWheel
        public ActionResult Index()
        {
            return View();
        }
        #region 查询
        public string getDataSource(string key, int start, int length, int draw)
        {
            var query = DB.ShopBigWheel.Where(a => a.IsDel != true 
            && (string.IsNullOrEmpty(key) ? true : a.Title.Contains(key)))
                 .Select(a => new
                 {
                     a.ID,
                     IsEnable = a.IsEnable == true ? "启用" : "停用",
                     a.Code,
                     a.CreateTime,
                     a.Desc,
                     a.EndTime,
                     a.StartTime,
                     a.Title
                 });
            var total = query.Count();
            var list = query.OrderByDescending(a => a.CreateTime).Skip(start).Take(length).ToList();

            return ToPage(list, total, start, length, draw);
        }
        #endregion

        #region 编辑视图
        public ActionResult Detail(int? id)
        {
            ShopBigWheel entity = null;

            if (id == null)
            {
                entity = new ShopBigWheel();
            }
            else
            {
                entity = DB.ShopBigWheel.FindEntity(id);
            }

            return View(entity);
        }
        #endregion

        #region 保存
        public ActionResult Save(ShopBigWheel entity)
        {
            var json = new JsonHelp();
            try
            {
                if (entity.ID == 0)
                {
                    entity.CreateTime = DateTime.Now;
                    json.IsSuccess = DB.ShopBigWheel.Insert(entity);
                    json.Msg = "添加";
                }
                else
                {
                    json.IsSuccess = DB.ShopBigWheel.Update(entity);
                    json.Msg = "修改";
                }
                if (json.IsSuccess)
                {
                    if (entity.IsEnable == true)
                    {
                        //更新其他的，停用 
                        var i = DB.ShopBigWheel.Where(a => a.ID != entity.ID).Update(a => new ShopBigWheel() { IsEnable = false });
                    }
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
                LogHelper.Error("保存大转盘失败：" + WebTools.getFinalException(e));
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
            if (DB.ShopBigWheel.Any(a => ids.Contains(a.ID)))
            {
                var names = DB.ShopBigWheel.Where(a => ids.Contains(a.ID)).Select(a => a.Title)
                    .ToList().Aggregate((m, n) => m + "," + n);
                if (DB.ShopBigWheel.Where(a => ids.Contains(a.ID)).Update(a => new ShopBigWheel() { IsDel = true }) > 0)
                {
                    json.IsSuccess = true;
                    json.Msg = "删除数据成功";
                }
                //添加操作日志
                DB.SysLogs.setAdminLog("Delete", "删除名字为[" + names + "]的大转盘 ");
            }
            else
            {
                json.Msg = "未找到要删除的对象，请刷新页面重试";
            }
            return Json(json);
        }
        #endregion

        #region 奖项设置
        public ActionResult BigDetail(int id)
        {
            //如果没有，则新建四个奖
            if (DB.ShopBigWheelDetail.Any(a => a.BID == id) == false)
            {
                var names = new string[] { "一等奖", "二等奖", "三等奖", "四等奖" };
                List<ShopBigWheelDetail> list = new List<ShopBigWheelDetail>();
                for (int i = 0; i < names.Length; i++)
                {
                    var j1 = new ShopBigWheelDetail()
                    {
                        BID = id,
                        Name = names[i],
                        Probability = 0.1m,
                        Sort = i + 1
                    };
                    list.Add(j1);
                }
                DB.ShopBigWheelDetail.Insert(list);
            }
            ViewBag.List = DB.ShopBigWheelDetail.Where(a => a.BID == id).ToList();
            ViewBag.BID = id;
            return View();
        }
        public ActionResult SaveDetail(string strList, int bid)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "保存失败" };

            List<ShopBigWheelDetail> LevList = new List<ShopBigWheelDetail>();
            strList = strList.Substring(0, strList.Length - 1);
            var tr = strList.Split('&');
            for (int i = 0; i < tr.Length; i++)
            {
                var data = tr[i].Split(',');
                ShopBigWheelDetail entity = DB.ShopBigWheelDetail.FindEntity(Convert.ToInt32(data[0]));
                entity.Name = data[1];
                entity.Desc = data[2];
                entity.Sort = Convert.ToInt32(data[3]);
                entity.Probability = Convert.ToDecimal(data[4]);
                LevList.Add(entity);
            }
            var result = DB.ShopBigWheelDetail.Update(LevList);
            if (result > 0)
            {
                json.Status = "y";
                json.Msg = "保存成功";
                //添加操作日志
                DB.SysLogs.setAdminLog("Edit", "修改了大转盘奖项");
            }

            return Json(json);
        }
        #endregion

        #region 抽奖记录
        public ActionResult BigLog(int id)
        {
            ViewBag.ID = id;
            return View();
        }
        public string getLogDataSource(int id, string key, int start, int length, int draw)
        {
            var query = DB.ShopBigWheelLog.Where(a => a.BID == id
                && (string.IsNullOrEmpty(key) ? true : (a.MemberCode.Contains(key) || a.NickName.Contains(key) || a.Result.Contains(key))))
                 .Select(a => new
                 {
                     a.ShopBigWheel.Title,
                     a.ID,
                     a.CreateTime,
                     a.Desc,
                     a.MemberCode,
                     a.NickName,
                     a.Remark,
                     a.Result,
                     a.ResultID
                 });
            var total = query.Count();
            var list = query.OrderByDescending(a => a.CreateTime).Skip(start).Take(length).ToList();

            return ToPage(list, total, start, length, draw);
        }
        #endregion
    }
}