using Common;
using System;
using System.Web.Mvc;
using Business;

namespace Web.Areas.Admin_MallManage.Controllers
{
    public class GoodsManageController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_MallManage/GoodsManage
        public ActionResult Index()
        {
            return View();
        }

        #region Detail 视图
        public ActionResult Detail(int? id)
        {
            ViewData["GoodsType"] = DB.Sys_BasicData.getBasicDataByType("商品类别");
            if (id != null)
            {
                return View(DB.Product_Info.FindEntity(id));
            }
            return View(new DataBase.Product_Info());
        }
        #endregion 

        #region 查询
        public string getDataSource(string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.Product_Info.getDataSource(key, start / length + 1, length, out total);
            return ToPage(list, total, start, length, draw);
        }
        #endregion

        #region 删除
        public ActionResult Delete(string idList)
        {
            return Json(DB.Product_Info.Delete(idList));
        }
        #endregion 

        #region 保存
        public string SaveImage()
        {
            return FileUpload.UpLoad("/upload/images/", Request, Server, 1);
        }
        [ValidateInput(false)]
        public ActionResult Save(DataBase.Product_Info entity)
        {
            var r = DB.Product_Info.Save(entity);
            if (r.IsSuccess)
            {
                r.ReUrl = ControllerPath + "/Index";
            }
            return Json(r);
        }
        #endregion 

        #region 导出
        public FileResult ToExcel(DateTime? startTime, DateTime? end, string key)
        {
            int total = 0;
            var list = DB.Product_Info.getDataSource(key, 1, int.MaxValue, out total);
            return base.ToExcel(list);
        }
        #endregion
    }
}