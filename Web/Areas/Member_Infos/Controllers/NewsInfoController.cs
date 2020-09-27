using System;
using System.Web.Mvc;
using Business;

namespace Web.Areas.Member_Infos.Controllers
{
    /// <summary>
    /// 新闻 公告
    /// </summary>
    public class NewsInfoController : Web.Controllers.MemberBaseController
    {
        // GET: Member_Info/NewsList
        public ActionResult Index()
        {
            return View();
        }

        #region 编辑 视图
        public ActionResult Detail(string id, string commentForm)
        {
            var model = new DataBase.News_Info();
            if (!string.IsNullOrEmpty(id))
            {
                model = DB.News_Info.FindEntity(Convert.ToInt32(id));
                DB.News_Info.ReadOne(model);
            }
            return View(model);
        }
        #endregion

        #region 查询
        public string getDataSource(string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.News_Info.getDataSource(key, out total, start, length);
            return ToPage(list, total, start, length, draw);
        }
        #endregion 
    }
}