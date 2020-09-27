using System.Web.Mvc;
using Business;
using System;
using DataBase;
using System.Linq;

namespace Web.Areas.Member_Center.Controllers
{
    /// <summary>
    /// 申请店主
    /// </summary>
    public class JiangLController : Web.Controllers.MemberBaseController
    {
        // GET: Member_Center/ServiceApply
        public ActionResult Index()
        {
            var model = DB.Member_Info.FindEntity(CurrentUser.Id);
            return View(model);
        }
        #region 查询
        public string getDataSource(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.Fin_ShiChangimp.getDataSource(CurrentUser.Id, startTime, end, key, out total, start, length);
            return ToPage(list, total, start, length, draw);
        }
        #endregion
        public ActionResult Detail(string id, string commentForm)
        {
            var m = DB.Member_Info.FindEntity(CurrentUser.Id);
            var LAmount = DB.Member_Info.Where(p => p.Position.StartsWith(m.Position + "1")).Sum(p => p.ActiveAmount) ?? 0;
            var RAmount = DB.Member_Info.Where(p => p.Position.StartsWith(m.Position + "2")).Sum(p => p.ActiveAmount) ?? 0;
            ViewBag.min = Math.Min(LAmount, RAmount);
            var model = new Fin_ShiChangimp()
            {
                MemberCode = m.Code,
                MemberId = m.MemberId,
                NickName = m.NickName
            };
            if (!string.IsNullOrEmpty(id))
            {
                model = DB.Fin_ShiChangimp.FindEntity(Convert.ToInt32(id));
            }
            return View(model);
        }
        #region 保存
        public ActionResult Save()
        {
            //var Pwd = Common.CryptHelper.DESCrypt.Encrypt(Request["Pwd2"]);
            var Name = Request["TypeName"];
            var model = DB.Member_Info.FindEntity(CurrentUser.Id);
            var LAmount = DB.Member_Info.Where(p => p.Position.StartsWith(model.Position + "1")).Sum(p => p.ActiveAmount) ?? 0;
            var RAmount = DB.Member_Info.Where(p => p.Position.StartsWith(model.Position + "2")).Sum(p => p.ActiveAmount) ?? 0;
            var min = Math.Min(LAmount, RAmount);
            var entity = new Fin_ShiChangimp();
            entity.State = "未发放";
            entity.CreateTime = DateTime.Now;
            entity.TypeName = Name;
            entity.MemberId = model.MemberId;
            entity.MemberCode = model.Code;
            entity.NickName = model.NickName;
            entity.JiangL = "";
            entity.SanXiaYji = min;
            entity.XiaoQUYej = min;
            var r = DB.Fin_ShiChangimp.Save(entity);
            if (r.IsSuccess)
            {
                r.ReUrl = ControllerPath + "/Index";
            }
            return Json(r);
        }
        #endregion 
    }
}