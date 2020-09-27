using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business.Interface;
using Business.Implementation;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            ISys_Employee manage = new Sys_EmployeeImp();

            var data = manage.LoadAll().ToList();

            return View();
        }
    }
}