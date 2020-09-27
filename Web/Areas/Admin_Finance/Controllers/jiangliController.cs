using System;
using System.Web.Mvc;
using Business;

namespace Web.Areas.Admin_Finance.Controllers
{
    public class jiangliController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_Finance/DrawManage
        public ActionResult Index()
        {
            return View();
        }

        #region 查询       
        public string getDataSource(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.Fin_ShiChangimp.getDataSource(null, startTime, end, key, out total, start, length);

            return ToPage(list, total, start, length, draw);
        }
        #endregion

        #region 审核操作
        public ActionResult Issue(int id, int type)
        {
            return Json(DB.Fin_ShiChangimp.Issue(id, type, CurrentUser.Id, CurrentUser.Name));
        }
        #endregion      

        #region 删除
        public ActionResult Delete(string idList)
        {
            return Json(DB.Fin_ShiChangimp.Delete(idList));
        }
        #endregion

        #region 导出excel
        public FileResult ToExcel(DateTime? startTime, DateTime? end, string key)
        {
            int total = 0;
            var list = DB.Fin_ShiChangimp.getDataSource(null, startTime, end, key, out total, 0, int.MaxValue);
            return base.ToExcel(list);
        }
        #endregion

        //public JsonResult GiveDraw(string idList)
        //{
        //    var account = GetAccountByCookie();
        //    return Json(DB.Fin_Draw.GiveDraw(idList, account.Id, account.Name));
        //}
    }
}