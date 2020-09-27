using System.Web.Mvc;
using System.Linq;
using Business;
using System;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Net.Http;
using System.Threading.Tasks;

namespace Web.Areas.SysManage.Controllers
{
    public class DeskController : Web.Controllers.MemberBaseController
    {
        // GET: SysManage/Desk
        public ActionResult Index()
        {
            var member = DB.Member_Info.FindEntity(p => p.MemberId == CurrentUser.Id);
            ViewData["member"] = member;
            ViewData["sum"] = DB.Member_Info.Count(p => p.RecommendId == member.MemberId && p.IsActive == "已激活");
            ViewData["news"] = DB.News_Info.Where().OrderByDescending(p => p.LastEditTime).Take(10).ToList();
            DB.XmlConfig.AutoSetUrl(Request);
            return View();
        }
        public ActionResult JYDemo()
        {
            return View();
        }
        public string RealAmountForWeekToLine()
        {
            return DB.Fin_Info.RealAmountForWeekToLine(CurrentUser.Id);
        }



        ///// <summary>
        ///// 头像文星
        ///// </summary>
        ///// <returns></returns>
        //public void GetTouXiang()
        //{
        //    var member = DB.Member_Info.FindEntity(p => p.MemberId == CurrentUser.Id);
        //    try
        //    {
        //        if (string.IsNullOrEmpty(member.Photo))
        //        {
        //            throw new Exception();
        //        }
        //        else
        //        {
        //            System.Net.WebClient client = new System.Net.WebClient();
        //            byte[] data = client.DownloadData(member.Photo);
        //           // Response.ContentType = "image/jpeg";
        //            Response.BinaryWrite(data);
        //            Response.End();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string filename = Server.MapPath("~/assets/mobile/images/people2.png");
        //        FileStream sr = new FileStream(filename, FileMode.Open, FileAccess.Read);
        //        byte[] data = new byte[sr.Length];
        //        sr.Read(data, 0, data.Length);
        //       // Response.ContentType = "image/jpeg";
        //        Response.BinaryWrite(data);
        //        Response.End();
        //    }
        //}



        ///// <summary>
        ///// 头像文星
        ///// </summary>
        ///// <returns></returns>
        //public FileContentResult GetTouXiang()
        //{
        //    var member = DB.Member_Info.FindEntity(p => p.MemberId == CurrentUser.Id);
        //    try
        //    {
        //        if (string.IsNullOrEmpty(member.Photo))
        //        {
        //            throw new Exception();
        //        }
        //        else
        //        {
        //            HttpClient client = new HttpClient();
        //            byte[] data = client.GetByteArrayAsync(member.Photo).Result;
        //            return File(data, "image/jpeg");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string filename = Server.MapPath("~/assets/mobile/images/people2.png");
        //        FileStream sr = new FileStream(filename, FileMode.Open, FileAccess.Read);
        //        byte[] data = new byte[sr.Length];
        //        sr.Read(data, 0, data.Length);
        //        return File(data, "image/jpeg");
        //    }
        //}

        /// <summary>
        /// 头像文星
        /// </summary>
        /// <returns></returns>
        public async Task<FileResult> GetTouXiang()
        {
            var member = DB.Member_Info.FindEntity(p => p.MemberId == CurrentUser.Id);
            try
            {
                if (string.IsNullOrEmpty(member.Photo))
                {
                    throw new Exception();
                }
                else
                {

                    HttpClient client = new HttpClient();
                    byte[] data = await client.GetByteArrayAsync(member.Photo);
                    return File(data, "image/jpeg");
                }
            }
            catch (Exception ex)
            {
                string filename = Server.MapPath("~/assets/mobile/images/people2.png");
                FileStream sr = new FileStream(filename, FileMode.Open, FileAccess.Read);
                byte[] data = new byte[sr.Length];
                sr.Read(data, 0, data.Length);
                return File(data, "image/jpeg");
            }
        }
    }
}