using System;
using System.Web.Mvc;
using Business;
using DataBase;

namespace Web.Areas.Mobile.Controllers
{
    public class MyMsgController : Web.Controllers.MemberBaseController
    {
        // GET: Member_Infos/MyMsg
        public ActionResult Index()
        {
            ViewBag.m = DB.Member_Info.FindEntity(CurrentUser.Id);
            return View();
        }

        #region 编辑视图
        public ActionResult Detail(int? id, string commentForm, int? msgid)
        {
            if (!string.IsNullOrEmpty(commentForm))
            {
                return RedirectToAction("Show", new { id = id.Value });
            }
            var m = DB.Member_Info.FindEntity(CurrentUser.Id);
            var model = new DataBase.Sys_Msg()
            {
                SenderId = CurrentUser.Id,
                SenderCode = m.Code
            };
            if (msgid != null)
            {
                var msg = DB.Sys_Msg.FindEntity(msgid.Value);
                model.Title = "回复“" + msg.Title + "”";
                model.ReceiverCode = msg.SenderCode;
            }

            if (id != null)
            {
                model = DB.Sys_Msg.FindEntity(id.Value);
            }
            return View(model);
        }
        #endregion

        #region 查看视图
        public ActionResult Show(int id)
        {
            var model = DB.Sys_Msg.FindEntity(id);

            if (model.ReceiverId != CurrentUser.Id)
            {
                ViewBag.IsHiddenReply = true;
            }
            else
            {
                model.State = 1;
                model.ReadTime = DateTime.Now;
                DB.Sys_Msg.Save(model);
                ViewBag.IsHiddenReply = false;
            }
            return View(model);
        }
        #endregion

        #region 查询
        public string getDataSource(string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.Sys_Msg.getDataSource(CurrentUser.Id, key, out total, start, length);
            return ToPage(list, total, start, length, draw);
        }
        #endregion

        #region 删除
        public ActionResult Delete(string idList)
        {
            return Json(DB.Sys_Msg.Delete(idList));
        }
        #endregion

        #region 保存
        public ActionResult Save(string Title,string Comment,string Image)
        {
            var m = DB.Member_Info.FindEntity(CurrentUser.Id);
            var entity = new Sys_Msg();
            entity.CreateTime = DateTime.Now;
            entity.State = 1;
            entity.SenderCode = m.Code;
            entity.Title = Title;
            entity.Comment = Comment;
            entity.ReceiverCode="admin";
            entity.Image = Image;
            var r = DB.Sys_Msg.Save(entity);
            if (r.IsSuccess)
            {
                r.ReUrl = ControllerPath + "/Index";
            }
            return Json(r);
        }
        #endregion
    }
}