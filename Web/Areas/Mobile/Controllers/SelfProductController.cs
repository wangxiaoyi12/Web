using DataBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.Mobile.Controllers
{
    /// <summary>
    /// 产品列表，产品详情
    /// </summary>
    public class SelfProductController : MobileBaseController
    {
        /// <summary>
        /// 列表页面
        /// </summary>
        /// <returns></returns>
        // GET: Mobile/SelfProduct
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取筛选获取列表数据
        /// </summary>
        public PartialViewResult List(string keywords, int? brandid, int? classid,
            int praise = 0,
            int order = 0,
            int ordertype = 0,
            int skipCount = 0
            )
        {
            //筛选
            var query = DB.ShopProduct.Where(a=>a.IsNew==false);
            if (classid != null)
            {
                if (classid != 0)
                {
                    ShopProductCategory cate = DB.ShopProductCategory.FindEntity(classid.Value);
                    List<int> childID = DB.ShopProductCategory.GetChildIDList(cate);
                    childID.Add(classid.Value);
                    query = query.Where(q => q.CategoryID != null && childID.Contains(q.CategoryID.Value));
                }
                else
                {
                    ShopProductCategory cate = DB.ShopProductCategory.FindEntity(DB.XmlConfig.XmlSite.Scores);
                    List<int> childID = DB.ShopProductCategory.GetChildIDList(cate);
                    childID.Add(Convert.ToInt32( DB.XmlConfig.XmlSite.Scores));
                    query = query.Where(q => q.CategoryID != null && !childID.Contains(q.CategoryID.Value));
                }
            }
            else
            {
                    ShopProductCategory cate = DB.ShopProductCategory.FindEntity(DB.XmlConfig.XmlSite.Scores);
                    List<int> childID = DB.ShopProductCategory.GetChildIDList(cate);
                    childID.Add(Convert.ToInt32(DB.XmlConfig.XmlSite.Scores));
                    query = query.Where(q => q.CategoryID != null && !childID.Contains(q.CategoryID.Value));
                
            }
            if (brandid != null)
                query = query.Where(q => q.BrandID == brandid);

            if (string.IsNullOrEmpty(keywords) == false)
                query = query.Where(q => q.Title.Contains(keywords));

            if (praise == 1)
            {
                query = query.Where(q => q.IsHot == true);
            }
            ViewBag.praise = praise;
            query = query.Where(q => q.IsEnable == true);
            ViewBag.allCount = query.Count();
            //排序
            #region 排序
            if (order == 0)
            {
                if (praise == 1)
                {
                    if (ordertype == 0)
                    {
                        query = query.OrderByDescending(q => q.PraiseCount)
                      .ThenBy(q => q.CreateTime);
                    }
                    else
                    {
                        query = query.OrderBy(q => q.PraiseCount)
                      .ThenBy(q => q.CreateTime);
                    }
                }
                else
                {
                    if (ordertype == 0)
                    {
                        query = query.OrderBy(q => q.Sort)
                            .ThenByDescending(q => q.IsHot)
                     .ThenByDescending(q => q.CreateTime);
                    }
                    else
                    {
                        query = query.OrderByDescending(q => q.Sort)
                         .ThenByDescending(q => q.IsHot)
                         .ThenByDescending(q => q.CreateTime);
                    }
                }
            }
            else if (order == 1)
            {
                if (ordertype == 0)
                {
                    query = query.OrderBy(q => q.ShopOrderProducts.Count());
                }
                else
                {
                    query = query.OrderByDescending(q => q.ShopOrderProducts.Count());
                }
            }
            else if (order == 2)
            {
                if (ordertype == 0)
                {
                    query = query.OrderBy(q => q.PriceShopping);
                }
                else
                {
                    query = query.OrderByDescending(q => q.PriceShopping);
                }
            }

            #endregion
            //分页
            List<ShopProduct> list = query.Skip(skipCount)
                .Take(8)
                .ToList();
            ViewBag.skipCount = skipCount + list.Count;
            return PartialView(list);
        }



        /// <summary>
        /// 添加点赞记录
        /// </summary>
        /// <param name="productid"></param>
        /// <returns></returns>
        public string Praise(int productid)
        {
            try
            {
                //判断是否登录
                if (User_Shop.IsLogin() == false)
                    throw new Exception("请登陆后点赞");
                string curUserID = User_Shop.GetMemberID();
                //判断是否赞过
                if (DB.PraiseRecord.Any(q => q.ProductID == productid && q.MemberID == curUserID))
                    throw new Exception("亲，当前商品已经赞过了");

                PraiseRecord record = new PraiseRecord();
                record.ProductID = productid;
                record.MemberID = curUserID;
                record.CreateTime = DateTime.Now;
                DB.PraiseRecord.Insert(record);

                //累计点赞数
                ShopProduct product = DB.ShopProduct.FindEntity(productid);
                product.PraiseCount++;
                DB.ShopProduct.Update(product);

                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public ActionResult UpLoadTu(string File, string path = "/upload/qrimg/")
        {
            JsonHelp json = new JsonHelp(true);
            //上传和返回(保存到数据库中)的路径
            var tempPath = Server.MapPath(path);
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);//不存在就创建目录 
            }
            if (Request.Files.Count <= 0) return Json(json);
            var imgFile = Request.Files["file"];
            //创建图片新的名称
            var nameImg = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            //获得上传图片的路径
            var strPath = imgFile.FileName;
            //获得上传图片的类型(后缀名)
            var type = strPath.Substring(strPath.LastIndexOf(".", StringComparison.Ordinal) + 1).ToLower(); ;

            //拼写数据库保存的相对路径字符串
            // savepath = "..\\" + path + "\\";
            path += nameImg + "." + type;
            //拼写上传图片的路径
            var uppath = Server.MapPath(path);
            // uppath += nameImg + "." + type;
            //上传图片
            imgFile.SaveAs(uppath);

            return Json(json.Msg = path);
        }
        #region 详细页面
        /// <summary>
        /// 产品详细页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Detail(int id, string isPhone)
        {
            ViewBag.id = id;
            ShopProduct model = DB.ShopProduct.FindEntity(q => q.ID == id);
            var category = DB.ShopProduct.GetCategoryId2(model.CategoryID.Value);
            var list = DB.GuiGeProduct_Info.Where(a => a.SName == category.ID && a.ProductId == model.ID).ToList();
            ViewBag.isPhone = isPhone;




            var gugelist = DB.GuiGeName.Where(a => a.CName == category.Name).OrderBy(a => a.GId).ToList();

            var strlist1 = new List<string>();

            int i = 0;
            foreach (var item in gugelist)
            {
                string str = item.GId + "_" + item.GName + "_";
                foreach (var item1 in list)
                {
                    string[] strs = item1.SComment.Split('_');//2

                    if (strs.Count() <= i)
                    {

                    }
                    else
                    {
                        if (!str.Contains("_" + strs[i] + "_"))
                        {
                            str += strs[i] + "_";
                        }
                    }
                }
                str = str.Substring(0, str.Length - 1);
                strlist1.Add(str);
                i++;
            }
            ViewBag.GG = strlist1;
            return View();
        }
        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="SkipCount"></param>
        /// <returns></returns>
        public PartialViewResult GetComment(int id, int SkipCount = 0)
        {
            int totalCount = 0;
            List<ShopOrderComment> commentList = DB.ShopProduct.GetCommentList(id, out totalCount, SkipCount);
            ViewBag.totalCount = totalCount;
            ViewBag.skipCount = SkipCount + commentList.Count;
            return PartialView(commentList);
        }
        #endregion
    }
}