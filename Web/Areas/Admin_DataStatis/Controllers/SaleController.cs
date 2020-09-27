using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using DataBase;
using Business;
using Common;
namespace Web.Areas.Admin_DataStatis.Controllers
{
    /// <summary>
    /// 销售收入统计
    /// </summary>
    public class SaleController : Web.Controllers.AdminBaseController
    {
        // GET: Admin_DataStatis/Sale
        public ActionResult Index()
        {
            return View();
        }
    }
}