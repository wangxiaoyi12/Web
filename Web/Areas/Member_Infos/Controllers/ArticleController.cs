using System.Web.Mvc;
using Business;
using System;

namespace Web.Areas.Member_Infos.Controllers
{
    /// <summary>
    /// 文章
    /// </summary>
    public class ArticleController : Web.Controllers.MemberBaseController
    {
        // GET: Member_Info/Article
        public ActionResult Index(int id)
        {
            ViewBag.Id = id;
            switch (id)
            {
                case 1:
                    ViewBag.Head = "公司简介";
                    break;
                case 2:
                    ViewBag.Head = "汇款账户";
                    break;
                case 3:
                    ViewBag.Head = "销售方案";
                    break;
                default:
                    break;
            }
            var m = DB.Article_Info.FindEntity(id);
            if (m == null) m = new DataBase.Article_Info() { Title = "无信息" };
            return View(m);
        }
    }
}