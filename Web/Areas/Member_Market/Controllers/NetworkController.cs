using System.Linq;
using System.Web.Mvc;
using Business;
using DataBase;
using System;

namespace Web.Areas.Member_Market.Controllers
{
    /// <summary>
    /// 网络结构图
    /// </summary>
    public class NetworkController : Web.Controllers.MemberBaseController
    {
        // GET: Member_Market/Network 
        public ActionResult Index(string id)
        {
            ViewBag.Code = CurrentUser.LoginName;
            return View();
        }

        #region 老版网络图
        public ActionResult Index2(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = CurrentUser.Id;
            }
            //查询会员，使用上下级查询
            var path = ControllerPath + "/Index2/";
            var m1 = DB.Member_Info.FindEntity(a => a.MemberId == id);
            var len = m1.Position.Length + 2;
            var list = DB.Member_Info;//.Where(a => a.Position.StartsWith(m1.Position) && a.Position.Length <= len).ToList();

            setDefaultAll(m1.Code);
            ViewBag.Code1 = "<span><b><a class='btn actived'  href='" + path + m1.MemberId + "'>" + m1.Code + "</a></b></span>";
            if (m1.IsActive == "未激活")
            {
                ViewBag.Code1 = "<span><b><a  class='btn noactive yellow' href='" + path + m1.MemberId + "'>" + m1.Code + "</a></b></span>";
            }
            ViewBag.NickName1 = m1.RecommendCode;
            ViewBag.LevelName1 = m1.MemberLevelName;
            ViewBag.L1 = "0";
            ViewBag.R1 = "0";

            var c1 = list.Where(a => a.UpperId == m1.MemberId);
            setDefault(m1, 1);
            foreach (var item in c1) //第一层
            {
                if (item.Position.EndsWith("1"))//左区
                {
                    ViewBag.L1 = list.Where(a => a.IsActive == "已激活" && a.Position.StartsWith(item.Position)).Sum(a => a.ActiveAmount);
                    ViewBag.Code2 = "<span><b><a class='btn actived'  href='" + path + item.MemberId + "'>" + item.Code + "</a></b></span>";
                    if (item.IsActive == "未激活")
                    {
                        ViewBag.Code2 = "<span><b><a  class='btn noactive yellow' href='" + path + item.MemberId + "'>" + item.Code + "</a></b></span>";
                    }
                    ViewBag.NickName2 = item.RecommendCode;
                    ViewBag.LevelName2 = item.MemberLevelName;
                    ViewBag.L2 = "0";
                    ViewBag.R2 = "0";
                    var c2 = list.Where(a => a.UpperId == item.MemberId);
                    setDefault(item, 2);
                    foreach (var item2 in c2)  //第二层
                    {
                        if (item2.Position.EndsWith("1"))//左区
                        {
                            ViewBag.L2 = list.Where(a => a.IsActive == "已激活" && a.Position.StartsWith(item2.Position)).Sum(a => a.ActiveAmount);
                            ViewBag.Code4 = "<span><b><a class='btn actived'  href='" + path + item2.MemberId + "'>" + item2.Code + "</a></b></span>";
                            if (item2.IsActive == "未激活")
                            {
                                ViewBag.Code4 = "<span><b><a  class='btn noactive yellow' href='" + path + item2.MemberId + "'>" + item2.Code + "</a></b></span>";
                            }
                            ViewBag.NickName4 = item2.RecommendCode;
                            ViewBag.LevelName4 = item2.MemberLevelName;
                            ViewBag.L4 = list.Where(a => a.IsActive == "已激活" && a.Position.StartsWith(item2.Position + "1")).Sum(a => a.ActiveAmount);
                            ViewBag.R4 = list.Where(a => a.IsActive == "已激活" && a.Position.StartsWith(item2.Position + "2")).Sum(a => a.ActiveAmount);
                        }
                        else if (item2.Position.EndsWith("2"))//左区
                        {
                            ViewBag.R2 = list.Where(a => a.IsActive == "已激活" && a.Position.StartsWith(item2.Position)).Sum(a => a.ActiveAmount);
                            ViewBag.Code5 = "<span><b><a class='btn actived'  href='" + path + item2.MemberId + "'>" + item2.Code + "</a></b></span>";
                            if (item2.IsActive == "未激活")
                            {
                                ViewBag.Code5 = "<span><b><a  class='btn noactive yellow' href='" + path + item2.MemberId + "'>" + item2.Code + "</a></b></span>";
                            }
                            ViewBag.NickName5 = item2.RecommendCode;
                            ViewBag.LevelName5 = item2.MemberLevelName;
                            ViewBag.L5 = list.Where(a => a.IsActive == "已激活" && a.Position.StartsWith(item2.Position + "1")).Sum(a => a.ActiveAmount);
                            ViewBag.R5 = list.Where(a => a.IsActive == "已激活" && a.Position.StartsWith(item2.Position + "2")).Sum(a => a.ActiveAmount);
                        }
                    }
                }
                else if (item.Position.EndsWith("2"))//左区
                {
                    ViewBag.R1 = list.Where(a => a.IsActive == "已激活" && a.Position.StartsWith(item.Position)).Sum(a => a.ActiveAmount);
                    ViewBag.Code3 = "<span><b><a class='btn actived'  href='" + path + item.MemberId + "'>" + item.Code + "</a></b></span>";
                    if (item.IsActive == "未激活")
                    {
                        ViewBag.Code3 = "<span><b><a  class='btn noactive yellow' href='" + path + item.MemberId + "'>" + item.Code + "</a></b></span>";
                    }
                    ViewBag.NickName3 = item.RecommendCode;
                    ViewBag.LevelName3 = item.MemberLevelName;
                    ViewBag.L3 = "0";
                    ViewBag.R3 = "0";
                    var c2 = list.Where(a => a.UpperId == item.MemberId);
                    setDefault(item, 2);
                    foreach (var item2 in c2)  //第二层
                    {
                        if (item2.Position.EndsWith("1"))//左区
                        {
                            ViewBag.L3 = list.Where(a => a.IsActive == "已激活" && a.Position.StartsWith(item2.Position)).Sum(a => a.ActiveAmount);
                            ViewBag.Code6 = "<span><b><a class='btn actived'  href='" + path + item2.MemberId + "'>" + item2.Code + "</a></b></span>";
                            if (item2.IsActive == "未激活")
                            {
                                ViewBag.Code6 = "<span><b><a  class='btn noactive yellow' href='" + path + item2.MemberId + "'>" + item2.Code + "</a></b></span>";
                            }
                            ViewBag.NickName6 = item2.RecommendCode;
                            ViewBag.LevelName6 = item2.MemberLevelName;
                            ViewBag.L6 = list.Where(a => a.IsActive == "已激活" && a.Position.StartsWith(item2.Position + "1")).Sum(a => a.ActiveAmount);
                            ViewBag.R6 = list.Where(a => a.IsActive == "已激活" && a.Position.StartsWith(item2.Position + "2")).Sum(a => a.ActiveAmount);
                        }
                        else if (item2.Position.EndsWith("2"))//左区
                        {
                            ViewBag.R3 = list.Where(a => a.IsActive == "已激活" && a.Position.StartsWith(item2.Position)).Sum(a => a.ActiveAmount);
                            ViewBag.Code7 = "<span><b><a class='btn actived'  href='" + path + item2.MemberId + "'>" + item2.Code + "</a></b></span>";
                            if (item2.IsActive == "未激活")
                            {
                                ViewBag.Code7 = "<span><b><a  class='btn noactive yellow' href='" + path + item2.MemberId + "'>" + item2.Code + "</a></b></span>";
                            }
                            ViewBag.NickName7 = item2.RecommendCode;
                            ViewBag.LevelName7 = item2.MemberLevelName;
                            ViewBag.L7 = list.Where(a => a.IsActive == "已激活" && a.Position.StartsWith(item2.Position + "1")).Sum(a => a.ActiveAmount);
                            ViewBag.R7 = list.Where(a => a.IsActive == "已激活" && a.Position.StartsWith(item2.Position + "2")).Sum(a => a.ActiveAmount);
                        }
                    }
                }
            }
            ViewBag.TopId = CurrentUser.Id;
            ViewBag.ID = id;
            return View();
        }
        /// <summary>
        /// 查询会员
        /// </summary>
        /// <param name="id">会员id</param>
        /// <returns></returns>
        public string Search(string id)
        {
            var list = DB.Member_Info.Where(p => p.Code == CurrentUser.LoginName || p.Code == id).Select(a => new { a.Code, a.Position, a.MemberId }).ToList();
            var cur = list.FirstOrDefault(a => a.Code == CurrentUser.LoginName);
            var search = list.FirstOrDefault(a => a.Code == id);
            if (search.Position.StartsWith(cur.Position))
            {
                return ControllerPath + "/Index2/" + search.MemberId;
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// 返回上层
        /// </summary>
        /// <param name="id">当前会员id</param>
        /// <returns></returns>
        public string ToUp(string id)
        {
            var m = DB.Member_Info.FindEntity(id);
            var upid = id;
            if (!string.IsNullOrEmpty(m.UpperId))
            {
                if (upid != CurrentUser.Id)
                {
                    upid = m.UpperId;
                }
                else
                {
                    upid = CurrentUser.Id;
                }
            }
            return ControllerPath + "/Index2/" + upid;
        }
        /// <summary>
        /// 设置默认空值 
        /// </summary>
        /// <param name="code"></param>
        private void setDefaultAll(string code)
        {
            //第一层
            ViewBag.Code1 = "<span><b><a class='btn noactive'  href='javascript:;'>空位</a></b></span>";
            ViewBag.L1 = "0";
            ViewBag.R1 = "0";

            for (int i = 2; i < 8; i++)
            {
                var p = i % 2 == 0 ? 1 : 2;
                ViewData["Code" + i] = "<span><b><a class='btn noactive'  href='javascript:;'>空位</a></b></span>";
                ViewData["NickName" + i] = "";
                ViewData["L" + i] = "0";
                ViewData["R" + i] = "0";
            }
        }
        private void setDefault(Member_Info model, int layers)
        {
            var regUrl = "/Member_Center/Register/Detail";
            if (model.IsActive != "已激活") return;
            if (layers == 1)
            {
                //第一层
                ViewBag.Code2 = "<a class='btn noactive reg' href='" + regUrl + "?upid=" + model.Code + "&position=1'>注册</a>";
                ViewBag.Code3 = "<a class='btn noactive reg' href='" + regUrl + "?upid=" + model.Code + "&position=2'>注册</a>";
            }
            else if (layers == 2)
            {
                if (model.Position.EndsWith("1"))
                {
                    //第二层左
                    ViewBag.Code4 = "<a class='btn noactive reg' href='" + regUrl + "?upid=" + model.Code + "&position=1'>注册</a>";
                    ViewBag.Code5 = "<a class='btn noactive reg' href='" + regUrl + "?upid=" + model.Code + "&position=2'>注册</a>";
                }
                else if (model.Position.EndsWith("2"))
                {
                    ViewBag.Code6 = "<a class='btn noactive reg' href='" + regUrl + "?upid=" + model.Code + "&position=1'>注册</a>";
                    ViewBag.Code7 = "<a class='btn noactive reg' href='" + regUrl + "?upid=" + model.Code + "&position=2'>注册</a>";
                }
            }
        }
        #endregion
    }
}