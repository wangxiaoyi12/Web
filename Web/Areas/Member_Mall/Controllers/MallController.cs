using System;
using System.Web.Mvc;
using Business;

namespace Web.Areas.Member_Mall.Controllers
{
    public class MallController : Web.Controllers.MemberBaseController
    {
        // GET: Member_Mall/Mall
        public ActionResult Index(string id, int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.Key = id;
            int totalCount = 0;
            var list = DB.Product_Info.getDataSource(id,pageIndex,pageSize,out totalCount);
            
            var pageCount = totalCount % pageSize == 0 ? totalCount / pageSize : totalCount / pageSize + 1;
            if (pageIndex > pageCount)
            {
                pageIndex = pageCount;
            }
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize= pageSize;
            ViewBag.TotalCount = totalCount;
            return View(list);
        }
      

        public ActionResult Detail(int id)
        {
            var m = DB.Product_Info.FindEntity(id);
            return View(m);
        }

        #region 保存
        public JsonResult Save(int goods_num, int productid)
        {
            //var goods_num = int.Parse(Request["goods_num"]);
            //var id = int.Parse(Request["ProductId"]);
            var product = DB.Product_Info.FindEntity(productid);
            var member = DB.Member_Info.FindEntity(CurrentUser.Id);
            var totalPrice = goods_num * product.SalePrice;

            var model = new DataBase.Bill_Info();
            model.ProductId = product.ProductId;
            model.CategoryId = product.CategoryId;
            model.CategoryName = product.CategoryName;
            model.ProductName = product.ProductName;
            model.ModelName = product.ModelName;
            model.OriginalPrice = product.OriginalPrice;
            model.SalePrice = product.SalePrice;
            model.PV = product.PV;
            model.Qty = goods_num;
            model.TotalPrice = totalPrice;
            model.State = "未发货";
            model.MemberId = member.MemberId;
            model.MemberCode = member.Code;
            model.NickName = member.NickName;
            model.PostAddress = member.PostAddress;
            model.Mobile = member.Mobile;
            model.CreateTime = DateTime.Now;
            //model.SendTime = SendTime;
            model.ExpressName = string.Empty;
            model.ExpressCode = string.Empty;
            var r = DB.Bill_Info.Save(model);
            if (r.IsSuccess)
            {
                r.ReUrl = "/Member_Mall/MyBill/Index";
            }
            return Json(r);
        }
        #endregion
    }
}