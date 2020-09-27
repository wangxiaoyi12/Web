using Common;
using DataBase;
using System;

namespace Business.Implementation
{
    public class XmlConfig
    {
        private Xml_Site xmlsite;
        private Xml_Shop xmlshop;

        /// <summary>
        /// 系统参数配置实例
        /// </summary>
        public Xml_Site XmlSite
        {
            get
            {
                if (xmlsite == null)
                {
                    RefreshConfigSite();
                }
                return xmlsite;
            }
            set { xmlsite = value; }
        }
        /// <summary>
        /// 系统参数配置实例
        /// </summary>
        public Xml_Shop XmlShop
        {
            get
            {
                if (xmlshop == null)
                {
                    RefreshConfigShop();
                }
                return xmlshop;
            }
            set { xmlshop = value; }
        }

        public void SaveSite(Xml_Site entity)
        {
            XMLHelp xmlhelp = new Common.XMLHelp("/XmlConfig/site.config");
            #region 通过反射来取各个字段
            var type = typeof(DataBase.Xml_Site);
            var ps = type.GetProperties();             
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
            RefreshConfigSite();
        }

        public void SaveShop(Xml_Shop entity)
        {
            XMLHelp xmlhelp = new Common.XMLHelp("/XmlConfig/shop.config");
            #region 通过反射来取各个字段
            var type = typeof(DataBase.Xml_Shop);
            var ps = type.GetProperties();
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
            RefreshConfigShop();
        }

        public void SaveConfigSite(string name ,string value)
        {
            XMLHelp xmlhelp = new Common.XMLHelp("/XmlConfig/site.config");
            xmlhelp.SetXmlNodeValue("//" + name, value.ToString());
            xmlhelp.SavexmlDocument();
            RefreshConfigSite();
        }

        public void SaveConfigShop(string name, string value)
        {
            XMLHelp xmlhelp = new Common.XMLHelp("/XmlConfig/shop.config");
            xmlhelp.SetXmlNodeValue("//" + name, value.ToString());
            xmlhelp.SavexmlDocument();
            RefreshConfigShop();
        }

        /// <summary>
        /// 刷新系统参数配置
        /// </summary>
        public void RefreshConfigSite()
        {
            XMLHelp xmlhelp = new Common.XMLHelp("/XmlConfig/site.config");
            xmlsite = new Xml_Site();
            #region 通过反射来取各个字段
            var type = typeof(Xml_Site);
            var ps = type.GetProperties();
            foreach (var item in ps)
            {
                var name = item.Name;
                var value = WebTools.ChangeType(xmlhelp.GetNodeValue(name), item.PropertyType);
                if (value != null && !string.IsNullOrEmpty(value.ToString()))
                    item.SetValue(xmlsite, value);
            }
            #endregion 
        }
        public void RefreshConfigShop()
        {
            XMLHelp xmlhelp = new Common.XMLHelp("/XmlConfig/shop.config");
            xmlshop = new Xml_Shop();
            #region 通过反射来取各个字段
            var type = typeof(Xml_Shop);
            var ps = type.GetProperties();
            foreach (var item in ps)
            {
                var name = item.Name;
                var value = WebTools.ChangeType(xmlhelp.GetNodeValue(name), item.PropertyType);
                if (value != null && !string.IsNullOrEmpty(value.ToString()))
                    item.SetValue(xmlshop, value);
            }
            #endregion 
        }

        /// <summary>
        /// 自动设置本系统域名网址
        /// </summary>
        /// <param name="Request"></param>
        public void AutoSetUrl(System.Web.HttpRequestBase Request)
        {
            try
            {
                //if (string.IsNullOrEmpty(entity.weburl) || entity.weburl.StartsWith("http://localhost") || entity.weburl.StartsWith("http://127.0.0.1"))
                {
                    //修改成当前地址的
                    var url = Request.Url.ToString();
                    XmlSite.weburl = url.Substring(0, url.IndexOf("/", 7));
                    XMLHelp xmlhelp = new Common.XMLHelp("/XmlConfig/site.config");
                    xmlhelp.SetXmlNodeValue("//weburl", XmlSite.weburl);
                    xmlhelp.SavexmlDocument();
                }
            }
            catch (Exception e)
            {
                LogHelper.Error("自动设置网址域名出错：" + Request?.Url);
            }
        }
    }
}
