using System;
using System.Web.Mvc;
using Business;
using DataBase;
using System.Linq;

namespace Web.Areas.Admin_Infos.Controllers
{
    public class AdminMsgController : Web.Controllers.AdminBaseController
    {
        // GET: Member_Infos/MyMsg
        public ActionResult Index(string SenderCode = "",string type="")
        {
            ViewBag.SenderCode = SenderCode;
            ViewBag.type = type;
            var total = 0;
            
            var list = DB.Sys_Msg.getDataSource("C3B57B68-3BBF-45DA-9B16-B3BE88F2A535", "", out total, 0, 0).ToList();
         
            ViewBag.List = list;
           
            return View(list);
        }

        public ActionResult Total()
        {
            return View();
        }

        public ActionResult JuBaoIndex(string code = "")
        {
            ViewBag.Code = code;
            var total = 0;
            if (code == "1")
            {
                code = "";
            }
            var list = DB.Sys_Msg.getDataSource("C3B57B68-3BBF-45DA-9B16-B3BE88F2A535", code, out total, 0, 0).ToList();

            ViewBag.List = list;

            return View(list);
        }
        #region 编辑视图
        public ActionResult Detail(int? id, string commentForm, int? msgid)
        {
            if (!string.IsNullOrEmpty(commentForm))
            {
                return RedirectToAction("Show", new { id = id.Value });
            }
            var m = DB.Member_Info.Where(a => a.Code == "admin").FirstOrDefault();
            var model = new DataBase.Sys_Msg()
            {
                SenderId = m.MemberId,
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

            if (model.ReceiverId != "C3B57B68-3BBF-45DA-9B16-B3BE88F2A535")
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
        public ActionResult JuBaoShow(int id)
        {
            var model = DB.Sys_Msg.FindEntity(id);

            if (model.ReceiverId != "C3B57B68-3BBF-45DA-9B16-B3BE88F2A535")
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
        public string getDataSource(string key, int start, int length, int draw, string SendCode = "", string type = "")
        {
            var total = 0;
            var list = DB.Sys_Msg.getDataSource("C3B57B68-3BBF-45DA-9B16-B3BE88F2A535", key, out total, start, length, null,false);


            return ToPage(list, total, start, length, draw);
        }
        //public string getDataSourceTotal(string key, int start, int length, int draw)
        //{
        //    var total = 0;
        //    var list = DB.Sys_Msg.getDataSourceTotal("C3B57B68-3BBF-45DA-9B16-B3BE88F2A535", key, out total, start, length);


        //    return ToPage(list, total, start, length, draw);
        //}
        #endregion

        #region 删除
        public ActionResult Delete(string idList)
        {
            return Json(DB.Sys_Msg.Delete(idList));
        }
        #endregion
        public void ToFY()
        {
            return;
        }
        #region 保存
        public ActionResult Save(string content,string ReceiverCode,string Title,string Image)
        {
            Sys_Msg entity = new Sys_Msg();
            entity.CreateTime = DateTime.Now;
            entity.Title = Title;
            entity.Comment = content;
            entity.SenderCode = DB.Member_Info.FindEntity("C3B57B68-3BBF-45DA-9B16-B3BE88F2A535").Code;
            entity.ReceiverCode = ReceiverCode;
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