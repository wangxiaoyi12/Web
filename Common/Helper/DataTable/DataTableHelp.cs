using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Common
{
    /// <summary>
    /// add duk by 20160624
    /// </summary>
    public class DataTableHelp
    {
        /// <summary>
        /// datatable 转为 List<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ConvertTo<T>(DataTable dt) where T : new()
        {
            if (dt == null) return null;
            if (dt.Rows.Count <= 0) return null;

            List<T> list = new List<T>();
            try
            {
                List<string> columnsName = new List<string>();
                foreach (DataColumn dataColumn in dt.Columns)
                {
                    columnsName.Add(dataColumn.ColumnName);//得到所有的表头
                }
                list = dt.AsEnumerable().ToList().ConvertAll<T>(row => getObject<T>(row, columnsName));  //转换
                return list;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 将集合类转换成DataTable
        /// </summary>
        /// <param name="list">集合</param>
        /// <returns></returns>
        public static DataTable ToDataTable(IList list)
        {
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (pi.PropertyType == typeof(DateTime?))
                    {
                        result.Columns.Add(pi.Name, typeof(DateTime));
                    }
                    else
                    {
                        result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }
        public static T getObject<T>(DataRow row, List<string> columnsName) where T : new()
        {
            T obj = new T();
            try
            {
                string columnname = "";
                string value = "";
                PropertyInfo[] Properties = typeof(T).GetProperties();
                foreach (PropertyInfo objProperty in Properties)  //遍历T的属性
                {
                    columnname = columnsName.Find(name => name.ToLower() == objProperty.Name.ToLower()); //寻找可以匹配的表头名称
                    if (!string.IsNullOrEmpty(columnname))
                    {
                        value = row[columnname].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            if (Nullable.GetUnderlyingType(objProperty.PropertyType) != null) //存在匹配的表头
                            {
                                value = row[columnname].ToString().Replace("$", "").Replace(",", ""); //从dataRow中提取数据
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(Nullable.GetUnderlyingType(objProperty.PropertyType).ToString())), null); //赋值操作
                            }
                            else
                            {
                                value = row[columnname].ToString().Replace("%", ""); //存在匹配的表头
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(objProperty.PropertyType.ToString())), null);//赋值操作
                            }
                        }
                    }
                }
                return obj;
            }
            catch
            {
                return obj;
            }
        }
    }
}
