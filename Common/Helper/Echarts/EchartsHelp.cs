using System;
using System.Collections;
using System.Data;
using System.Text;

namespace Common
{
    public class EchartsHelp
    {
        /// <summary>
        /// 构建Echart 柱形图，线形图，所需要的json格式
        /// 对应单个数据源
        /// </summary>
        /// <param name="ds">DataTable</param>
        /// <param name="xAxisField">对应x轴数据源的字段：DataRow["xAxisField"]</param>
        /// <param name="yAxisField">对应y轴数据源的字段：DataRow["yAxisField"]</param>
        /// <param name="LegendName">对应数据源的名称</param>
        /// <returns>json字符串</returns>
        public static string EchartJsonToBar(DataTable dt, string xAxisField, string yAxisField, string LegendName)
        {
            StringBuilder Echartsb = new StringBuilder();
            StringBuilder Legendsb = new StringBuilder();
            StringBuilder Axissb = new StringBuilder();
            StringBuilder Seriessb = new StringBuilder();
            string axisStr = string.Empty;
            string seriesStr = string.Empty;

            Legendsb.Append("[");
            Axissb.Append("[");
            Seriessb.Append("[");
            Legendsb.Append("\"" + LegendName + "\",");

            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                Axissb.Append("\"" + dt.Rows[i][xAxisField].ToString() + "\",");
                Seriessb.Append(dt.Rows[i][yAxisField].ToString() + ",");
            }
            //foreach (DataRow dr in dt.Rows)
            //{
            //    Axissb.Append("\"" + dr[xAxisField].ToString() + "\",");
            //    Seriessb.Append(dr[yAxisField].ToString() + ",");
            //}
            axisStr += Axissb.ToString().TrimEnd(',') + "],";
            seriesStr += Seriessb.ToString().TrimEnd(',') + "],";
            Axissb.Clear();
            Seriessb.Clear();

            Echartsb.Append("{\"legen\":" + Legendsb.ToString().TrimEnd(',') + "],\"axis\":" + axisStr.ToString().TrimEnd(',') + ",\"series\":" + seriesStr.ToString().TrimEnd(',') + "}");

            return Echartsb.ToString();
        }

        /// <summary>
        /// 构建Echart 柱形图，线形图，所需要的json格式
        /// 对应多数据源对比
        /// </summary>
        /// <param name="ds">DataSet集合</param>
        /// <param name="xAxisSource">统一的x轴数据</param>
        /// <param name="xAxisField">对应x轴数据源的字段:DataRow["xAxisField"]</param>
        /// <param name="yAxisField">对应y轴数据源的字段:DataRow["yAxisField"]</param>
        /// <returns>json字符串</returns>
        public static string EchartJsonToMultipleBar(DataSet ds, ArrayList xAxisSource, string xAxisField, string yAxisField)
        {
            StringBuilder Echartsb = new StringBuilder();
            StringBuilder Legendsb = new StringBuilder();
            StringBuilder Axissb = new StringBuilder();
            StringBuilder Seriessb = new StringBuilder();
            StringBuilder Toltal = new StringBuilder();
            string axisStr = string.Empty;
            string seriesStr = string.Empty;
            int i = 0;
            Legendsb.Append("[");
            foreach (DataTable item in ds.Tables)
            {
                var sum = 0.0;
                Axissb.Append("\"Axis" + i + "\":[");
                Seriessb.Append("\"Series" + i + "\":[");

                //,\"totalount\":" + sum + "
                Legendsb.Append("\"" + item.TableName + "\",");
                for (int y = 0; y < xAxisSource.Count; y++)
                {
                    var xAxisValue = xAxisSource[y].ToString();
                    var yAxisValue = 0.0;
                    foreach (DataRow dr in item.Rows)
                    {
                        var aa = dr[xAxisField].ToString();
                        var bb = xAxisSource[y].ToString();
                        if (xAxisSource[y].ToString().Replace('点', ' ').Replace('月', ' ').Replace('年', ' ').Trim() == dr[xAxisField].ToString().Trim().Replace('省', ' ').Replace('市', ' ').Trim())
                        {
                            xAxisValue = xAxisSource[y].ToString();//dr[xAxisField].ToString();
                            yAxisValue = Convert.ToDouble(dr[yAxisField]);
                            sum += yAxisValue;
                        }
                    }
                    Axissb.Append("\"" + xAxisValue + "\",");
                    Seriessb.Append(yAxisValue + ",");
                }

                axisStr += Axissb.ToString().TrimEnd(',') + "],";
                seriesStr += Seriessb.ToString().TrimEnd(',') + "],";
                Toltal.Append(item.TableName + ":" + sum + ",");
                Axissb.Clear();
                Seriessb.Clear();
                i++;
            }
            Echartsb.Append("{\"legen\":" + Legendsb.ToString().TrimEnd(',') + "],\"axis\":{" + axisStr.ToString().TrimEnd(',') + "},\"series\":{" + seriesStr.ToString().TrimEnd(',') + "},\"total\":\"" + Toltal.ToString().TrimEnd(',') + "\"}");

            return Echartsb.ToString();
        }

        /// <summary>
        /// 构建Echart 饼图，所需要的json格式
        /// </summary>
        /// <param name="ds">dataTable</param>
        /// <param name="xAxisField">对应x轴数据源的字段：DataRow["xAxisField"]</param>
        /// <param name="yAxisField">对应y轴数据源的字段：DataRow["yAxisField"]</param>
        /// <param name="LegendName">对应数据源的名称</param>
        /// <returns>json字符串</returns>
        public static string EchartJsonToPie(DataTable dt, string NameField, string ValueField, string LegendName)
        {
            StringBuilder Echartsb = new StringBuilder();
            StringBuilder Legendsb = new StringBuilder();
            StringBuilder Seriessb = new StringBuilder();

            string seriesStr = string.Empty;

            Legendsb.Append("[");
            Seriessb.Append("[");


            foreach (DataRow dr in dt.Rows)
            {
                Legendsb.Append("\"" + dr[NameField].ToString() + "\",");
                Seriessb.Append("{\"value\":" + dr[ValueField].ToString() + ",\"name\":\"" + dr[NameField].ToString() + "\"},");
            }
            seriesStr += Seriessb.ToString().TrimEnd(',') + "],";
            Seriessb.Clear();

            Echartsb.Append("{\"legen\":" + Legendsb.ToString().TrimEnd(',') + "],\"series\":" + seriesStr.ToString().TrimEnd(',') + "}");

            return Echartsb.ToString();
        }
    }
}
