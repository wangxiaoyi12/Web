using Common;
using System;
using System.Web.Mvc;
using Business;
using System.Text;

namespace Web.Areas.ShopAdmin.Controllers
{
    /// <summary>
    /// 商城参数配置
    /// </summary>
    public class ShopParameterController : Web.Controllers.AdminBaseController
    {
        public ActionResult Index()
        {
            return View(DB.XmlConfig.XmlShop);
        }

        #region 保存
        [ValidateInput(false)]
        public ActionResult Save(DataBase.Xml_Shop entity)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "保存失败" };
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("原值：");

                XMLHelp xmlhelp = new Common.XMLHelp("/XmlConfig/shop.config");
                #region 通过反射来取各个字段
                var type = typeof(DataBase.Xml_Shop);
                var ps = type.GetProperties();
                #region 先列出原来的值
                {
                    var m = DB.XmlConfig.XmlShop;
                    foreach (var item in ps)
                    {
                        var name = item.Name;
                        var value = item.GetValue(m);
                        if (value != null)
                        {
                            sb.AppendFormat("{0}：{1},", name, value);
                        }
                    }
                }
                #endregion
                foreach (var item in ps)
                {
                    var name = item.Name;
                    var value = item.GetValue(entity);
                    if (value != null)
                    {
                        xmlhelp.SetXmlNodeValue("//" + item.Name, value.ToString());
                    }
                }
                #endregion
                xmlhelp.SavexmlDocument();
                DB.XmlConfig.RefreshConfigShop();
                #region 列出新的值
                {
                    var m = DB.XmlConfig.XmlShop;
                    sb.AppendFormat("新值：");
                    foreach (var item in ps)
                    {
                        var name = item.Name;
                        var value = item.GetValue(m);
                        if (value != null)
                        {
                            sb.AppendFormat("{0}：{1},", name, value);
                        }
                    }
                }
                #endregion
                LogHelper.Info(sb.ToString());
                json.Status = "y";
                json.Msg = "保存成功";
            }
            catch (Exception e)
            {
                LogHelper.Error("保存系统配置参数出错：" + e.Message);
            }
            return Json(json);
        }
        #endregion
    }
}