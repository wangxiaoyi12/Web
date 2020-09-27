using System;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Common
{
    public class WebTools
    {
        #region 复制对象，主要是对于数据库表某条记录 每个字段分别复制其内容（可以排除不复制的字段）

        /// <summary> 
        /// 把源对象的值复制到目标对象 
        /// </summary> 
        /// <param name="SourceObject">源对象</param> 
        /// <param name="TargetObject">目标对象</param> 
        /// <param name="isNullCopy">null属性不赋值</param> 
        /// <returns>返回AOPResult</returns> 
        public static JsonHelp CopyToObject(object SourceObject, object TargetObject, bool isNullCopy = false)
        {
            try
            {
                Type TS = SourceObject.GetType();
                Type TT = TargetObject.GetType();
                //if (!TS.Equals(TT))
                //{
                //    return new JsonHelp(false, "源对象与目标对象的类型不一致");
                //}
                FieldInfo[] fieldInfos = TS.GetFields(BindingFlags.Instance | BindingFlags.Public);
                PropertyInfo[] propertyInfos = TS.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var item in fieldInfos)
                {
                    var value = item.GetValue(SourceObject);
                    if (value != null || (value == null && isNullCopy))
                    {
                        item.SetValue(TargetObject, value);
                    }
                }
                foreach (var item in propertyInfos)
                {
                    if (item.CanWrite == true && (item.PropertyType.IsValueType == true || item.PropertyType == typeof(String)))
                    {
                        var value = item.GetValue(SourceObject, null);
                        if (value != null || (value == null && isNullCopy))
                        {
                            if (item.PropertyType == typeof(DateTime))
                            {
                                if (Convert.ToDateTime(value).ToString("yyyy") != "0001")
                                {
                                    item.SetValue(TargetObject, value, null);
                                }
                            }
                            else
                            {
                                item.SetValue(TargetObject, value, null);
                            }
                        }
                    }
                }
                return new JsonHelp(true);
            }
            catch (Exception Exc)
            {
                return new JsonHelp(false, Exc.Message);
            }
        }

        /// <summary> 
        /// 把源对象的值复制到目标对象,排除不需要复制的字段 
        /// </summary> 
        /// <param name="SourceObject">源对象</param> 
        /// <param name="TargetObject">目标对象</param> 
        /// <param name="field">不需要复制的字段</param> 
        /// <returns>返回AOPResult</returns> 
        public static JsonHelp CopyToObject(object SourceObject, object TargetObject, string[] field, bool isNullCopy = false)
        {
            try
            {
                Type TS = SourceObject.GetType();
                Type TT = TargetObject.GetType();
                if (!TS.Equals(TT))
                {
                    return new JsonHelp(false, "源对象与目标对象的类型不一致");
                }
                FieldInfo[] fieldInfos = TS.GetFields(BindingFlags.Instance | BindingFlags.Public);
                PropertyInfo[] propertyInfos = TS.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                StringBuilder sb = new StringBuilder();
                sb.Append(",");
                foreach (var item in field)
                {
                    sb.Append(item + ",");
                }
                foreach (var item in fieldInfos)
                {
                    if (!sb.ToString().Contains("," + item.Name + ","))
                    {
                        var value = item.GetValue(SourceObject);
                        if (value != null || (value == null && isNullCopy))
                        {
                            item.SetValue(TargetObject, value);
                        }
                    }
                }
                foreach (var item in propertyInfos)
                {
                    if (!sb.ToString().Contains("," + item.Name + ","))
                    {
                        if (item.CanWrite == true && (item.PropertyType.IsValueType == true || item.PropertyType == typeof(String)))
                        {
                            var value = item.GetValue(SourceObject, null);
                            if (value != null || (value == null && isNullCopy))
                            {
                                item.SetValue(TargetObject, value, null);
                            }
                        }
                    }
                }
                return new JsonHelp(true);
            }
            catch (Exception Exc)
            {
                return new JsonHelp(false, Exc.Message);
            }
        }
        #endregion

        #region 读取WebConfig---AppSetting节的配置
        /// <summary>
        /// 读取AppSetting节的配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppConfig(string key)
        {
            try
            {
                return System.Configuration.ConfigurationManager.AppSettings[key];
            }
            catch (Exception e)
            {
                LogHelper.Error(string.Format("读取AppSetting配置节异常，要读取的Key 为 {0},，失败原因：{1}", key, e.Message));
                return null;
            }
        }
        #endregion

        #region 获取客户端IP地址（无视代理）
        /// <summary>
        /// 获取客户端IP地址（无视代理）
        /// </summary>
        /// <returns>若失败则返回回送地址</returns>
        public static string GetHostAddress()
        {
            string userHostAddress = HttpContext.Current.Request.UserHostAddress;

            if (string.IsNullOrEmpty(userHostAddress))
            {
                userHostAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            //最后判断获取是否成功，并检查IP地址的格式（检查其格式非常重要）
            if (!string.IsNullOrEmpty(userHostAddress) && IsIP(userHostAddress))
            {
                return userHostAddress;
            }
            return "127.0.0.1";
        }
        /// <summary>
        /// 检查IP地址格式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        #endregion

       

        #region 通用转换类型
        public static object ChangeType(object value, Type type)
        {
            try
            {
                if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
                if (value == null) return null;
                if (type == value.GetType()) return value;
                if (type.IsEnum)
                {
                    if (value is string)
                        return Enum.Parse(type, value as string);
                    else
                        return Enum.ToObject(type, value);
                }
                if (!type.IsInterface && type.IsGenericType)
                {
                    Type innerType = type.GetGenericArguments()[0];
                    object innerValue = ChangeType(value, innerType);
                    return Activator.CreateInstance(type, new object[] { innerValue });
                }
                if (value is string && type == typeof(Guid)) return new Guid(value as string);
                if (value is string && type == typeof(Version)) return new Version(value as string);
                if (!(value is IConvertible)) return value;
                return Convert.ChangeType(value, type);
            }
            catch
            {

            }
            return value;
        }
        #endregion

        #region 获取最终异常
        /// <summary>
        /// 获取最终的异常信息
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string getFinalException(Exception e)
        {
            if (e.InnerException != null)
            {
                return getFinalException(e.InnerException);
            }
            return e.Message;
        }
        #endregion

        #region 获取绝对路径
        /// <summary>
        /// 获取最终的异常信息
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static string MapPath(string path)
        {
            return AppDomain.CurrentDomain.BaseDirectory + path;
        }
        #endregion
    }
}
