using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Common;


namespace Wechat
{
    /// <summary>
    /// 公众账号配置信息
    /// </summary>
    public class ConfigInfo
    {
        /// <summary>
        /// 请求验证，自定义token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 微信账号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppID { get; set; }
        /// <summary>
        /// 应用 AppSecret
        /// </summary>
        public string AppSecret { get; set; }
        /// <summary>
        /// 和微信服务器，交互，状态值
        /// </summary>
        public string Access_Token
        {
            get { return AccessTokenManage.AccessToken; }
        }


        /// <summary>
        /// 商户号
        /// </summary>
        public string Mch_ID { get; set; }
        /// <summary>
        /// 商户平台的APIKey
        /// </summary>
        public string APIKey { get; set; }


        private static ConfigInfo config = new ConfigInfo();
        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <returns></returns>
        public static ConfigInfo GetInfo()
        {
            try
            {
                //从磁盘加载对象
                string filename = ConfigValue.DataPath + "wxconfig.xml";
                XmlSerializerHelper _serialize = new XmlSerializerHelper(filename);
                ConfigInfo config = _serialize.Deserialize<ConfigInfo>();
                return config;
            }
            catch (Exception ex)
            {
                LogOperate.Write("加载 ConfigInfo 失败，" + ex.Message);
                throw new Exception("加载微信配置文件失败", ex);
            }
        }
        /// <summary>
        /// 修改配置信息
        /// </summary>
        /// <returns></returns>
        public static void UpdateInfo(ConfigInfo con)
        {
            config = con;
            //序列化到磁盘
            try
            {
                //保存到磁盘
                string filename = ConfigValue.DataPath + "wxconfig.xml";
                XmlSerializerHelper _serialize = new XmlSerializerHelper(filename);
                _serialize.Serialize<ConfigInfo>(con);
            }
            catch (Exception ex)
            {
                LogOperate.Write("序列化配置配置信息异常，" + ex.Message);
            }
        }
        public static void AddTest()
        {
            ConfigInfo config = new ConfigInfo();
            config.AppID = "wx145b4a8fd07b24e8";
            config.AppSecret = "fe6951dcb99772411c42f724b1336065";
            config.UserName = "gh_2ffac0e030f8";
            config.Token = "servicewx";
            UpdateInfo(config);
        }



        /// <summary>
        /// 签名处理------统一下单---key统一小写处理
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string GetSign(Dictionary<string, string> dic)
        {
            ConfigInfo config = ConfigInfo.GetInfo();
            string[] tempList = dic.OrderBy(q => q.Key).Select(q => q.Key.ToLower() + "=" + q.Value).ToArray();
            string signTemp = string.Join("&", tempList);
            //追加apikey
            signTemp += "&key=" + config.APIKey;

            LogOperate.Write(signTemp);
            return Common.SecurityHelper.MD5(signTemp);
        }

        /// <summary>
        /// 签名处理------统一下单---key不处理大小写
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string GetPaySign(Dictionary<string, string> dic)
        {
            ConfigInfo config = ConfigInfo.GetInfo();
            string[] tempList = dic.OrderBy(q => q.Key).Select(q => q.Key + "=" + q.Value).ToArray();
            string signTemp = string.Join("&", tempList);
            //追加apikey
            signTemp += "&key=" + config.APIKey;

            LogOperate.Write(signTemp);
            return Common.SecurityHelper.MD5(signTemp);
        }

        ///// <summary>
        ///// 签名处理------jsAPI使用
        ///// </summary>
        ///// <param name="dic"></param>
        ///// <returns></returns>
        //public static string GetJsPaySign(Dictionary<string, string> dic)
        //{
        //    ConfigInfo config = ConfigInfo.GetInfo();
        //    string[] tempList = dic.OrderBy(q => q.Key).Select(q => q.Key.ToLower() + "=" + q.Value).ToArray();
        //    string signTemp = string.Join("&", tempList);
        //    return Common.SecurityHelper.MD5(signTemp);
        //}
    }
}
