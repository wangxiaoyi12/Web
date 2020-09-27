using Common;
using System;
using System.Web.Mvc;
using Business;
using System.Text;
using System.Collections;

namespace Web.Areas.Admin_BasicSettings.Controllers
{
    public class SysParametersController : Web.Controllers.AdminBaseController
    {
        public ActionResult Index()
        {
            Hashtable drawtime = new Hashtable();
            drawtime.Add(0, "周日");
            drawtime.Add(1, "周一");
            drawtime.Add(2, "周二");
            drawtime.Add(3, "周三");
            drawtime.Add(4, "周四");
            drawtime.Add(5, "周五");
            drawtime.Add(6, "周六");
            ViewData["DrawName"] = drawtime;
            return View(DB.XmlConfig.XmlSite);
        }

        #region 保存
        [ValidateInput(false)]
        public ActionResult Save(DataBase.Xml_Site entity)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "保存失败" };
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("原值：");

                XMLHelp xmlhelp = new Common.XMLHelp("/XmlConfig/site.config");
                #region 通过反射来取各个字段
                var type = typeof(DataBase.Xml_Site);
                var ps = type.GetProperties();
                #region 先列出原来的值
                {
                    var m = DB.XmlConfig.XmlSite;
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
                DB.XmlConfig.RefreshConfigSite();
                #region 列出新的值
                {
                    var m = DB.XmlConfig.XmlSite;
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

        #region 保存
        public string SaveImage()
        {
            return FileUpload.UpLoad("/upload/images/", Request, Server, 1);
        }
        #endregion 
    }
}