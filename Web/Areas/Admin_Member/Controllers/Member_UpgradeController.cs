using Business;
using System;
using System.Web.Mvc;
using Web.Controllers;

namespace Web.Areas.Admin_Member.Controllers
{
    public class Member_UpgradeController : AdminBaseController
    {
        // GET: Admin_Member/Member_Upgrade
        public ActionResult Index()
        {
            return View();
        }

        public string getDataSource(DateTime? startTime, DateTime? end, string key, int start, int length, int draw)
        {
            var total = 0;
            var list = DB.Member_Upgrade.getDataSource(startTime, end, key, out total, start, length);
            return ToPage(list, total, start, length, draw);
        }
    }
}