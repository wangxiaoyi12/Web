using System;
using System.Web.Mvc;
using Business;

namespace Web.Areas.Admin_Information.Controllers
{
    public class NewsController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_Information/News
        public ActionResult Index()
        {
            return View();
        }

        #region 查看视图
        public ActionResult Show(string id, string commentForm)
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

        #region 编辑视图
        public ActionResult Detail(int? id, string type)
        {
            switch (type)
            {
                case "1"://编辑新闻
                    ViewBag.Head = "编辑新闻";
                    ViewBag.Ids = id;
                    ViewBag.Type = type;
                    var entity1 = DB.News_Info.FindEntity(id);
                    ViewBag.Type = type;
                    ViewData["id"] = entity1.NewsId;
                    ViewData["title"] = entity1.Title;
                    ViewData["content"] = entity1.Comment;
                    ViewData["ReadNum"] = entity1.ReadNum;
                    break;
                case "2"://添加新闻
                    ViewBag.Head = "添加新闻";
                    ViewBag.Ids = id;
                    ViewBag.Type = type;
                    ViewData["id"] = "";
                    ViewData["title"] = "";
                    ViewData["content"] = "";
                    ViewData["ReadNum"] = 0;
                    break;
                case "3"://公司简介，公司账户，销售方案
                    ViewBag.Ids = id;
                    ViewBag.Type = type;
                    if (id == 1)
                    {
                        ViewBag.Head = "编辑公司简介";
                    }
                    else if (id == 2)
                    {
                        ViewBag.Head = "编辑公司账户";
                    }
                    else if (id == 3)
                    {
                        ViewBag.Head = "编辑注册协议";
                    }
                    var entity2 = DB.Article_Info.FindEntity(id);
                    ViewData["id"] = entity2.ArticleId;
                    ViewData["title"] = entity2.Title;
                    ViewData["content"] = entity2.Comment;
                    ViewData["ReadNum"] = entity2.ReadNum;
                    break;
                default:
                    break;
            }
            return View();
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

        #region 保存
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="type">操作类型</param>
        /// <param name="id">主键</param>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        [Web.Controllers.Permissions]
        [ValidateInput(false)]
        public ActionResult Save(string type, string id, string title, string content, int? ReadNum)
        {
            if (ReadNum == null) ReadNum = 0;
            return Json(DB.News_Info.Save(type, id, title, content, ReadNum.Value, CurrentUser.Id, CurrentUser.Name));
        }
        #endregion

        #region 删除（可批量）
        [Web.Controllers.Permissions]
        public ActionResult Delete(string idList)
        {
            return Json(DB.News_Info.Delete(idList));
        }
        #endregion

        #region 导出excel
        public FileResult ToExcel(DateTime? startTime, DateTime? end, string key)
        {
            int total = 0;
            var list = DB.News_Info.getDataSource(key, out total, 0, int.MaxValue);
            return base.ToExcel(list);
        }
        #endregion
    }
}