using Business;
using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.Admin_Finance.Controllers
{
    public class AllocationController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_Finance/Allocation
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Detail()
        {
            var model = new Allocation_Info();
            var code = "FH" + DateTime.Now.ToString("yyMMddHHmmss") + DB.Random.Next(1000, 9999).ToString();
            model.Code = code;
            model.StartTime = DB.Allocation_Info.Where().Select(a => a.EndTime).OrderByDescending(a => a).FirstOrDefault();

            return View(model);
        }

        public string getDataSource(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.Allocation_Info.getDataSource(startTime, end, key, out total, start, length);
            return ToPage(list, total, start, length, draw, "yyyy-MM-dd");
        }
        /// <summary>
        /// 计算分红
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public JsonResult JSFH(DateTime end)
        {
            var start = new DateTime(2000, 1, 1);
            var last = DB.Allocation_Info.Where().Select(a => a.EndTime).OrderByDescending(a => a).FirstOrDefault();
            if (last != null)
            {
                start = last.Value.AddDays(1);
            }

            //计算分红
            var result = DB.Allocation_Info.JSFH(start, end);
            return Json(result);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ActionResult Save(Allocation_Info entity)
        {
            entity.CreateTime = DateTime.Now;
            entity.CreateEmpId = CurrentUser.Id;
            entity.CreateEmpName = CurrentUser.Name;
            var json = DB.Allocation_Info.Save(entity);
            if (json.IsSuccess)
            {
                json.ReUrl = ControllerPath + "/Index";
            }
            return Json(json);
        }
    }
}