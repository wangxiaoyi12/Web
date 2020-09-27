using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace System
{
    public class JsonHelp
    {
        #region 构造器
        public JsonHelp()
        {
            IsSuccess = false;
            Msg = "操作失败";
        }
        /// <summary>
        /// 带参数的构造器
        /// </summary>
        /// <param name="isSuccess">是否成功</param>
        /// <param name="msg">提示信息，默认为空，isSuccess=true时：操作成功，isSuccess=false：操作失败</param>
        public JsonHelp(bool isSuccess, string msg = null)
        {            
            if (msg == null)
            {
                if (isSuccess == false)
                {
                    msg = "操作失败";
                }
                else
                {
                    msg = "操作成功";
                }
            }
            IsSuccess = isSuccess;
            Msg = msg;
        }
        #endregion
        /// <summary>
        /// 关联对象
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 操作信息提示
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 跳转URL
        /// </summary>
        public string ReUrl { get; set; }
        /// <summary>
        /// 操作状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess
        {
            get
            {
                return Status == "y";
            }
            set
            {
                if (value == true)
                {
                    Status = "y";
                }
                else
                {
                    Status = "n";
                }
            }
        }
        public static string ConvertToTablesJson(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();


            //---------------   
            foreach (DataRow item in dt.Rows)
            {
                sb1.Append("[");
                //---------------   
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sb2.Append("\"" + item[i].ToString() + "\",");
                }
                string str1 = sb2.ToString().TrimEnd(',');
                //---------------   
                sb1.Append(str1);
                sb2.Clear();
                sb1.Append("],");

            }
            string str2 = sb1.ToString().TrimEnd(',');
            //---------------   

            sb.Append("{");
            sb.Append("\"data\":[");
            sb.Append(str2);
            sb.Append("]");
            sb.Append("}");
            return sb.ToString();
        }
        public static string ConvertToTablesJson<T>(IEnumerable<T> list) where T : class
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            //---------------   
            Type types = typeof(T);
            System.Reflection.PropertyInfo[] ps = types.GetProperties();
            foreach (var item in list)
            {
                sb1.Append("{");
                //---------------   
                foreach (PropertyInfo i in ps)
                {
                    sb2.Append("\"" + i.Name + "\":\"" + i.GetValue(item, null) + "\",");
                }
                string str1 = sb2.ToString().TrimEnd(',');
                //---------------   
                sb1.Append(str1);
                sb2.Clear();
                sb1.Append("},");

            }
            string str2 = sb1.ToString().TrimEnd(',');
            //---------------   
            sb.Append("{");
            sb.Append("\"data\":[");
            sb.Append(str2);
            sb.Append("]");
            sb.Append("}");
            return sb.ToString();
        }
        public static string ConvertToTablesJsonBySqlQuery(string query)
        {
            StringBuilder sb = new StringBuilder();
            //---------------   
            sb.Append("{");
            sb.Append("\"data\":");
            sb.Append(query);

            sb.Append("}");
            return sb.ToString();
        }
    }
}
