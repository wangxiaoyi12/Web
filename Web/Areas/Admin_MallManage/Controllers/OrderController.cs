using System;
using System.Web.Mvc;
using Business;

namespace Web.Areas.Admin_MallManage.Controllers
{
    public class OrderController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_MallManage/Order
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail(int? id)
        {
            if (id != null)
            {
                return View(DB.Bill_Info.FindEntity(id));
            }
            return View(new DataBase.Product_Info());
        }

        #region 查询
        public string getDataSource(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.Bill_Info.getDataSource(null, startTime, end, key, out total, start, length);

            return ToPage(list, total, start, length, draw);
        }
        #endregion

        #region 删除
        public ActionResult Delete(string idList)
        {
            return Json(DB.Bill_Info.Delete(idList));
        }
        #endregion

        #region 保存
        [ValidateInput(false)]
        public ActionResult Save(DataBase.Bill_Info entity)
        {
            var model = DB.Bill_Info.FindEntity(entity.BillId);
            model.ExpressCode = entity.ExpressCode;
            model.ExpressName = entity.ExpressName;
            model.SendTime = entity.SendTime;
            model.NickName = entity.NickName;
            model.Mobile = entity.Mobile;
            model.PostAddress = entity.PostAddress;
            var json = DB.Bill_Info.Save(model);
            if (json.Status == "y")
            {
                json.ReUrl = ControllerPath + "/Index";
            }
            return Json(json);
        }
        #endregion

        #region 导出excel
        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="end"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public FileResult ToExcel(DateTime? startTime, DateTime? end, string key)
        {
            int total = 0;
            var list = DB.Bill_Info.getDataSource(null, startTime, end, key, out total, 0, int.MaxValue);
            return base.ToExcel(list);
        }
        #endregion
    }
}