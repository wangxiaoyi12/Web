using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Wechat;
using DataBase;
using Business;
namespace Web.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {

            //模板消息--提现成功
            try
            {
                MsgMessage _msg = new MsgMessage();
                Fin_Draw model = DB.Fin_Draw.Where(q => q.DrawId == 1).FirstOrDefault();
                _msg.Send_DrawSuccess(model, "oUEId1rfKD_Kbhpwqgn7mm24owSM", $"http://{Request.Url.Host}/Member_Finance/Draw/Index");

                return Content("发送成功");
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
           
        }


        #region  维护RPosition
        public ActionResult ResetRPosition(string q)
        {
            if (q == "zhijukejixsy888")
            {
                using (var db = new DbMallEntities())
                {
                    var list = db.Member_Info.ToList();
                    var admin = list.FirstOrDefault(a => a.Code == "admin");
                    admin.RPosition = "0|";
                    updatechild_rposition(admin, list);
                    db.SaveChanges();
                }
                return Content(DateTime.Now + "_succss");
            }
            return View();
        }
        public void updatechild_rposition(Member_Info m, List<Member_Info> list)
        {
            var childs = list.Where(a => a.RecommendId == m.MemberId).ToList();
            for (int i = 0; i < childs.Count; i++)
            {
                childs[i].RPosition = m.RPosition + i + "|";
                updatechild_rposition(childs[i], list);
            }                  
        }
        #endregion
    }
}